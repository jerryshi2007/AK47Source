using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Web.WebControls;

namespace PermissionCenter.Dialogs
{
	public partial class MemberSearchDialog : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "8FCAB9CF-533B-4B00-90C9-0D5A5282540F";

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

		private DeluxeGrid CurrentGrid
		{
			get
			{
				if (views.ActiveViewIndex == 0)
					return this.gridViewUsers;
				else
					return this.gridView2;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();
			this.Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.hfViewMode.Value = ProfileUtil.UserBrowseModeIndex.ToString();
				this.views.ActiveViewIndex = this.hfViewMode.Value == "2" ? 1 : 0;

				this.gridView2.PageSize = ProfileUtil.PageSize;
				this.gridViewUsers.PageSize = ProfileUtil.PageSize;
			}

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

		protected void ToggleViewMode(object sender, EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(0);
					break;
				case "1":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(1);
					break;
				case "2":
					Util.SwapGrid(this.views, 1, this.CurrentGrid, this.gridView2);
					ProfileUtil.ToggleUserBrowseMode(2);
					break;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dd list-cmd shortcut";
					this.displayFilter.InnerText = "常规列表";
					break;
				case "1":
					this.listContainer.Attributes["class"] = "pc-grid-container pc-reduced-view";
					this.lnkDisplay.CssClass = "pc-toggler-dr list-cmd shortcut";
					this.displayFilter.InnerText = "精简列表";
					break;
				case "2":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dt list-cmd shortcut";
					this.displayFilter.InnerText = "精简表格";
					break;
			}

			base.OnPreRender(e);
		}

		protected void ObjectDataSourceUsers_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			string[] permissions = this.Request.QueryString.GetValues("pp");

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

			if (permissions != null && Util.SuperVisiorMode == false)
			{
				e.InputParameters["parentPermissions"] = permissions;
				e.InputParameters["logonUserID"] = Util.CurrentUser.ID;
				e.InputParameters["excludeID"] = this.Request.QueryString["exclude"];
				e.InputParameters["defaultOnly"] = this.Request.QueryString["defaultOnly"] == "1";
			}
		}
	}
}