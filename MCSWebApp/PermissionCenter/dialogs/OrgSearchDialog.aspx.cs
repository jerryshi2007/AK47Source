using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Dialogs
{
	public partial class OrgSearchDialog : System.Web.UI.Page
	{
		public readonly static string ThisPageSearchResourceKey = "65239AC1-5EDD-45BC-9AAE-2BEB46E363E1";

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

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			if (this.Request["mode"] == "single")
			{
				this.gridMain.MultiSelect = false;
				this.grid2.MultiSelect = false;
			}
			else
			{
				this.gridMain.MultiSelect = true;
				this.grid2.MultiSelect = true;
			}
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
			Util.EnsureOperationSafe();

			if (Page.IsPostBack == false)
			{
				string[] excludes = Request.Params.GetValues("excludes");
				if (excludes != null)
				{
					this.hfExcludes.Value = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(excludes);
				}

				var mode = (OuBrowseViewMode)UserSettings.GetSettings(Util.CurrentUser.ID).GetPropertyValue("PermissionCenter", "OuBrowseView", 0);

				switch (mode & OuBrowseViewMode.Hierarchical)
				{
					case OuBrowseViewMode.List:
						this.mainView.ActiveViewIndex = 0;
						break;
					case OuBrowseViewMode.Hierarchical:
						this.mainView.ActiveViewIndex = 1;
						break;
				}

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
			this.Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
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

		protected override void OnPreRender(EventArgs e)
		{
			bool godMode = Util.SuperVisiorMode;

			if (this.mainView.ActiveViewIndex == 1)
			{
				this.panMode.Attributes["class"] = string.Empty;
				this.toggleButton.Attributes["title"] = "切换为列表模式";

				PC.SCOrganization root = PC.SCOrganization.GetRoot();
				DeluxeTreeNode rootTreeNode = CreateTreeNode(root.ID, root.Name, root.DisplayName, string.Empty, false, false);

				rootTreeNode.Expanded = true;
				this.tree.Nodes.Add(rootTreeNode);

				rootTreeNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;

				Dictionary<string, PC.SCSimpleObject> parentsList = PrepareParentsList(PC.SCOrganization.RootOrganizationID);

				var pmLimitString = this.Request.QueryString.Get("permission");

				var requiredPermissions = pmLimitString != null ? pmLimitString.Split(',') : null;

				var excludeID = this.Request.QueryString["superOf"];

				var exceptID = this.Request.QueryString["exceptOrg"];

				PrepareTreeNodeRecursively(root.ToSimpleObject(), rootTreeNode, parentsList, godMode, DateTime.MinValue, requiredPermissions, excludeID, exceptID);

				this.tree.CallBackContext = godMode ? "godMode" : "general";
			}
			else
			{
				this.panMode.Attributes["class"] = "pc-listmode";
				this.toggleButton.Attributes["title"] = "切换为分层模式";
			}

			this.hfMode.Value = this.mainView.ActiveViewIndex.ToString();
			this.hfGod.Value = godMode ? "1" : "0";
			this.hfSingle.Value = this.Request["mode"] == "single" ? "1" : string.Empty;

			base.OnPreRender(e);
		}

		protected void HandleRowBound(object sender, GridViewRowEventArgs e)
		{
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var conditons = this.DeluxeSearch.GetCondition();

			var allConditions = new ConnectiveSqlClauseCollection(conditons);

			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				allConditions.Add(builder);
			}

			var subtreeLimit = this.Request.QueryString["superOf"];

			if (subtreeLimit != null)
			{
				var orgRelation = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(subtreeLimit)[0];
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("R.FullPath", orgRelation.FullPath + "%", "NOT LIKE");

				allConditions.Add(where);
			}
			else
			{
				subtreeLimit = this.Request.QueryString["exceptOrg"];
				if (subtreeLimit != null)
				{
					if (subtreeLimit != PC.SCOrganization.RootOrganizationID)
					{
						var orgRelation = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(subtreeLimit)[0];
						WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
						where.AppendItem("R.FullPath", orgRelation.FullPath, "<>");

						allConditions.Add(where);
					}
				}
			}

			if (this.hfExcludes.Value.Length > 0)
			{
				string[] ids = MCS.Web.Library.Script.JSONSerializerExecute.Deserialize<string[]>(this.hfExcludes.Value);

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("R.ObjectID");
				inSql.IsNotIn = true;
				inSql.AppendItem(ids);

				allConditions.Add(inSql);
			}

			var permissions = Request.QueryString.GetValues("permission");

			if (permissions != null && Util.SuperVisiorMode == false)
			{
				e.InputParameters["userID"] = Util.CurrentUser.ID;
				e.InputParameters["orgPermissions"] = permissions;
				this.dataSourceMain.Condition = allConditions;
			}
			else
			{
				this.dataSourceMain.Condition = allConditions;
			}
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			var excludeSubTree = this.Request.QueryString["superOf"];

			var exceptOrg = this.Request.QueryString["exceptOrg"];

			var exceptID = excludeSubTree ?? exceptOrg;

			bool godBehavior = callBackContext == "godMode";

			if (excludeSubTree != parentNode.Value)
			{
				PC.SCObjectAndRelationCollection relations = PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(PC.SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(), new string[] { parentNode.Value }, false, true, false, DateTime.MinValue);

				var pmLimitString = this.Request.QueryString.Get("permission");

				var requiredPermissions = pmLimitString != null ? pmLimitString.Split(',') : null;

				PC.Permissions.SCContainerAndPermissionCollection permissions = null;

				if (requiredPermissions != null)
				{
					permissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, relations.ToIDArray());
				}

				BindObjectsToTreeNodes(relations, result, godBehavior, requiredPermissions, permissions, excludeSubTree, exceptID);
			}
		}

		protected void ToggleModeHandler(object sender, EventArgs e)
		{
			this.mainView.ActiveViewIndex = this.mainView.ActiveViewIndex == 0 ? 1 : 0;

			var settings = UserSettings.LoadSettings(Util.CurrentUser.ID);

			var cate = settings.Categories["PermissionCenter"];

			var mode = (OuBrowseViewMode)cate.Properties.GetValue("OuBrowseView", 0);

			if ((mode & OuBrowseViewMode.Fixed) != OuBrowseViewMode.Fixed)
			{
				switch (this.mainView.ActiveViewIndex)
				{
					case 0:
						mode = mode & ~OuBrowseViewMode.Hierarchical;
						break;
					case 1:
						mode = mode | OuBrowseViewMode.Hierarchical;
						break;
				}
			}

			cate.Properties.SetValue("OuBrowseView", (int)mode);

			settings.Update();
		}

		private static Dictionary<string, PC.SCSimpleObject> PrepareParentsList(string navOu)
		{
			Dictionary<string, PC.SCSimpleObject> result = null;

			if (navOu != PC.SCOrganization.RootOrganizationID)
			{
				Dictionary<string, PC.SCSimpleObjectCollection> parents = PC.Adapters.SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, navOu);

				if (parents != null && parents.ContainsKey(navOu))
				{
					Dictionary<string, PC.SCSimpleObject> queue = parents[navOu].ToDictionary();

					result = parents[navOu].ToDictionary();
				}
			}
			else
			{
				PC.SCOrganization root = PC.SCOrganization.GetRoot();

				result = new Dictionary<string, PC.SCSimpleObject>() { { root.ID, root.ToSimpleObject() } };
			}

			return result;
		}

		private static void PrepareTreeNodeRecursively(PC.SCSimpleObject parentOrg, DeluxeTreeNode parentTreeNode, Dictionary<string, PC.SCSimpleObject> parentsList, bool godMode, DateTime timePoint, string[] requiredPermissions, string excludeId, string exceptID)
		{
			if (excludeId != parentOrg.ID)
			{
				PC.SCObjectAndRelationCollection relations = PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(PC.SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(), new string[] { parentOrg.ID }, false, true, false, timePoint);

				PC.Permissions.SCContainerAndPermissionCollection permissions = null;

				if (requiredPermissions != null)
				{
					permissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, relations.ToIDArray());
				}

				BindObjectsToTreeNodes(relations, parentTreeNode.Nodes, godMode, requiredPermissions, permissions, excludeId, exceptID);

				foreach (DeluxeTreeNode childNode in parentTreeNode.Nodes)
				{
					PC.SCSimpleObject obj = null;

					if (parentsList.TryGetValue(childNode.Value, out obj))
					{
						childNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
						childNode.Expanded = true;

						PrepareTreeNodeRecursively(obj, childNode, parentsList, godMode, timePoint, requiredPermissions, excludeId, exceptID);
					}
				}
			}
		}

		private static DeluxeTreeNode CreateTreeNode(string id, string name, string displayName, string fullPath, bool selectable, bool showCheckBox)
		{
			DeluxeTreeNode node = new DeluxeTreeNode(displayName.IsNotEmpty() ? displayName : name, id);

			node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			node.ToolTip = fullPath;
			node.NodeOpenImg = selectable ? ControlResources.OULogoUrl : ControlResources.OULogoUrl;
			node.NodeCloseImg = selectable ? ControlResources.OULogoUrl : ControlResources.OULogoUrl;

			node.ShowCheckBox = showCheckBox;

			if (selectable == false)
			{
				node.CssClass = "pc-node-exclude";
				node.SelectedCssClass = "pc-node-exclude";
				node.ExtendedData = "noselect";
			}

			return node;
		}

		private static void BindObjectsToTreeNodes(PC.SCObjectAndRelationCollection relations, DeluxeTreeNodeCollection nodes, bool godMode, string[] requiredPermissions, PC.Permissions.SCContainerAndPermissionCollection permissions, string excludeID, string exceptID)
		{
			relations.Sort((m, n) => m.InnerSort.CompareTo(n.InnerSort));
			foreach (PC.SCObjectAndRelation r in relations)
			{
				if (r.ID != excludeID && Util.IsOrganization(r.SchemaType))
				{
					bool showCheckBoxes;
					bool selectable;

					if (r.ID == exceptID)
					{
						selectable = false;
					}
					else if (godMode)
					{
						selectable = true;
					}
					else
					{
						selectable = true;
						if (requiredPermissions != null && permissions != null)
						{
							foreach (string p in requiredPermissions)
							{
								selectable &= Util.ContainsPermission(permissions, r.ID, p);
							}
						}
					}

					if (HttpContext.Current.Request.QueryString["mode"] == "single")
					{
						showCheckBoxes = false;
					}
					else
					{
						showCheckBoxes = selectable;
					}

					DeluxeTreeNode newTreeNode = CreateTreeNode(r.ID, r.Name, r.DisplayName, r.FullPath, selectable, showCheckBoxes);

					nodes.Add(newTreeNode);
				}
			}
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
	}
}