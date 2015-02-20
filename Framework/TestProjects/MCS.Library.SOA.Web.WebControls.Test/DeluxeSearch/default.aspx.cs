using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class Default : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            BoundField field = new BoundField()
            {
                //标记
                DataField = "SupplierName",
                HeaderText = "SupplierName",
            };
            this.relativeLinkGroupGrid.Columns.Insert(2, field);
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


          


                InitializeData();
                
            }

            //if (!IsPostBack)
            //{
                //Category cat = new Category();

                //cat.CategoryField = "type";
                //cat.DataTextField = "供应商类型";
                //cat.DataValueField = "TypeName";
                //cat.DataSourceID = "testDataSource1";

                //cat.ConditionText = "TypeName";
                //cat.ConditionValue = "TypeValue";

                //DeluxeSearch1.Categories.Add(cat);
            //}

        }

        protected override void CreateChildControls()
        {
            Category cat = new Category();

            cat.CategoryField = "Status";
            cat.DataTextField = "供应商类型";
            cat.DataValueField = "type";
            cat.DataSourceID = "testDataSource1";

            cat.ConditionText = "TypeName";
            cat.ConditionValue = "TypeValue";

            DeluxeSearch1.Categories.Add(cat);
            base.CreateChildControls();
        }


        private void InitializeData()
        {
            ddlType.BindData(new TestResult().GetTypeItem(), "TypeValue", "TypeName");
            ddlScope.BindData(new TestResult().GetAreaItem(), "AreaValue", "AreaName");

            DeluxeSearch1.UserCustomSearchConditions = UserCustomSearchConditionAdapter.Instance.Load(c =>
                {
                    c.AppendItem("RESOURCE_ID", "12c2e9c8-a6e1-4e48-a3d2-17537c750272");
                    c.AppendItem("CONDITION_TYPE", "test");
                });
        }

        protected void BtnSearchClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
        {
            DeluxeSearch1.HasCategory = true;
            if (DeluxeSearch1.IsAdvanceSearching)
            {
                DeluxeSearch1.ClearWhereSqlClauses();
            }
            supplierGrid.Condition = DeluxeSearch1.GetCondition();//GetSqlString();//ConnectiveSqlClauses.ToSqlString(TSqlBuilder.Instance);            
            supplierGrid.LastQueryRowCount = -1;
            relativeLinkGroupGrid.SelectedKeys.Clear();
            relativeLinkGroupGrid.PageIndex = 0;
            relativeLinkGroupGrid.DataBind();            
         
            if(e.IsSaveCondition&&!string.IsNullOrEmpty(e.ConditionName))
            {
                UserCustomSearchCondition condition = new UserCustomSearchCondition()
                                                          {
                                                              ID = Guid.NewGuid().ToString(),
                                                              UserID = DeluxeSearch1.User.ID,
                                                              ResourceID = "12c2e9c8-a6e1-4e48-a3d2-17537c750272",
                                                              ConditionName = e.ConditionName,
                                                              ConditiontType = "test",
                                                              CreateTime = DateTime.Now
                                                          };

                bindingControl.Data = new SupplierSearch();
				bindingControl.CollectData();

                condition.ConditionContent = JSONSerializerExecute.Serialize(bindingControl.Data);

                UserCustomSearchConditionAdapter.Instance.Update(condition);
            }

            DeluxeSearch1.UserCustomSearchConditions = UserCustomSearchConditionAdapter.Instance.Load(c =>
            {
                c.AppendItem("RESOURCE_ID", "12c2e9c8-a6e1-4e48-a3d2-17537c750272");
                c.AppendItem("CONDITION_TYPE", "test");
            });
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            relativeLinkGroupGrid.DataBind();
        }
       
    }
}