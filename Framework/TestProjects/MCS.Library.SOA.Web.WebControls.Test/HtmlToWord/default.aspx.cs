using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MCS.Web.Library;
using System.IO;

namespace MCS.Library.SOA.Web.WebControls.Test.HtmlToWord
{
	public partial class _default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "openExportWordDocumentUrl");

			pageRenderMode.ContentTypeKey = ResponseContentTypeKey.WORD.ToString();
			pageRenderMode.DispositionType = ResponseDispositionType.Inline;
			pageRenderMode.AttachmentFileName = "exports.doc";
			pageRenderMode.UseNewPage = false;

			exportUrl.Value = WebUtility.GetRequestExecutionUrl(pageRenderMode);

			exportBtn.Visible = WebUtility.GetRequestPageRenderMode().IsHtmlRender;

			base.OnPreRender(e);
		}

		public override void RenderControl(HtmlTextWriter writer)
		{
			PageRenderMode renderMode = WebUtility.GetRequestPageRenderMode();

			if (renderMode.IsHtmlRender == false)
			{
				StringBuilder strB = new StringBuilder();
				StringWriter sw = new StringWriter(strB);

				using (HtmlTextWriter baseWriter = new HtmlTextWriter(sw))
				{
					base.RenderControl(baseWriter);
				}

				renderMode.RenderPageOnlySelf(strB.ToString());
			}
			else
				base.RenderControl(writer);
		}
	}
}