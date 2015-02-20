using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace PermissionCenter.Dialogs
{
	public partial class GroupSearchDialog : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "AD2C0441-98BD-48EC-99B7-DB2E7E13C7EC";
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

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();
			this.Page.Response.CacheControl = "no-cache";

			Util.InitSecurityContext(this.notice);

			if (this.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected bool IsDeleteEnabled(string ouId)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, ouId, "DeleteChildren"));
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

			string pp = Request.QueryString.Get("pp");
			if (pp != null && Util.SuperVisiorMode == false)
			{
				string[] permissions = pp.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
				e.InputParameters["userID"] = Util.CurrentUser.ID;
				e.InputParameters["parentPermissions"] = permissions;
			}
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is DataView)
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					var view = (DataView)e.ReturnValue;
					List<string> ouIds = new List<string>(view.Count);

					foreach (DataRow row in view.Table.Rows)
					{
						ouIds.Add(row["ParentID"].ToString());
					}

					this.containerPermissions = SCAclAdapter.Instance.GetCurrentContainerAndPermissions(Util.CurrentUser.ID, ouIds);
				}
			}
		}
	}
}