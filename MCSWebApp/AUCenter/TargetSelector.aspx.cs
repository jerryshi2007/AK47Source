using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Web.WebControls;

namespace AUCenter
{
	public partial class TargetSelector : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				string schemaID = Request.QueryString["schemaId"];
				if (string.IsNullOrEmpty(schemaID))
					throw new HttpException("必须提供schemaId");
				this.hfSchemaID.Value = schemaID;
				this.hfParentID.Value = Request.QueryString["parentID"];

				var auSchema = DbUtil.GetEffectiveObject<AU.AUSchema>(schemaID);
				var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadSubUnits(schemaID, schemaID, true, DateTime.MinValue);

				this.schemaLabel.InnerText = auSchema.GetQualifiedName();

				string[] excludes = this.Request.QueryString.GetValues("exclude");

				this.tree.CallBackContext = schemaID;

				var rootNode = new MCS.Web.WebControls.DeluxeTreeNode(string.IsNullOrEmpty(auSchema.DisplayName) ? auSchema.Name : auSchema.DisplayName, auSchema.ID)
				{
					NodeOpenImg = ControlResources.OULogoUrl,
					NodeCloseImg = ControlResources.OULogoUrl,
					CssClass = "au-catenode",
					ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal,
					ExtendedData = "AUSchema",
					Expanded = true
				};

				tree.Nodes.Add(rootNode);

				foreach (AU.AdminUnit item in subUnits)
				{
					if (IsInExclude(item.ID, excludes) == false)
					{
						AddUnitToTree(item, rootNode.Nodes);
					}
				}
			}
		}

		private bool IsInExclude(string key, string[] keys)
		{
			bool result = false;
			if (keys != null)
			{
				for (int i = keys.Length - 1; i >= 0; i--)
				{
					if (keys[i] == key)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		private void AddUnitToTree(AU.AdminUnit item, MCS.Web.WebControls.DeluxeTreeNodeCollection treeNodes)
		{
			treeNodes.Add(new MCS.Web.WebControls.DeluxeTreeNode(item.Name, item.ID)
			{
				NodeOpenImg = ControlResources.OULogoUrl,
				NodeCloseImg = ControlResources.OULogoUrl,
				CssClass = "au-catenode",
				ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading,
				ExtendedData = "AU"
			});
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			string[] excludes = this.Request.QueryString.GetValues("exclude");
			var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadSubUnits(callBackContext, parentNode.Value, true, DateTime.MinValue);

			foreach (AU.AdminUnit item in subUnits)
			{
				if (IsInExclude(item.ID, excludes) == false)
				{
					AddUnitToTree(item, result);
				}
			}
		}
	}
}