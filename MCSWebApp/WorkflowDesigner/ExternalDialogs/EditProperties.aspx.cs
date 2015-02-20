using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.Globalization;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class EditProperties : ExternalPropertyEditorBase
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.descriptorEditor.PreRenderControl += new EventHandler(descriptorEditor_PreRenderControl);
			this.descriptorEditor.SaveButtonClicked += new EventHandler(descriptorEditor_SaveButtonClicked);
		}

		protected void descriptorEditor_SaveButtonClicked(object sender, EventArgs e)
		{
			EditProcessPropertiesHelper helper = new EditProcessPropertiesHelper(
				this.CurrentProcessDescriptor.ProcessInstance,
				this.CurrentProcessDescriptor,
				this.propertyGrid.Properties, syncMSObj.Checked);

			WfExecutorBase executor = helper.GetExecutor();

			executor.Execute();
		}

		protected void descriptorEditor_PreRenderControl(object sender, EventArgs e)
		{
			ControllerHelper.ExecuteMethodByRequest(this);

			if (this.descriptorEditor.ShowMainStream)
				this.syncMSObj.Disabled = true;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region Controller Methods
		[ControllerMethod]
		protected void EditProcessPropertiesPreRender(string processID)
		{
			diagLogoText.Text = Translator.Translate(Define.DefaultCulture, "编辑流程属性");
			documentTitle.Text = diagLogoText.Text;

			this.descriptorEditor.RegisterAfterProcessDeserializedFunction("WFWeb.BindPropertyGrid(process.Key, process);");
		}

		[ControllerMethod]
		protected void EditActivityPropertiesPreRender(string processID, string activityKey)
		{
			if (this.CurrentProcessDescriptor != null)
			{
				IWfActivityDescriptor actDesp = this.CurrentProcessDescriptor.Activities[activityKey];

				(actDesp != null).FalseThrow("不能在流程中找到Key为{0}的活动", activityKey);

				this.descriptorEditor.RegisterAfterProcessDeserializedFunction(
					string.Format("WFWeb.BindPropertyGrid(process.Key, findActivityByKey(process, \"{0}\"));", activityKey));
			}

			diagLogoText.Text = Translator.Translate(Define.DefaultCulture, "编辑活动属性");
			documentTitle.Text = diagLogoText.Text;
		}

		[ControllerMethod]
		protected void EditTransitionPropertiesPreRender(string processID, string transitionKey)
		{
			if (this.CurrentProcessDescriptor != null)
			{
				IWfTransitionDescriptor matchedTransition = null;

				foreach (IWfActivityDescriptor actDesp in this.CurrentProcessDescriptor.Activities)
				{
					matchedTransition = actDesp.ToTransitions.Find(t => t.Key == transitionKey);

					if (matchedTransition != null)
						break;
				}

				(matchedTransition != null).FalseThrow("不能在流程中找到Key为{0}的连线", transitionKey);

				this.descriptorEditor.RegisterAfterProcessDeserializedFunction(
					string.Format("WFWeb.BindPropertyGrid(process.Key, findTransitionByKey(process, \"{0}\"));", transitionKey));
			}

			diagLogoText.Text = Translator.Translate(Define.DefaultCulture, "编辑连线属性");
			documentTitle.Text = diagLogoText.Text;
		}

		#endregion Controller Methods

		private IWfProcessDescriptor CurrentProcessDescriptor
		{
			get
			{
				return this.descriptorEditor.CurrentProcessDescriptor;
			}
		}
	}
}