using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Dialogs
{
	public partial class RoleSearchDialog : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "0B163002-E082-421C-B70B-C88F9271CA4E";

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
			this.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (this.IsPostBack == false)
			{
				var appID = Request.QueryString["appId"];
				if (appID != null)
				{
					if (this.appList.SelectedValue != appID)
					{
						this.appList.SelectedValue = appID;
					}
				}

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = ProfileUtil.PageSize;
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
		}

		protected override void OnPreRenderComplete(EventArgs e)
		{
			base.OnPreRenderComplete(e);
			if (this.appList.Items.Count > 0)
			{
				this.lnkAppMan.Visible = true;
				this.lnkAppMan.NavigateUrl = "~/lists/AppRoles.aspx?app=" + Server.UrlEncode(this.appList.SelectedValue);
			}
			else
			{
				this.lnkAppMan.Visible = false;
			}
		}

		protected void appList_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void HandleOk(object sender, EventArgs e)
		{
			if (this.gridMain.SelectedKeys.Count > 0)
			{
				var rst = this.ParpareResult();

				var rstJson = JSONSerializerExecute.Serialize(rst);

				this.preScript.Text = Util.SurroundScriptBlock("finishDialog('" + rstJson + "')");
			}
		}

		protected void RefreshClick(object sender, EventArgs e)
		{
			this.dataSourceMain.LastQueryRowCount = -1;
			this.gridMain.SelectedKeys.Clear();
			this.gridMain.DataBind();
		}

		private IList<RoleDisplayItem> ParpareResult()
		{
			IList<RoleDisplayItem> rst = RoleDisplayItemAdapter.Instance.LoadByRoleIds(this.gridMain.SelectedKeys.ToArray());
			return rst;
		}
	}
}