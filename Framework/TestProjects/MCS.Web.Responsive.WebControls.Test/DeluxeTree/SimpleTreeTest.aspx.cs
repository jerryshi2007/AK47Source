using System;
using System.Data;
using System.Threading;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Web.Responsive.Library.Script;
using MCS.Web.Responsive.WebControls;

namespace MCS.Web.Responsive.WebControls.Test.DeluxeTree
{
	public partial class SimpleTreeTest : System.Web.UI.Page
	{
		private class ExtendedData
		{
			public string Data = "Hello world";
		}

		protected override void OnInit(EventArgs e)
		{
			StaticCallBackProxy.Instance.TargetControlLoaded += new StaticCallBackProxyControlLoadedEventHandler(Instance_TargetControlLoaded);
			base.OnInit(e);
		}

		private void Instance_TargetControlLoaded(Control targetControl)
		{
			MCS.Web.Responsive.WebControls.DeluxeTree tree = targetControl as MCS.Web.Responsive.WebControls.DeluxeTree;

			if (tree != null)
			{
                tree.GetChildrenData += new MCS.Web.Responsive.WebControls.DeluxeTree.GetChildrenDataDelegate(tree_GetChildrenData);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void tree_GetChildrenData(DeluxeTreeNode parentNode, DeluxeTreeNodeCollection result, string callBackContext)
		{
			if (parentNode.Text == "加载子节点会出现异常")
				throw new System.ApplicationException("加载子节点出现异常");
			else
				if (parentNode.Text == "很多子节点，小心打开！")
					CreateSubTreeNodes(result, 300);
				else
				{
					Thread.Sleep(1000);
					Random rnd = new Random();
					CreateSubTreeNodes(result, rnd.Next(5));
				}
		}

		private static void CreateSubTreeNodes(DeluxeTreeNodeCollection parent, int subNodesCount)
		{
			for (int i = 0; i < subNodesCount; i++)
			{
				string text = "动态子节点" + i;
				DeluxeTreeNode node = new DeluxeTreeNode(text, text);

				node.ExtendedData = new ExtendedData();
				node.ShowCheckBox = true;

				if (i != 1)
					node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;

				parent.Add(node);

				if (i == 1)
				{
					DeluxeTreeNode subNode = new DeluxeTreeNode("固定孙节点", text);
					node.Nodes.Add(subNode);
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			dynamicTreeContainer.Controls.Add(CreateDynamicTree());

			base.OnPreRender(e);
		}

		private MCS.Web.Responsive.WebControls.DeluxeTree CreateDynamicTree()
		{
            MCS.Web.Responsive.WebControls.DeluxeTree tree = new MCS.Web.Responsive.WebControls.DeluxeTree();

			tree.Width = Unit.Pixel(200);
			tree.Height = Unit.Pixel(300);
			tree.BorderStyle = BorderStyle.Solid;
			tree.BorderWidth = Unit.Pixel(1);
			tree.NodeCloseImg = "closeImg.gif";
			tree.NodeOpenImg = "openImg.gif";
            tree.Style["overflow"] = "auto";
            tree.Style["padding-left"] = "5px";
            //tree.GetChildrenData += new WebControls.DeluxeTree.GetChildrenDataDelegete(tree_GetChildrenData);

			DeluxeTreeNode first = new DeluxeTreeNode("First Node", "FirstNode");
			first.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			tree.Nodes.Add(first);

			DeluxeTreeNode second = new DeluxeTreeNode("Second Node", "SecondNode");
			second.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			tree.Nodes.Add(second);

			return tree;
		}
	}
}
