using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public partial class MemberRoleView : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "A850C13A-924D-4E2D-88D2-71784683A181";

		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("R.CodeName")]
			public string CodeName { get; set; }
		}

		//protected bool AdvanceSearchEnabled
		//{
		//    get
		//    {
		//        object o = this.ViewState["PageAdvanceSearch"];
		//        return (o is bool) ? (bool)o : false;
		//    }

		//    set
		//    {
		//        this.ViewState["PageAdvanceSearch"] = value;
		//    }
		//}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private SCSimpleObject UserObject
		{
			get { return (SCSimpleObject)this.ViewState["UserObject"]; }
			set { this.ViewState["UserObject"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string userId = Request.QueryString["id"];

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.UserObject = DbUtil.GetEffectiveObject<SCUser>(userId).ToSimpleObject();

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.binding1.Data = this.UserObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
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

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			(sender as LinkButton).Attributes["data-parentid"] = this.UserObject.ID;
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
			}
			//else
			//{
			//    this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
			//}
		}

		protected bool EditRoleMembersEnabled(string appID)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, appID, "ModifyMembersInRoles"));
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			System.Data.DataView view = e.ReturnValue as System.Data.DataView;
			if (view != null)
			{
				HashSet<string> parentIds = new HashSet<string>();
				HashSet<string> deleteLimitIds = new HashSet<string>();
				foreach (System.Data.DataRow row in view.Table.Rows)
				{
					string parentID = (string)row["AppID"];
					parentIds.Add(parentID);
				}

				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, parentIds);
			}
		}
	}
}