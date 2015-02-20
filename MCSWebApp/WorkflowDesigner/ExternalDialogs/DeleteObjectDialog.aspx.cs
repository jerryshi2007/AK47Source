using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class DeleteObjectDialog : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.operationEditor.PreRenderControl += new EventHandler(descriptorEditor_PreRenderControl);
			this.operationEditor.SaveButtonClicked += new EventHandler(descriptorEditor_SaveButtonClicked);
		}

		protected void descriptorEditor_SaveButtonClicked(object sender, EventArgs e)
		{
			DeleteObjectHelper helper = new DeleteObjectHelper(
				this.CurrentProcessDescriptor.ProcessInstance,
				this.CurrentProcessDescriptor,
				syncMSObj.Checked);

			WfExecutorBase executor = helper.GetExecutor();

			executor.Execute();
		}

		protected void descriptorEditor_PreRenderControl(object sender, EventArgs e)
		{
			ControllerHelper.ExecuteMethodByRequest(this);

			if (this.operationEditor.ShowMainStream)
				this.syncMSObj.Disabled = true;
		}

		[ControllerMethod]
		protected void ExecuteDeleteActivity(string processID, string activityKey)
		{
			if (this.CurrentProcessDescriptor != null)
			{
				IWfActivityDescriptor actDesp = this.CurrentProcessDescriptor.Activities[activityKey];

				(actDesp != null).FalseThrow("不能在流程中找到Key为{0}的活动", activityKey);

				dialogContent.InnerText = string.Format("确认删除活动{0}吗", activityKey);
			}

			diagLogoText.Text = Translator.Translate(Define.DefaultCulture, "删除活动");
			documentTitle.Text = diagLogoText.Text;
		}

		[ControllerMethod]
		protected void ExecuteDeleteTransition(string processID, string transitionKey)
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

				dialogContent.InnerText = string.Format("确认删除连线{0}吗", transitionKey);
			}

			diagLogoText.Text = Translator.Translate(Define.DefaultCulture, "删除连线");
			documentTitle.Text = diagLogoText.Text;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		private IWfProcessDescriptor CurrentProcessDescriptor
		{
			get
			{
				return this.operationEditor.CurrentProcessDescriptor;
			}
		}
	}
}