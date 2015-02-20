using System;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Web.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	public partial class LogList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			var settings = LogCategoryConfigSection.GetConfig();

			if (settings == null)
				throw new System.Configuration.ConfigurationErrorsException("未找到日志类别配置");

			var nodeGenerl = new DeluxeTreeNode("用户操作", "G")
			{
				Expanded = true
			};

			this.tree.Nodes.Add(nodeGenerl);

			System.Collections.Specialized.HybridDictionary dic = new System.Collections.Specialized.HybridDictionary();

			foreach (LogCategoryConfigurationElement item in settings.Categories)
			{
				var node = new DeluxeTreeNode(item.Title, nodeGenerl.Value + "." + item.Name);
				node.Expanded = false;
				nodeGenerl.Nodes.Add(node);
				dic.Add(item.Name, node);
			}

			var categoris = LogCategoryAdapter.Instance.LoadCategories();

			foreach (var item in categoris)
			{
				if (dic.Contains(item.Category))
				{
					var nodeParent = (DeluxeTreeNode)dic[item.Category];

					var node = new DeluxeTreeNode(item.Description, nodeParent.Value + "." + item.OperationType);
					nodeParent.Nodes.Add(node);
				}
			}
		}

		protected void ShuttleClick(object sender, EventArgs e)
		{
			if (this.shuttlePoint.Value.Length > 0)
			{
				long t = long.Parse(this.shuttlePoint.Value);
				DateTime dt = new DateTime(t);
				AUCenter.WebControls.Banner.ChangeAndSaveTimePoint(dt);
			}
		}

		private static DeluxeTreeNode CreateTreeNode(SchemaObjectBase obj)
		{
			string name = obj.Properties.GetValue("DisplayName", string.Empty);

			if (string.IsNullOrEmpty(name))
				name = obj.Properties.GetValue("Name", string.Empty);

			DeluxeTreeNode node = new DeluxeTreeNode(name, obj.ID);

			node.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;
			node.ToolTip = obj.Properties.GetValue("description", string.Empty);
			node.NodeOpenImg = ControlResources.OULogoUrl;
			node.NodeCloseImg = ControlResources.OULogoUrl;

			return node;
		}
	}
}