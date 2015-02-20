using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace PermissionCenter
{
    public partial class FlowList : System.Web.UI.Page
    {
        private readonly string thisPageSearchResourceKey = "44290C46-F958-476E-AC9F-69609DB5B3A2";

        [Serializable]
        internal class PageAdvancedSearchCondition
        {
            [ConditionMapping("D.APPLICATION_NAME", Operation = "LIKE", EscapeLikeString = true, Postfix = "%")]
            public string ApplicationName { get; set; }

            [ConditionMapping("D.PROGRAM_NAME", Operation = "LIKE", EscapeLikeString = true, Postfix = "%")]
            public string ProgramName { get; set; }

            //[ConditionMapping("D.CREATOR_NAME", Operation = "LIKE", EscapeLikeString = true, Postfix = "%")]
            //public string CreatorName { get; set; }

            [ConditionMapping("D.MODIFIER_NAME", Operation = "LIKE", EscapeLikeString = true, Postfix = "%")]
            public string ModifierName { get; set; }

            [ConditionMapping("D.IMPORT_USER_NAME", Operation = "LIKE", EscapeLikeString = true, Postfix = "%")]
            public string ImportUserName { get; set; }

            [ConditionMapping("D.CREATE_TIME", Operation = ">=")]
            public DateTime CreationDateFrom { get; set; }

            [ConditionMapping("D.CREATE_TIME", Operation = "<=", AdjustDays = 1)]
            public DateTime CreationDateTo { get; set; }

            [ConditionMapping("D.MODIFY_TIME", Operation = ">=")]
            public DateTime ModificationDateFrom { get; set; }

            [ConditionMapping("D.MODIFY_TIME", Operation = "<=", AdjustDays = 1)]
            public DateTime ModificationDateTo { get; set; }

            [ConditionMapping("D.IMPORT_TIME", Operation = ">=")]
            public DateTime ImportDateFrom { get; set; }

            [ConditionMapping("D.IMPORT_TIME", Operation = "<=", AdjustDays = 1)]
            public DateTime ImportDateTo { get; set; }
        }

        private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
        {
            get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

            set { this.ViewState["AdvSearchCondition"] = value; }
        }

        private string[] AdditionIds
        {
            get
            {
                return ViewState["AdditionIds"] as string[];
            }

            set
            {
                ViewState["AdditionIds"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Util.InitSecurityContext(this.notice);

            if (this.IsPostBack == false)
            {
                SCBase obj = (SCBase)PC.Adapters.SchemaObjectAdapter.Instance.Load(Request.QueryString["id"]);
                if (obj is SCUser)
                {
                    var userRoles = PC.Adapters.SCSnapshotAdapter.Instance.QueryUserBelongToRoles(new string[] { "Roles" }, string.Empty, new string[] { obj.ID }, false, DBTimePointActionContext.Current.TimePoint);
                    string[] additionIds = new string[userRoles.Count];
                    for (int i = additionIds.Length - 1; i >= 0; i--)
                    {
                        additionIds[i] = userRoles[i].ID;
                    }

                    this.AdditionIds = additionIds;
                }

                this.appName.Text = obj.Name;

                this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(thisPageSearchResourceKey, "Default");
                this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

                this.gridMain.PageSize = ProfileUtil.PageSize;
            }

            this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
        }

        protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["additionIds"] = this.AdditionIds;
            if (this.dataSourceMain.Condition == null)
            {
                var condition = this.CurrentAdvancedSearchCondition;

                WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);
                this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
            }
        }

        protected void RefreshList(object sender, EventArgs e)
        {
            this.InnerRefreshList();
        }

        private void InnerRefreshList()
        {
            // 重新刷新列表
            this.dataSourceMain.LastQueryRowCount = -1;
            this.gridMain.SelectedKeys.Clear();
            this.Page.PreRender += new EventHandler(this.DelayRefreshList);
        }

        private void DelayRefreshList(object sender, EventArgs e)
        {
            this.gridMain.DataBind();
        }

        protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
        {
            this.gridMain.PageIndex = 0;
            Util.UpdateSearchTip(this.DeluxeSearch);

            //this.AdvanceSearchEnabled = this.DeluxeSearch.IsAdvanceSearching;

            this.searchBinding.CollectData();

            Util.SaveSearchCondition(e, this.DeluxeSearch, thisPageSearchResourceKey, this.searchBinding.Data);

            this.InnerRefreshList();
        }
    }
}