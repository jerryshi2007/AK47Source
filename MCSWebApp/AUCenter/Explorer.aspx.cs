using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	public partial class Explorer : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (string.IsNullOrEmpty(Request.QueryString["id"]))
				throw new InvalidOperationException("必须提供id");
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			this.tree.NodeOpenImg = ControlResources.OULogoUrl;

			var cate = AU.Adapters.SchemaCategoryAdapter.Instance.LoadByID(Request.QueryString["id"]);
			if (cate == null || cate.Status != MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
			{
				Response.Redirect("~/Default.aspx", true);
			}
			else
			{

				var subCate = AU.Adapters.SchemaCategoryAdapter.Instance.LoadSubCategories(cate.ID);

				cateName.InnerText = cate.Name;

				foreach (AU.AUSchemaCategory item in subCate)
				{
					DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.Name, item.Name);
					node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
					node.Expanded = false;
					node.ExtendedData = "category";
					this.tree.Nodes.Add(node);
				}

				var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaByCategory(cate.ID, true, DateTime.MinValue);

				foreach (AU.AUSchema item in subUnits)
				{
					DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.DisplayName, item.Name);
					node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
					node.NodeOpenImg = "Images/blocks.png";
					node.NodeCloseImg = "Images/blocks.png";
					node.ExtendedData = "schema";
					node.Expanded = false;
					this.tree.Nodes.Add(node);
				}
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
			if (parentNode.ExtendedData.ToString() == "category")
			{
				var subCates = AU.Adapters.SchemaCategoryAdapter.Instance.LoadSubCategories(parentNode.Value);

				foreach (var item in subCates)
				{
					DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.Name, item.Name);
					node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
					node.ExtendedData = "category";
					node.Expanded = false;
					result.Add(node);
				}
			}

			var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaByCategory(parentNode.Value, true, DateTime.MinValue);

			foreach (AU.AUSchema item in subUnits)
			{
				DeluxeTreeNode node = CreateTreeNode(item.ID, item.Name, item.DisplayName, item.Name);
				node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
				node.NodeOpenImg = "Images/blocks.png";
				node.NodeCloseImg = "Images/blocks.png";
				node.ExtendedData = "schema";
				node.Expanded = false;
				result.Add(node);
			}
		}
	}
}