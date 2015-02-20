using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using System;
using System.Web.UI.WebControls;

namespace MCS.OA.CommonPages.UserOperationLog
{
    public partial class UserOperationLogView : System.Web.UI.Page
    {
        private LogQueryCondition QueryCondition
        {
            get
            {
                LogQueryCondition result = (LogQueryCondition)ViewState["QueryCondition"];

                if (null == result)
                {
                    result = new LogQueryCondition();
                    ViewState["QueryCondition"] = result;
                }
                return result;
            }
        }

        private int LastQueryRowCount
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
            }
        }

        [ControllerMethod(true)]
        protected void DefaultProcess()
        {
            //CurrentLogViewMode = LogViewMode.Form;
            //ExtControllerHelper.BindAllFormCategoriesDDLData(this.FormCategory);//Note：当前日志的浏览模式以及分类模式取消
            ProgramName.Items.Clear();
            ProgramName.Items.Insert(0, "全部");
            compatible.Text = "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=7\" />";

            ExecQuery();
        }

        [ControllerMethod(true)]
        protected void DefaultProcess(string resourceID)
        {
            //CurrentLogViewMode = LogViewMode.Form;//Note：日志浏览模式
            QueryCondition.ResourceID = resourceID;
            this.BindlogDeluxeGrid();
        }

        [ControllerMethod]
        protected void DefaultProcess(string resourceID, string processID)
        {
            QueryCondition.ResourceID = resourceID;
            QueryCondition.ProcessID = processID;

            if (string.IsNullOrEmpty(resourceID) == true && string.IsNullOrEmpty(processID) == true)
            {
                QueryCondition.TagNull = 2;
            }

            this.BindlogDeluxeGrid();
        }

        private void BindlogDeluxeGrid()
        {
            divSearch.Visible = false;
            divClose.Visible = true;
            compatible.Text = "";
            //DeluxeGridLog.Columns[0].ItemStyle.Width = Unit.Percentage(40);
            DeluxeGridLog.Columns[1].Visible = false;

            ExecQuery();
        }

        [ControllerMethod]
        protected void DefaultProcessRequest(string mode)
        { //NOTE：日志浏览模式注
            //    if (null != Request.QueryString["mode"] && Request.QueryString["mode"] == LogViewMode.Normal.ToString("d"))
            //    {
            //        CurrentLogViewMode = LogViewMode.Normal;

            //        tdFormCategory.Visible = false;

            //        tdFormCateogryLbl.Visible = false;

            //        DeluxeGridLog.Columns[1].Visible = false;
            //        DeluxeGridLog.Columns[0].ItemStyle.Width = Unit.Percentage(50);

            //        DeluxeGridLog.Columns[2].Visible = true;
            //        DeluxeGridLog.Columns[4].Visible = false;
            //        DeluxeGridLog.Columns[7].Visible = false;

            //        ExecQuery();
            //    }
        }

        private void ExecQuery()
        {
            //重新收集所需要的查询内容
            bindingControl.CollectData();

            QueryCondition.ApplicationName = FormCategory.SelectedValue;
            QueryCondition.ApplicationName =
                (QueryCondition.ApplicationName == "" || QueryCondition.ApplicationName == "全部") ? "" : QueryCondition.ApplicationName;

            QueryCondition.ProgramName = ProgramName.SelectedValue;
            QueryCondition.ProgramName =
                (QueryCondition.ProgramName == "" || QueryCondition.ProgramName == "全部") ? "" : QueryCondition.ProgramName;

            //对某些查询内容进行格式处理
            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
                new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            builder.AppendTenantCode();

            //对数据源中的where进行赋值操作，自动完成查询操作
            whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);

            if (whereCondition.Value != "")
            {
                whereCondition.Value += " AND";
            }

            whereCondition.Value += " RESOURCE_ID IS NOT NULL";

            LastQueryRowCount = -1;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();
            bindingControl.Data = QueryCondition;

            WfRuntime.ProcessContext.EnableSimulation = WfClientContext.SimulationEnabled;

            if (!IsPostBack)
            {
                ControllerHelper.ExecuteMethodByRequest(this);
            }
        }

        protected void ObjectDataSourceLogs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        protected void ObjectDataSourceLogs_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        private static object AdjustQueryConditionValue(string propertyName, object data, ref bool ignored)
        {
            object result = data;
            switch (propertyName)
            {
                case "Title":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "ActivityName":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "EndDate":
                    result = ((DateTime)data).AddDays(1);
                    break;
                case "Operator":
                    result = "(" + data + ")";
                    break;
            }
            return result;
        }

        protected string GetOguObjectName(object obj)
        {
            string result = string.Empty;

            if ((obj is IOguObject) && obj != null)
            {
                IOguObject oguObj = (IOguObject)obj;

                result = string.IsNullOrEmpty(oguObj.DisplayName) ? oguObj.Name : oguObj.DisplayName;
            }

            return result;
        }

        //public string GetSubject(string sub)
        //{
        //    string result = string.Empty;//NOTE：读取分类配置节注

        //    //try
        //    //{
        //    //    FormCategorySection fc = FormCategorySection.GetConfig();
        //    //    result = fc.AllCategories[sub].Description;
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    result = sub;
        //    //}

        //    return result;
        //}

        protected void QueryBtnClick(object sender, EventArgs e)
        {
            ExecQuery();
        }

        protected void ButtonAdvanced_Click(object sender, EventArgs e)
        {
            ExecQuery();
        }

        protected void DeluxeGridLog_ExportData(object sender, EventArgs e)
        {
            bindingControl.Data = QueryCondition;
            ExecQuery();
        }

        protected void FormCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormCategory.SelectedValue != "全部")
            {
                //   ControllerHelper.BindddlFormName(FormCategory.SelectedValue, ProgramName);//NOTE：绑定下拉菜单的日志浏览模式
            }
            else
            {
                ProgramName.Items.Clear();
                ProgramName.Items.Insert(0, "全部");
            }
        }
    }
}