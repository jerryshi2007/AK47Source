using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using AUClient = MCS.Library.SOA.DataObjects.Security.AUClient;
using MCS.Web.WebControls;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner.ModalDialog
{
	public partial class WfAUSchemaRoleEditor : System.Web.UI.Page
	{
		private static string blocksImgUri = null;
		private static string folderImgUri = null;
		private static string roleImgUri = null;
		private static bool imgUrlRetrived = false;

		public static string BlocksImgUri
		{
			get
			{
				RetriveImgUrls();
				return blocksImgUri;
			}
		}

		private static void RetriveImgUrls()
		{
			if (imgUrlRetrived == false)
			{
				blocksImgUri = HttpContext.Current.Response.ApplyAppPathModifier("~/images/blocks.png");
				roleImgUri = HttpContext.Current.Response.ApplyAppPathModifier("~/images/role16.gif"); ;
				folderImgUri = ControlResources.OULogoUrl;
			}
		}

		public static string FolderImgUri
		{
			get
			{
				RetriveImgUrls();
				return folderImgUri;
			}
		}

		public static string RoleImgUri
		{
			get
			{
				RetriveImgUrls();
				return roleImgUri;
			}
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			WfConverterHelper.RegisterConverters();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			var cates = AUClient.ServiceBroker.AUCenterQueryService.Instance.GetSubCategories(null, true);
			foreach (var item in cates)
			{
				this.tree.Nodes.Add(CreateNode(item));
			}
		}

		private MCS.Web.WebControls.DeluxeTreeNode CreateNode(AUClient.ClientAUSchemaCategory item)
		{
			return new MCS.Web.WebControls.DeluxeTreeNode()
			{
				ChildNodesLoadingType = MCS.Web.WebControls.ChildNodesLoadingTypeDefine.LazyLoading,
				Text = item.Name,
				Value = item.ID,
				ExtendedData = "Category",
				NodeOpenImg = FolderImgUri,
				NodeCloseImg = FolderImgUri
			};
		}

		private MCS.Web.WebControls.DeluxeTreeNode CreateNode(AUClient.ClientAUSchema item)
		{
			return new MCS.Web.WebControls.DeluxeTreeNode()
			{
				ChildNodesLoadingType = MCS.Web.WebControls.ChildNodesLoadingTypeDefine.LazyLoading,
				Text = item.Name,
				Value = item.ID,
				ExtendedData = "AUSchema",
				NodeOpenImg = BlocksImgUri,
				NodeCloseImg = BlocksImgUri
			};
		}


		private MCS.Web.WebControls.DeluxeTreeNode CreateNode(AUClient.ClientAUSchemaRole item)
		{
			return new MCS.Web.WebControls.DeluxeTreeNode()
			{
				ChildNodesLoadingType = MCS.Web.WebControls.ChildNodesLoadingTypeDefine.Normal,
				Text = item.Name,
				Value = item.ID,
				ExtendedData = JSONSerializerExecute.Serialize(new WfAURoleResourceDescriptor(CreateWrappedRole(item))),
				NodeOpenImg = RoleImgUri,
				NodeCloseImg = RoleImgUri,
				ExtendedDataKey = "AUSchemaRoles"
			};
		}

		private WrappedAUSchemaRole CreateWrappedRole(AUClient.ClientAUSchemaRole item)
		{
			return new WrappedAUSchemaRole(item.ID) { Description = item.Name };
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			switch ((string)parentNode.ExtendedData)
			{
				case "Category":
					{
						var subCates = AUClient.ServiceBroker.AUCenterQueryService.Instance.GetSubCategories(parentNode.Value, true);
						foreach (var item in subCates)
							result.Add(CreateNode(item));

						var subSchemas = AUClient.ServiceBroker.AUCenterQueryService.Instance.GetAUSchemaByCategory(parentNode.Value, true);
						foreach (var item in subSchemas)
							result.Add(CreateNode(item));
					}
					break;
				case "AUSchema":
					{
						var schemaRoles = AUClient.ServiceBroker.AUCenterQueryService.Instance.GetMembers(parentNode.Value, new string[] { "AUSchemaRoles" }, true);
						foreach (AUClient.ClientAUSchemaRole item in schemaRoles)
							result.Add(CreateNode(item));
					}
					break;
				default:
					break;
			}
		}
	}
}