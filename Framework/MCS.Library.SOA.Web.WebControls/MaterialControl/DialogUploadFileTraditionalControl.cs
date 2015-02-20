#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	DialogUploadFileTraditionalControl.cs
// Remark	��	�ϴ��ļ�
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

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogUploadFileTraditionalControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.MaterialControl.HBWebHelperControl.CAB", "Flash Shockwave-Dateien")]
[assembly: WebResource("MCS.Web.WebControls.Images.uploadFile.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.circle.gif", "image/gif")]

namespace MCS.Web.WebControls
{

	/// <summary>
	/// ��ʾ�����ϴ���ҳ��
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(MaterialScript), 4)]
	[ClientScriptResource("MCS.Web.WebControls.DialogUploadFileTraditionalControl",
		"MCS.Web.WebControls.MaterialControl.DialogUploadFileTraditionalControl.js")]
	[ParseChildren(true), PersistChildren(true),]
	internal class DialogUploadFileTraditionalControl : ScriptControlBase, INamingContainer
	{
		public DialogUploadFileTraditionalControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Span)
		{
		}

		[ScriptControlProperty, ClientPropertyName("uploadFileImage"), Browsable(false)]
		private string FolderImagePath
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogUploadFileTraditionalControl),
					"MCS.Web.WebControls.Images.uploadFile.gif");
			}
		}

        [ScriptControlProperty, ClientPropertyName("circleImagePath"), Browsable(false)]
        private string CircleImagePath
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DialogUploadFileProcessControl),
                    "MCS.Web.WebControls.Images.circle.gif");
            }
        }

        /// <summary>
        /// �ؼ�ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("controlID")]
        private string ControlID
        {
            get
            {
                return this.ClientID;
            }
        }

		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȷ��(O)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȡ��(C)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�ϴ��ļ�");
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
