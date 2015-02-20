using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class MemberGrpView : Page
	{
		public static readonly string ThisPageSearchResourceKey = "4DC39D82-E2D5-42FB-B4C3-C7DBB628D079";

		private SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
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

		protected DeluxeGrid CurrentGrid
		{
			get
			{
				switch (this.views.ActiveViewIndex)
				{
					case 0:
						return this.gridMain;
					default:
						return this.grid2;
				}
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string id = Request.QueryString["id"];

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.UserObject = DbUtil.GetEffectiveObject(id, string.Format("指定的群组(ID:{0})无效", id)).ToSimpleObject();

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.binding1.Data = this.UserObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.dataSourceMain.LastQueryRowCount = -1;
			this.CurrentGrid.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.CurrentGrid.DataBind();
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.CurrentGrid.PageIndex = 0;
			Util.UpdateSearchTip(this.DeluxeSearch);

			//this.AdvanceSearchEnabled = this.DeluxeSearch.IsAdvanceSearching;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void ToggleViewMode(object sender, CommandEventArgs e)
		{
			if (e.CommandName == "ToggleViewMode")
			{
				switch ((string)e.CommandArgument)
				{
					case "0":
						ProfileUtil.ToggleGeneralBrowseMode(0);
						Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridMain);
						break;
					default:
						ProfileUtil.ToggleGeneralBrowseMode(1);
						Util.SwapGrid(this.views, 1, this.CurrentGrid, this.grid2);
						break;
				}
			}
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is DataView)
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					DataView view = (DataView)e.ReturnValue;
					List<string> ouIds = new List<string>(view.Count);
					foreach (DataRow row in view.Table.Rows)
					{
						ouIds.Add(row["OUID"].ToString());
					}

					this.containerPermissions = SCAclAdapter.Instance.GetCurrentContainerAndPermissions(Util.CurrentUser.ID, ouIds);
				}
			}
		}

		protected bool IsExitEnabled(string ouId)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, ouId, "EditMembersOfGroups"));
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

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "DeleteItem":
					this.DoDeleteItem((string)e.CommandArgument);
					break;
				default:
					break;
			}
		}

		private void DoDeleteItem(string grpKey)
		{
			try
			{
				var grp = (SCGroup)DbUtil.GetEffectiveObject(grpKey, string.Format("指定的群组(ID:{0})无效", grpKey));

				var user = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);

				SCObjectOperations.InstanceWithPermissions.RemoveUserFromGroup(user, grp);
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex);
				this.notice.AddErrorInfo(ex);
			}

			this.InnerRefreshList();
		}
	}
}