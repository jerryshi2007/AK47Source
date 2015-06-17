#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	DialogMaterialVersionControl.cs
// Remark	��	�ļ��汾
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070820		����
// -------------------------------------------------
#endregion

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.ComponentModel;

using MCS.Web.Library;
using MCS.Web.Library.Script;

using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogMaterialVersionControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Images.closeImg.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.openImg.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.fileVersion.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// ��ʾ�����汾��ҳ��
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(MaterialScript), 3)]
	[ClientScriptResource("MCS.Web.WebControls.DialogMaterialVersionControl",
	   "MCS.Web.WebControls.MaterialControl.DialogMaterialVersionControl.js")]
	[ParseChildren(true), PersistChildren(true),]
    //[Obsolete("����ķ���������")]
	internal class DialogMaterialVersionControl : ScriptControlBase, INamingContainer
	{
		DeluxeTree treeControl = null;
		string userID = string.Empty;

		public DialogMaterialVersionControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Span)
		{
		}

		/// <summary>
		/// ���Ŀͻ���ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("treeControlID")]
		public string TreeControlClientID
		{
			get
			{
				return this.treeControl.ClientID;
			}
		}

		/// <summary>
		/// �ļ��汾ͼ��·��
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("fileVersionPath")]
		public string FileVersionPath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(
					typeof(MCS.Web.WebControls.DialogMaterialVersionControl),
					"MCS.Web.WebControls.Images.fileVersion.gif");
			}
		}

		private string CurrentPageUrl
		{
			get
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				return page.ResolveUrl(page.AppRelativeVirtualPath);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.userID = WebUtility.GetRequestQueryValue("userID", string.Empty);

			this.EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			if (!this.ChildControlsCreated)
			{
				Controls.Clear();

				this.treeControl = new DeluxeTree();
				this.treeControl.ID = "innerTree";
				//this.InitTree();

				this.Controls.Add(this.treeControl);
				this.ChildControlsCreated = true;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.InitTree();

			base.OnPreRender(e);
		}

		private void InitTree()
		{
			string materialID = HttpUtility.UrlEncode(WebUtility.GetRequestQueryValue<string>("materialID", string.Empty));

			if (materialID != string.Empty)
			{
				MaterialTreeNode rootNode = MaterialAdapter.Instance.LoadMaterialVersionsByMaterialID(materialID);

				if (rootNode == null)
					return;

				DeluxeTreeNode deluxeTreeNode = this.GenerateDeluxeTreeNode(rootNode);

				if (deluxeTreeNode != null)
				{
					this.treeControl.Visible = true;
					this.treeControl.Nodes.Add(deluxeTreeNode);
				}
			}
		}

		/// <summary>
		/// ��ʼ��ĳ���ڵ�
		/// </summary>
		/// <param name="node">�ڵ�</param>
		/// <param name="childNodes">�ӽڵ�</param>
		private DeluxeTreeNode GenerateDeluxeTreeNode(MaterialTreeNode materialNode)
		{
			DeluxeTreeNode rootNode = new DeluxeTreeNode(materialNode.Material.Title, materialNode.Material.ID);

			this.InitNode(rootNode, materialNode);

			foreach (MaterialTreeNode node in materialNode.Children)
			{
				rootNode.Nodes.Add(GenerateDeluxeTreeNode(node));
			}

			return rootNode;
		}

		/// <summary>
		/// ����TreeNode������
		/// </summary>
		/// <param name="node">TreeNode</param>
		/// <param name="materialNode">MaterialTreeNode</param>
		private void InitNode(DeluxeTreeNode node, MaterialTreeNode materialNode)
		{
			node.ToolTip = "������ļ�";
			node.Expanded = true;
			node.SelectedCssClass = " ";
			node.Html = string.Format("[{0}]<a href=\"{1}?requestType=download&rootPathName={2}&controlID={3}&fileName={4}&pathType={5}&filePath={6}&fileReadonly=true&userID={7}\" target=\"_blank\" >{8}</a>",
				(materialNode.Material.Department != null) ? materialNode.Material.Department.DisplayName : string.Empty,
				this.CurrentPageUrl,
				WebUtility.GetRequestQueryValue("rootPathName", string.Empty),
				WebUtility.GetRequestQueryValue("controlID", string.Empty),
				materialNode.Material.OriginalName,
				Convert.ToInt16(PathType.relative),
				HttpUtility.UrlEncode(materialNode.Material.RelativeFilePath),
				this.userID,
				HttpUtility.HtmlEncode(materialNode.Material.Title));

			if (!string.IsNullOrEmpty(materialNode.Material.RelativeFilePath))
			{
				string fileName = Path.GetFileName(materialNode.Material.RelativeFilePath);

				string fileIconPath = FileConfigHelper.GetFileIconPath(fileName);

				if (fileIconPath != string.Empty)
				{
					node.NodeCloseImg = fileIconPath;
					node.NodeOpenImg = fileIconPath;
				}
			}
		}
	}
}
