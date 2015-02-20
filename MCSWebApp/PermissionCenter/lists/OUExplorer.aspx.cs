using System;
using System.Collections.Generic;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Web.Library;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public partial class OrgExplorer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void ReloadTree(object sender, EventArgs e)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (string.IsNullOrEmpty(this.lastVisitOrg.Value) == false)
			{
				this.Response.Redirect("OUExplorer.aspx?ou=" + Server.UrlEncode(this.lastVisitOrg.Value), true);
			}
			else
			{
				base.OnPreRender(e);

				SCOrganization root = SCOrganization.GetRoot();

				if (this.IsPostBack == false)
					this.navOUID.Value = WebUtility.GetRequestQueryString("ou", root.ID);

				DeluxeTreeNode rootTreeNode = CreateTreeNode(root.ID, root.Name, root.DisplayName, string.Empty);

				rootTreeNode.Expanded = true;
				this.tree.Nodes.Add(rootTreeNode);

				rootTreeNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;

				Dictionary<string, SCSimpleObject> parentsList = PrepareParentsList(this.navOUID.Value);

				PrepareTreeNodeRecursively(root.ToSimpleObject(), rootTreeNode, parentsList, Util.GetTime(), this.navOUID.Value);
			}
		}

		protected void tree_GetChildrenData(DeluxeTreeNode parentNode, DeluxeTreeNodeCollection result, string callBackContext)
		{
			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(PC.SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(), new string[] { parentNode.Value }, false, true, false, Util.GetTime());
			BindObjectsToTreeNodes(relations, result);
		}

		private static DeluxeTreeNode CreateTreeNode(string id, string name, string displayName, string fullPath)
		{
			DeluxeTreeNode node = new DeluxeTreeNode(displayName.IsNotEmpty() ? displayName : name, id);

			node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			node.ToolTip = fullPath;
			node.NodeOpenImg = ControlResources.OULogoUrl;
			node.NodeCloseImg = ControlResources.OULogoUrl;
			node.CssClass = "pc-orgnode";

			return node;
		}

		private static Dictionary<string, SCSimpleObject> PrepareParentsList(string navOu)
		{
			Dictionary<string, SCSimpleObject> result = null;

			if (navOu != SCOrganization.RootOrganizationID)
			{
				Dictionary<string, SCSimpleObjectCollection> parents = PC.Adapters.SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, navOu);

				if (parents != null && parents.ContainsKey(navOu))
				{
					Dictionary<string, SCSimpleObject> queue = parents[navOu].ToDictionary();

					result = parents[navOu].ToDictionary();
				}
			}
			else
			{
				SCOrganization root = SCOrganization.GetRoot();

				result = new Dictionary<string, SCSimpleObject>() { { root.ID, root.ToSimpleObject() } };
			}

			return result;
		}

		private static void BindObjectsToTreeNodes(SCObjectAndRelationCollection relations, DeluxeTreeNodeCollection nodes)
		{
			relations.Sort((m, n) => m.InnerSort.CompareTo(n.InnerSort));
			foreach (SCObjectAndRelation r in relations)
			{
				if (Util.IsOrganization(r.SchemaType))
				{
					DeluxeTreeNode newTreeNode = CreateTreeNode(r.ID, r.Name, r.DisplayName, r.FullPath);
					nodes.Add(newTreeNode);
				}
			}
		}

		private static void PrepareTreeNodeRecursively(SCSimpleObject parentOrg, DeluxeTreeNode parentTreeNode, Dictionary<string, SCSimpleObject> parentsList, DateTime timePoint, string selectedNodeHint)
		{
			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(PC.SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(), new string[] { parentOrg.ID }, false, true, false, timePoint);

			BindObjectsToTreeNodes(relations, parentTreeNode.Nodes);

			foreach (DeluxeTreeNode childNode in parentTreeNode.Nodes)
			{
				SCSimpleObject obj = null;
				if (childNode.Value == selectedNodeHint)
					childNode.Checked = childNode.Selected = true;

				if (parentsList.TryGetValue(childNode.Value, out obj))
				{
					childNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
					childNode.Expanded = true;

					PrepareTreeNodeRecursively(obj, childNode, parentsList, timePoint, selectedNodeHint);
				}
			}
		}

		private void InitializeTreeProperties()
		{
			this.tree.NodeOpenImg = ControlResources.OULogoUrl;
		}
	}
}