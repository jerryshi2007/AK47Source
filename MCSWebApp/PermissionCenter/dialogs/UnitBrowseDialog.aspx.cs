using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public partial class UnitBrowseDialog : System.Web.UI.Page
	{
		private class SchemaProfile
		{
			private bool isTerminal;

			public bool Terminal
			{
				get { return isTerminal; }
				set { isTerminal = value; }
			}
			private string openImageUrl;

			public string OpenImageUrl
			{
				get { return openImageUrl; }
				set { openImageUrl = value; }
			}
			private string closeImageUrl;

			public string CloseImageUrl
			{
				get { return closeImageUrl; }
				set { closeImageUrl = value; }
			}
			private string cssClass;

			public string CssClass
			{
				get { return cssClass; }
				set { cssClass = value; }
			}

			public SchemaProfile(bool isTerminal, string openImageUrl, string closeImageUrl, string cssClass)
			{
				this.isTerminal = isTerminal;
				this.openImageUrl = openImageUrl;
				this.closeImageUrl = closeImageUrl;
				this.cssClass = cssClass;
			}
		}

		private static readonly Dictionary<string, SchemaProfile> profiles;

		private static readonly string[] DefaultSearchSchemas = SchemaInfo.FilterByCategory("Users", "Organizations", "Groups").ToSchemaNames();

		private static HashSet<string> alwaysVisibleSchemaObjects = new HashSet<string>(SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());

		static UnitBrowseDialog()
		{
			profiles = new Dictionary<string, SchemaProfile>();

			foreach (var schema in SchemaInfo.FilterByCategory("Users"))
				profiles[schema.Name] = new SchemaProfile(true, ControlResources.UserLogoUrl, ControlResources.UserLogoUrl, "pc-usernode");

			foreach (var schema in SchemaInfo.FilterByCategory("Organizations"))
				profiles[schema.Name] = new SchemaProfile(false, ControlResources.OULogoUrl, ControlResources.OULogoUrl, "pc-orgnode");

			foreach (var schema in SchemaInfo.FilterByCategory("Groups"))
				profiles[schema.Name] = new SchemaProfile(true, ControlResources.GroupLogoUrl, ControlResources.GroupLogoUrl, "pc-groupnode");
		}


		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SCOrganization root = SCOrganization.GetRoot();

			DeluxeTreeNode rootTreeNode = CreateTreeNode(root.ID, root.Name, root.DisplayName, string.Empty, root.SchemaType);

			rootTreeNode.Expanded = true;
			rootTreeNode.ShowCheckBox = false;
			this.tree.Nodes.Add(rootTreeNode);

			rootTreeNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
			string[] schemaTypes = this.Request.QueryString.GetValues("schemaType") ?? DefaultSearchSchemas;

			HashSet<string> union = new HashSet<string>(schemaTypes);
			union.UnionWith(alwaysVisibleSchemaObjects);

			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(union.ToArray(), new string[] { root.ID }, false, false, false, Util.GetTime());
			BindObjectsToTreeNodes(relations, rootTreeNode.Nodes, schemaTypes);
		}

		private static DeluxeTreeNode CreateTreeNode(string id, string name, string displayName, string fullPath, string schemaType)
		{
			DeluxeTreeNode node = new DeluxeTreeNode(displayName.IsNotEmpty() ? displayName : name, id);

			var profile = profiles[schemaType];
			node.ChildNodesLoadingType = profile.Terminal ? ChildNodesLoadingTypeDefine.Normal : ChildNodesLoadingTypeDefine.LazyLoading;
			node.ToolTip = fullPath;
			node.NodeOpenImg = profile.OpenImageUrl;
			node.NodeCloseImg = profile.CloseImageUrl;
			node.CssClass = profile.CssClass ?? "pc-orgnode";
			node.ShowCheckBox = true;

			return node;
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			string[] schemaTypes = this.Request.QueryString.GetValues("schemaType") ?? DefaultSearchSchemas;

			HashSet<string> union = new HashSet<string>(schemaTypes);
			union.UnionWith(alwaysVisibleSchemaObjects);

			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(union.ToArray(), new string[] { parentNode.Value }, false, false, false, Util.GetTime());
			BindObjectsToTreeNodes(relations, result, schemaTypes);
		}

		private static void BindObjectsToTreeNodes(SCObjectAndRelationCollection relations, DeluxeTreeNodeCollection nodes, string[] schemaTypes)
		{
			HashSet<string> filter = new HashSet<string>(schemaTypes);
			relations.Sort((m, n) => m.InnerSort.CompareTo(n.InnerSort));
			foreach (SCObjectAndRelation r in relations)
			{
				DeluxeTreeNode newTreeNode = CreateTreeNode(r.ID, r.Name, r.DisplayName, r.FullPath, r.SchemaType);
				if (filter.Contains(r.SchemaType) == false)
				{
					newTreeNode.ShowCheckBox = false;
					newTreeNode.Checked = false;
				}
				nodes.Add(newTreeNode);
			}
		}
	}
}