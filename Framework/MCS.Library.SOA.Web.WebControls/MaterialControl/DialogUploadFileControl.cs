#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	DialogUploadFileControl.cs
// Remark	��	�����ϴ��ļ�
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070810		����
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
	/// ��ʾ�����ϴ���ҳ��
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
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȷ��(O)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȡ��(C)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�����ϴ��ļ�");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��ѡ���ļ��б�");
			
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ѡ���ļ�(S)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�Ƴ��ļ�(R)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�ϴ��ļ�(U)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "���ϴ��ļ��б�");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�ļ���");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��ע");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "���");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ɾ���ļ�(D)");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��ѡ��Ҫɾ�����ļ�!");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȷ��ɾ����ѡ���ļ���");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�����˴��༭");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�س����棬ESCȡ��");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��ѡ����ļ�·�����ȳ��������ƣ��볢�Լ���ѡ���ļ�������");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "����Ҫѡ��һ���ϴ��ļ���");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "������δ�ϴ����ļ���ȷ��Ҫ�رձ�������");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ÿ���ϴ��ļ��ĳߴ����С��{0}�ֽ�");
			
			base.OnPreRender(e);
		}
	}
}
