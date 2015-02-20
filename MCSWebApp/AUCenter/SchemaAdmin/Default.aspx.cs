using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter.SchemaAdmin
{
	public partial class Default : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			this.tree.NodeOpenImg = ControlResources.OULogoUrl;

			var topCates = AU.Adapters.SchemaCategoryAdapter.Instance.LoadSubCategories(null);

			foreach (AU.AUSchemaCategory item in topCates)
			{
				DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.Name, item.Name);
				node.Expanded = false;
				this.tree.Nodes.Add(node);
			}
		}

		private static DeluxeTreeNode CreateTreeNode(string id, string name, string displayName, string fullPath)
		{
			DeluxeTreeNode node = new DeluxeTreeNode(string.IsNullOrWhiteSpace(displayName) ? name : displayName, id);

			node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			node.ToolTip = fullPath;
			node.NodeOpenImg = ControlResources.OULogoUrl;
			node.NodeCloseImg = ControlResources.OULogoUrl;
			node.CssClass = "au-catenode";

			return node;
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			var subCates = AU.Adapters.SchemaCategoryAdapter.Instance.LoadSubCategories(parentNode.Value);

			foreach (var item in subCates)
			{
				DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.Name, item.Name);
				node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
				node.Expanded = true;
				result.Add(node);
			}
		}
	}
}