using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using System.Reflection;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:PlanCompleteTime runat=server></{0}:PlanCompleteTime>")]
	public class PlanCompleteTime : WebControl
	{
		[DefaultValue("")]
		public string Title
		{
			get
			{
				return this.ViewState.GetViewStateValue("Title", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("Title", value);
			}
		}
		[DefaultValue("")]
		public DateTime? CompleteTime
		{
			get
			{
				DateTime? result = this.ViewState.GetViewStateValue("completeTime", (DateTime?)null);

				if (result.HasValue == false)
				{
					IWfActivity currentActivity = GetDefaultActivity();

					if (currentActivity != null)
						result = currentActivity.Descriptor.EstimateEndTime;
				}

				return result;
			}
			set
			{
				this.ViewState.SetViewStateValue("completeTime", value);
			}

		}
		[DefaultValue("")]
		public string HeaderClassName
		{
			get
			{
				return this.ViewState.GetViewStateValue("HeaderClassName", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("HeaderClassName", value);
			}
		}
		[DefaultValue("")]
		public string ContentClassName
		{
			get
			{
				return this.ViewState.GetViewStateValue("HeaderClassName", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("HeaderClassName", value);
			}
		}

		public PlanCompleteTime()
			: base(HtmlTextWriterTag.Div)
		{ }
		protected override void CreateChildControls()
		{
			this.Controls.Add(GetTable());
			base.CreateChildControls();
		}
		protected override void OnPreRender(EventArgs e)
		{
			RegisterCSS();
		}

		private HtmlTable GetTable()
		{
			HtmlTable tbl = new HtmlTable();
			HtmlTableRow headerRow = new HtmlTableRow();
			HtmlTableCell headerCell = new HtmlTableCell();

			headerCell.Attributes["class"] = string.IsNullOrEmpty(HeaderClassName) ? "title" : HeaderClassName;
			headerCell.InnerText = Title;

			HtmlTableRow contentRow = new HtmlTableRow();
			HtmlTableCell contentCell = new HtmlTableCell();
			contentCell.Attributes["class"] = string.IsNullOrEmpty(ContentClassName) ? "time" : HeaderClassName;

            if (CompleteTime.HasValue && CompleteTime.Value != DateTime.MinValue)
            {
                contentCell.InnerText = CompleteTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                contentCell.InnerHtml = "&nbsp;";
            }
			headerRow.Controls.Add(headerCell);
			contentRow.Controls.Add(contentCell);

			tbl.Controls.Add(headerRow);
			tbl.Controls.Add(contentRow);
			return tbl;
		}

		private static IWfActivity GetDefaultActivity()
		{
			IWfActivity result = null;

			if (WfClientContext.Current.CurrentActivity != null)
				result = WfClientContext.Current.CurrentActivity.ApprovalRootActivity;

			return result;
		}


		private void RegisterCSS()
		{
			string css = ResourceHelper.LoadStringFromResource(
				Assembly.GetExecutingAssembly(),
				"MCS.Web.WebControls.PlanCompleteTime.PlanCompleteTime.css");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StatusCSS",
				string.Format("<style type='text/css'>{0}</style>", css));
		}

	}
}
