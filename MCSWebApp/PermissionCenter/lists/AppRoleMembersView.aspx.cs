using System;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AppRoleMembersView : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "F0420F52-9933-4E7E-897E-515C9FF919DB";
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }
		}

		[Serializable]
		internal class RoleAndAppData
		{
			public string RoleID { get; set; }

			public string AppID { get; set; }

			public string AppName { get; set; }

			public string AppCodeName { get; set; }

			public string RoleName { get; set; }

			public string RoleCodeName { get; set; }

			public string RoleDisplayName { get; set; }

			public string VisibleName
			{
				get
				{
					if (string.IsNullOrEmpty(RoleDisplayName))
						return RoleName;
					else
						return RoleDisplayName;
				}
			}
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

		protected bool EditRoleMembersEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.RoleAndAppObject.AppID, "ModifyMembersInRoles"));
			}
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private RoleAndAppData RoleAndAppObject
		{
			get { return (RoleAndAppData)this.ViewState["RoleAndAppObject"]; }
			set { this.ViewState["RoleAndAppObject"] = value; }
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
			string roleId = Request.QueryString["role"];

			this.Page.Response.CacheControl = "no-cache";

			this.calcProgress.Tag = roleId;
			if (Page.IsPostBack == false)
			{
				SCRole role = (SCRole)MCS.Library.SOA.DataObjects.Security.Adapters.SchemaObjectAdapter.Instance.Load(roleId);

				if (role == null || role.Status != SchemaObjectStatus.Normal)
					throw new ObjectNotFoundException("未查询到指定的角色或角色已不再有效");

				SCApplication app = DbUtil.LoadApplicationForRole(role);

				this.RoleAndAppObject = new RoleAndAppData() { AppID = app.ID, AppCodeName = app.CodeName, AppName = app.Name, RoleCodeName = role.CodeName, RoleID = role.ID, RoleName = role.Name, RoleDisplayName = role.DisplayName };

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.binding1.Data = this.RoleAndAppObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Util.SuperVisiorMode == false)
			{
				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.RoleAndAppObject.AppID });
			}

			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);

			roleMatrixEntryControl.AppID = this.RoleAndAppObject.AppID;
			roleMatrixEntryControl.AppCodeName = this.RoleAndAppObject.AppCodeName;
			roleMatrixEntryControl.AppName = this.roleMatrixEntryControl.AppName;
			roleMatrixEntryControl.RoleID = this.RoleAndAppObject.RoleID;
			roleMatrixEntryControl.RoleName = this.RoleAndAppObject.RoleName;
			roleMatrixEntryControl.RoleCodeName = this.RoleAndAppObject.RoleCodeName;
			roleMatrixEntryControl.RoleDescription = this.RoleAndAppObject.RoleDisplayName;
			roleMatrixEntryControl.Enabled = this.EditRoleMembersEnabled == false;
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

		protected void ProcessCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			SCConditionCalculator calc = new SCConditionCalculator();

			calc.GenerateUserAndContainerSnapshot(new[] { (SCRole)SchemaObjectAdapter.Instance.Load(this.calcProgress.Tag) });

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void ProcessGlobalCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			SCConditionCalculator calc = new SCConditionCalculator();

			calc.GenerateAllUserAndContainerSnapshot();

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}
	}
}