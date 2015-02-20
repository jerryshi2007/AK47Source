using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Logs;

namespace PermissionCenter
{
	public partial class _default : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "F54BD438-EBD8-4587-A0CF-08DCD4657D6B";

		private ObjectSchemaConfigurationElementCollection schemaDefinitions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }

			[ConditionMapping("AccountDisabled")]
			public bool AccountDisabled { get; set; }

			[ConditionMapping("SchemaType")]
			public string SchemaType { get; set; }
		}

		public SCOrganization MainOrgnization
		{
			get
			{
				var relations = SchemaRelationObjectAdapter.Instance.LoadByObjectID(Util.CurrentUser.ID).Find(m => m.ParentSchemaType == "Organizations" && m.Status == SchemaObjectStatus.Normal && m.Default == true);
				return relations != null ? relations.Parent as SCOrganization : null;
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

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		protected DeluxeGrid CurrentGrid
		{
			get
			{
				switch (this.gridViews.ActiveViewIndex)
				{
					case 0:
						return this.gridMain;
					default:
						return this.grid2;
				}
			}
		}

		public override void ProcessRequest(System.Web.HttpContext context)
		{
			if (context.Request.RawUrl.IndexOf(".aspx") < 0)
			{
				context.Response.Redirect("default.aspx", true);
			}
			else
			{
				if (context.Request.QueryString["transfer"] != null)
				{
					var memberships = SCMemberRelationAdapter.Instance.LoadByMemberID(this.Request.QueryString["transfer"]);
					var app = memberships.Find(m => m.ContainerSchemaType == "Applications");
					if (app != null)
					{
						context.Response.Redirect("~/lists/Apps.aspx?id=", true); // TODO:aaaaa
					}
				}
				else
				{
					base.ProcessRequest(context);
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);



			if (this.Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.grid2.PageSize = this.gridMain.PageSize = ProfileUtil.PageSize;
				this.gridViews.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.views.ActiveViewIndex == 0)
			{
				var dic = SCSnapshotAdapter.Instance.QueryCountGroupBySchema(DateTime.MinValue);

				this.ConfigMetro(dic, SchemaInfo.FilterByCategory("Users").ToSchemaNames(), this.ltMemberCount);
				this.ConfigMetro(dic, "Groups", this.ltGroupCount);
				this.ConfigMetro(dic, "Organizations", this.ltOrgCount);
				this.ConfigMetro(dic, "Applications", this.ltAppCount);
				this.logItems.DataSource = SCOperationLogAdapter.Instance.LoadRecentSummaryLog(5);
				this.logItems.DataBind();
			}

			Util.ConfigToggleViewButton(this.gridViews.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);

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

			this.views.ActiveViewIndex = 1;
			this.searchPerformed.Value = "1";
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
						Util.SwapGrid(this.gridViews, 0, this.CurrentGrid, this.gridMain);
						break;
					default:
						ProfileUtil.ToggleGeneralBrowseMode(1);
						Util.SwapGrid(this.gridViews, 1, this.CurrentGrid, this.grid2);
						break;
				}
			}
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

		protected string UrlFor(string schemaType, string id)
		{
			string result;
			switch (schemaType)
			{
				case "Organizations":
					result = "~/lists/OUExplorer.aspx?ou=" + Server.UrlEncode(id);
					break;
				case "Roles":
					result = "~/lists/AppRoleMembers.aspx?role=" + Server.UrlEncode(id);
					break;
				case "Applications":
					result = "~/lists/AppRoles.aspx?app=" + Server.UrlEncode(id);
					break;
				case "Groups":
					result = "~/lists/GroupConstMembers.aspx?id=" + Server.UrlEncode(id);
					break;

				default:
					result = string.Empty;
					break;
			}

			return result;
		}

		protected string ClickFor(string schemaType, string id)
		{
			string result;
			switch (schemaType)
			{
				case "Roles":
				case "Applications":
				case "Groups":
					result = "return $pc.modalPopup(this);";
					break;
				default:
					result = string.Empty;
					break;
			}

			return result;
		}

		protected string SchemaTypeToString(string schemaName)
		{
			this.schemaDefinitions = this.schemaDefinitions ?? ObjectSchemaSettings.GetConfig().Schemas;
			return this.schemaDefinitions[schemaName].Description ?? schemaName;
		}

		private void ConfigMetro(System.Collections.Generic.Dictionary<string, int> dic, string key, HtmlGenericControl literal)
		{
			if (dic.ContainsKey(key))
				literal.InnerText = dic[key].ToString();
		}

		private void ConfigMetro(System.Collections.Generic.Dictionary<string, int> dic, string[] keys, HtmlGenericControl literal)
		{
			int count = 0;
			foreach (string key in keys)
			{
				if (dic.ContainsKey(key))
					count += dic[key];
			}

			literal.InnerText = count.ToString("#,##0");
		}

		protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
		{

		}
	}
}