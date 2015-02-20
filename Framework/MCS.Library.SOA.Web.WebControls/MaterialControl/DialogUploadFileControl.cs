#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	DialogUploadFileControl.cs
// Remark	：	批量上传文件
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070810		创建
// -------------------------------------------------
#endregion

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogUploadFileControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.MaterialControl.HBWebHelperControl.CAB", "Flash Shockwave-Dateien")]
[assembly: WebResource("MCS.Web.WebControls.Images.uploadFile.gif", "image/gif")]

namespace MCS.Web.WebControls
{

	/// <summary>
	/// 显示附件上传的页面
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(MaterialScript), 4)]
	[ClientScriptResource("MCS.Web.WebControls.DialogUploadFileControl",
		"MCS.Web.WebControls.MaterialControl.DialogUploadFileControl.js")]
	[ParseChildren(true), PersistChildren(true),]
	internal class DialogUploadFileControl : ScriptControlBase, INamingContainer
	{
        private DialogHelperWrapper dialogHelperWrapper = new DialogHelperWrapper();
        private ComponentHelperWrapper componentHelperWrapper = new ComponentHelperWrapper();

		public DialogUploadFileControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Span)
		{
		}

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.Controls.Add(this.dialogHelperWrapper);
            this.Controls.Add(this.componentHelperWrapper);
            base.CreateChildControls();
        }

		[ScriptControlProperty, ClientPropertyName("uploadFileImage"), Browsable(false)]
		private string FolderImagePath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogUploadFileControl),
					"MCS.Web.WebControls.Images.uploadFile.gif");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确定(O)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "取消(C)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "批量上传文件");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "已选择文件列表");
			
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "选择文件(S)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "移除文件(R)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "上传文件(U)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "已上传文件列表");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "文件名");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "备注");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "序号");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "删除文件(D)");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请选择要删除的文件!");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认删除所选的文件吗？");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "单击此处编辑");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "回车保存，ESC取消");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "您选择的文件路径长度超过了限制，请尝试减少选择文件的数量");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "至少要选择一个上传文件！");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "您还有未上传的文件，确定要关闭本窗口吗？");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "每个上传文件的尺寸必须小于{0}字节");
			
			base.OnPreRender(e);
		}
	}
}
