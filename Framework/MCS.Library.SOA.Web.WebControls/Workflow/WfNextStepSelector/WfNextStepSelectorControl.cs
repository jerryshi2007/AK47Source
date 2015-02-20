using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 流程的下一步选择控件
	/// </summary>
	[ToolboxData("<{0}:WfNextStepSelectorControl runat=server></{0}:WfNextStepSelectorControl>")]
	public class WfNextStepSelectorControl : Control, INamingContainer
	{
		private WfControlNextStep selectedNextStep = null;

		public WfNextStepSelectorControl()
		{
			if (this.DesignMode == false)
			{
				JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
				JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
				JSONSerializerExecute.RegisterConverter(typeof(WfControlNextStepConverter));
			}
		}

		public WfControlNextStep SelectedNextStep
		{
			get
			{
				if (this.selectedNextStep == null)
				{
					string nextStepIndexString = HttpContext.Current.Request.Form[this.ClientID + "_nextSteps"];

					if (string.IsNullOrEmpty(nextStepIndexString) == false)
					{
						int nextStepIndex = int.Parse(nextStepIndexString);

						WfControlNextStepCollection nextSteps = GetNextSteps(this.ActivityDescriptor);

						this.selectedNextStep = nextSteps[nextStepIndex];
					}
				}

				return this.selectedNextStep;
			}
		}

		public WfControlNextStepCollection NextSteps
		{
			get
			{
				return GetNextSteps(this.ActivityDescriptor);
			}
		}

		[Browsable(true)]
		public IWfActivityDescriptor ActivityDescriptor
		{
			get;
			set;
		}

		public bool Enabled
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "Enabled", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Enabled", value);
			}
		}

		public string OnChangeClientScript
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "OnChangeClientScript", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "OnChangeClientScript", value);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (DesignMode == false)
			{
				ExceptionHelper.FalseThrow(WfMoveToControl.Current != null,
					"WfNextStepSelectorControl控件必须和WfMoveToControl一起使用");

				RenderNextStepSelector();

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
					"WfNextStepSelectorControl",
					ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.Workflow.WfNextStepSelector.WfNextStepSelectorControl.js"),
					true);
			}

			base.OnPreRender(e);
		}

		private void RenderNextStepSelector()
		{
			WfControlNextStepCollection nextSteps = GetNextSteps(this.ActivityDescriptor);

			RenderNextSteps(nextSteps);
			RenderNextStepsInfo(nextSteps);
		}

		private void RenderNextStepsInfo(WfControlNextStepCollection nextSteps)
		{
			if (Enabled)
			{
				WfMoveToControl.DoActionAfterRegisterContextConverter(() =>
					Page.ClientScript.RegisterHiddenField(
						this.ClientID + "_data",
						JSONSerializerExecute.Serialize(nextSteps))
				);
			}
		}

		private void RenderNextSteps(WfControlNextStepCollection nextSteps)
		{
			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Style[HtmlTextWriterStyle.MarginBottom] = "5px";

			Controls.Add(div);

			if (nextSteps.Count > 1)
			{
				int defaultStepIndex = FindDefaultSelectStep(nextSteps);

				for (int i = 0; i < nextSteps.Count; i++)
					RenderOneRadio(nextSteps[i], i, defaultStepIndex, div);
			}
		}

		private int FindDefaultSelectStep(WfControlNextStepCollection nextSteps)
		{
			int result = 0;

			for (int i = 0; i < nextSteps.Count; i++)
			{
				WfControlNextStep nextStep = nextSteps[i];

				if (nextStep.TransitionDescriptor.DefaultSelect)
				{
					result = i;
					break;
				}
			}

			return result;
		}

		private void RenderOneRadio(WfControlNextStep nextStep, int i, int defaultStepIndex, Control container)
		{
			string script = this.OnChangeClientScript;

			if (string.IsNullOrEmpty(script))
			{
				script = "onWFNextStepChange();";
			}
			else
			{
				string innerScript = "var radioGroupName = event.srcElement.name;var radioBtns = document.getElementsByName(radioGroupName);for(var i=0;i<radioBtns.length;i++){radioBtns[i].disabled = true;}";
				script += innerScript;
			}
			string tranRadioValue = !string.IsNullOrEmpty(nextStep.TransitionDescriptor.Name) ? nextStep.TransitionDescriptor.Name : nextStep.TransitionDescriptor.Description;
			string actRadioValue = !string.IsNullOrEmpty(nextStep.ActivityDescriptor.Name) ? nextStep.ActivityDescriptor.Name : nextStep.ActivityDescriptor.Description;
			string html = string.Format("<input type=\"radio\" name=\"{0}_nextSteps\" style=\"cursor: hand\" value=\"{1}\" id=\"{2}\" {4} onclick=\"{6}\" {5} dataID=\"{0}_data\"/><label style=\"cursor: hand;font-family:'宋体',Arial,Helvetica,sans-serif;margin-right:4px;font-size: 12px;font-weight:bold;\" for=\"{2}\" {5}>{3}</label>",
				this.ClientID,
				i,
				this.ClientID + "_nextStepRadio_" + i,
				HttpUtility.HtmlEncode(!string.IsNullOrEmpty(tranRadioValue) ? tranRadioValue : actRadioValue),
				i == defaultStepIndex ? "checked" : string.Empty,
				Enabled ? string.Empty : "disabled",
				script);

			container.Controls.Add(new LiteralControl(html));
		}

		internal static WfControlNextStepCollection GetNextSteps(IWfActivityDescriptor actDesp)
		{
			if (actDesp == null && WfClientContext.Current.OriginalActivity != null)
				actDesp = WfClientContext.Current.CurrentActivity.Descriptor;

			WfControlNextStepCollection result = null;

			if (actDesp != null)
				result = new WfControlNextStepCollection(actDesp.ToTransitions.GetAllCanTransitTransitions(false));
			else
				result = new WfControlNextStepCollection();

			return result;
		}
	}
}
