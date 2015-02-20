#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	DialogUploadFileProcessControl.cs
// Remark	��	�ϴ��ļ��ȴ�ҳ��
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070929		����
// -------------------------------------------------
#endregion

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogUploadFileProcessControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Images.circle.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// �����ϴ��ȴ���ҳ��
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(MaterialScript), 4)]
	[ClientScriptResource("MCS.Web.WebControls.DialogUploadFileProcessControl",
		"MCS.Web.WebControls.MaterialControl.DialogUploadFileProcessControl.js")]
	[ParseChildren(true), PersistChildren(true)]
	internal class DialogUploadFileProcessControl : ScriptControlBase, INamingContainer
	{
        private ComponentHelperWrapper componentHelperWrapper = new ComponentHelperWrapper();

		public DialogUploadFileProcessControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Span)
		{
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

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.Controls.Add(this.componentHelperWrapper);
            base.CreateChildControls();
        }

		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�����ϴ��ļ�......");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "ȡ��(C)");
		    PreloadImage(this.CircleImagePath, this.CircleImagePath);
			base.OnPreRender(e);
		}

        private void PreloadImage(string key, string imgSrc)
        {
            if (string.IsNullOrEmpty(imgSrc) == false)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key,
                    string.Format("<img src=\"{0}\" style=\"display:none\"/>", imgSrc));
            }
        }
	}
}
