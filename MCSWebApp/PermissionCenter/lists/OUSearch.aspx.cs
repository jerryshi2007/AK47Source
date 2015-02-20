using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace PermissionCenter
{
	public partial class OUSearch : System.Web.UI.Page
	{
		public static readonly string ThisPageSearchResourceKey = "173C35E7-BD4D-4758-85C7-A91C953C7164";

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("O.CodeName")]
			public string CodeName { get; set; }

			[NoMapping]
			public int SchemaTypeOption { get; set; }
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

		private SCSimpleObject ParentOrganization
		{
			get
			{
				return (SCSimpleObject)ViewState["ParentOrganization"];
			}

			set
			{
				this.ViewState["ParentOrganization"] = value;
			}
		}

		private string StartFullPath
		{
			get
			{
				return (string)ViewState["StartFullPath"];
			}

			set
			{
				this.ViewState["StartFullPath"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (Page.IsPostBack == false && Page.IsCallback == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.gridMain.PageSize = ProfileUtil.PageSize;

				if (this.InitState() == false)
				{
					this.Server.Transfer("~/lists/OUNotFound.aspx");
					return;
				}
			}

			this.binding1.Data = this.ParentOrganization;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void RefreshOwnerTree(object sender, EventArgs e)
		{
			this.InnerRefreshOwnerTree();
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

		protected string SchemaTypeToString(string schemaName)
		{
			string result = schemaName;

			ObjectSchemaConfigurationElement schemaElement = ObjectSchemaSettings.GetConfig().Schemas[schemaName];

			if (schemaElement != null && schemaElement.Description.IsNotEmpty())
				result = schemaElement.Description;

			return result;
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (this.IsPostBack == false)
			{
				e.Cancel = true;
			}
			else
			{
				e.InputParameters["startPath"] = this.StartFullPath;
				e.InputParameters["schemaTypes"] = null;

				//if (this.AdvanceSearchEnabled)
				{
					var condition = this.CurrentAdvancedSearchCondition;

					WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

					this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
					switch (condition.SchemaTypeOption)
					{
						case 1:
							e.InputParameters["schemaTypes"] = SchemaInfo.FilterByCategory("Users").ToSchemaNames();
							break;
						case 2:
							e.InputParameters["schemaTypes"] = SchemaInfo.FilterByCategory("Groups").ToSchemaNames();
							break;
						case 3:
							e.InputParameters["schemaTypes"] = SchemaInfo.FilterByCategory("Organizations").ToSchemaNames();
							break;
						default:
							break;
					}
				}
				//else
				//{
				//    this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
				//}
			}
		}

		private bool InitState()
		{
			string ouID = WebUtility.GetRequestQueryString("ou", string.Empty);
			bool result = true;

			try
			{
				SCOrganization parent = DbUtil.GetEffectiveObject<SCOrganization>(ouID);
				SCRelationObject parentRelation = DbUtil.GetParentOURelation(parent.ID);

				this.ParentOrganization = parent.ToSimpleObject();
				this.StartFullPath = parentRelation != null ? parentRelation.FullPath : string.Empty;
				this.hfOuParentId.Value = parent.ID == SCOrganization.RootOrganizationID ? string.Empty : parentRelation.ParentID;
			}
			catch (ObjectNotFoundException)
			{
				result = false;
			}

			return result;
		}

		private void InnerRefreshOwnerTree()
		{
			this.InnerRefreshList();
			var script = @"Sys.Application.add_init(function(){ refreshOwnerTree();});";
			Page.ClientScript.RegisterStartupScript(this.GetType(), "orgRefresh", script, true);
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
	}
}