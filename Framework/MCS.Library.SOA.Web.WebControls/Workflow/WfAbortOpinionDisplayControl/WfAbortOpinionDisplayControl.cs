using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using System.Web;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	[ToolboxData("<{0}:WfAbortOpinionDisplayControl runat=server></{0}:WfAbortOpinionDisplayControl>")]
	public class WfAbortOpinionDisplayControl : WebControl
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (Page.IsCallback == false && 
				WfClientContext.Current.OriginalActivity != null &&
				(WfClientContext.Current.OriginalActivity.Process.Status == WfProcessStatus.Aborted ||
				WfClientContext.Current.OriginalActivity.Process.SameResourceRootProcess.Status == WfProcessStatus.Aborted))
			{
				GenericOpinion opinion =
					GenericOpinionAdapter.Instance.LoadAbortProcessOpinion(WfClientContext.Current.OriginalActivity.Process.ResourceID);
				if (opinion != null)
					CreateOpinionInfo(opinion);
			}

			base.OnPreRender(e);
		}

		private void CreateOpinionInfo(GenericOpinion opinion)
		{
			HtmlTable table = new HtmlTable();

			table.Style["border"] = "1px solid #efae27";
			table.Style[HtmlTextWriterStyle.BackgroundColor] = "#ffffe8";
			table.Width = "100%";

			Controls.Add(table);

			HtmlTableRow row = new HtmlTableRow();

			table.Controls.Add(row);

			HtmlTableCell cellLeft = new HtmlTableCell();

			row.Controls.Add(cellLeft);

			HtmlTableCell cellRight = new HtmlTableCell();

			cellRight.Align = "right";

			row.Controls.Add(cellRight);

			HtmlImage logo = new HtmlImage();

			logo.Src = ControlResources.CancelledLogoUrl;
			logo.Align = "absmiddle";
			logo.Alt = "流程被作废";

			cellLeft.Controls.Add(logo);

			HtmlGenericControl span = new HtmlGenericControl("span");

			span.InnerHtml = HttpUtility.HtmlEncode(opinion.Content).Replace("\n", "<br/>");

			cellLeft.Controls.Add(span);

			cellRight.InnerText = opinion.IssuePerson.DisplayName + " " +
				string.Format("{0:yyyy-MM-dd HH:mm:ss}", opinion.IssueDatetime);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("WfAbortOpinionDisplayControl");
			else
				base.Render(writer);
		}
	}
}
