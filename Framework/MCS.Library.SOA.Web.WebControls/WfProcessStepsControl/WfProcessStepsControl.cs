using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using System.Web.UI.HtmlControls;

[assembly: WebResource("MCS.Web.WebControls.WfProcessStepsControl.flow_steps_bg.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.WfProcessStepsControl.WfProcessStepsControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.WfProcessStepsControl.WfProcessStepsControl.css", "text/css")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 显示流程步骤和状态的控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientCssResource("MCS.Web.WebControls.WfProcessStepsControl.WfProcessStepsControl.css")]
	[ClientScriptResource("MCS.Web.WebControls.WfProcessStepsControl",
		"MCS.Web.WebControls.WfProcessStepsControl.WfProcessStepsControl.js")]
	[ToolboxData("<{0}:WfProcessStepsControl runat=server></{0}:WfProcessStepsControl>")]
	public class WfProcessStepsControl : ScriptControlBase
	{
		public WfProcessStepsControl()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (WfClientContext.Current.OriginalActivity != null)
			{
				HtmlGenericControl ol = new HtmlGenericControl("ol");

				ol.Attributes["class"] = "workflow";

				this.Controls.Add(ol);
				WfClientContext.Current.OriginalActivity.Process.Activities.ForEach(act => RenderActivity(ol, act));
			}
			base.OnPreRender(e);
		}

		private void RenderActivity(Control container, IWfActivity activity)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");

			container.Controls.Add(li);

			HtmlGenericControl div = new HtmlGenericControl("div");
			li.Controls.Add(div);

			if (activity.Descriptor.ActivityType == WfActivityType.InitialActivity)
			{
				div.Controls.Add(CreateImageSpan(activity.Status == WfActivityStatus.Running ? "runningLeftStart" : "passedLeftStart"));
			}

			HtmlGenericControl itemDiv = new HtmlGenericControl("div");

			div.Controls.Add(itemDiv);

			itemDiv.Controls.Add(CreateTextSpan(activity));

			string rightImageClass = "notRunningLeft";

			switch (activity.Status)
			{
				case WfActivityStatus.Running:
					itemDiv.Attributes["class"] = "running";

					switch (activity.Descriptor.ActivityType)
					{
						case WfActivityType.InitialActivity:
							rightImageClass = "runningRight";
							break;
						case WfActivityType.CompletedActivity:
							rightImageClass = "runningRightEnd";
							break;
					}
					break;
				case WfActivityStatus.NotRunning:
					itemDiv.Attributes["class"] = "notRunning";

					if (activity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
						rightImageClass = "notRunningFinished";
					break;
				case WfActivityStatus.Completed:
					itemDiv.Attributes["class"] = "passed";

					switch (activity.Descriptor.ActivityType)
					{
						case WfActivityType.InitialActivity:
							rightImageClass = "passedRight";
							break;
						case WfActivityType.CompletedActivity:
							rightImageClass = "passedRightEnd";
							break;
						default:
							rightImageClass = "passedRight";
							break;
					}

					break;
			}

			itemDiv.Controls.Add(CreateImageSpan(rightImageClass));
		}

		private HtmlGenericControl CreateTextSpan(IWfActivity activity)
		{
			HtmlGenericControl span = new HtmlGenericControl("span");

			span.Attributes["class"] = "stepText";

			span.InnerText = activity.Descriptor.Name;

			return span;
		}

		private HtmlGenericControl CreateImageSpan(string imageClass)
		{
			HtmlGenericControl span = new HtmlGenericControl("span");

			span.Attributes["class"] = "imageContainer";

			HtmlImage img = CreateBackgroundImage();

			img.Attributes["class"] = imageClass;
			span.Controls.Add(img);

			return span;
		}

		private HtmlImage CreateBackgroundImage()
		{
			HtmlImage img = new HtmlImage();

			img.Src = BackgroundImage;

			return img;
		}

		private string BackgroundImage
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(this.GetType(),
						"MCS.Web.WebControls.WfProcessStepsControl.flow_steps_bg.gif");
			}
		}
	}
}
