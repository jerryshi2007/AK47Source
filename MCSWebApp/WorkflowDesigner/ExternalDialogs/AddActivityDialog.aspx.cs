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

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class AddActivityDialog : ExternalPropertyEditorBase
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.descriptorEditor.PreRenderControl += new EventHandler(descriptorEditor_PreRenderControl);
			this.descriptorEditor.SaveButtonClicked += new EventHandler(descriptorEditor_SaveButtonClicked);
		}

		private void descriptorEditor_SaveButtonClicked(object sender, EventArgs e)
		{
			if (this.descriptorEditor.CurrentProcess != null)
			{
				WfActivityDescriptor activityTemplate = new WfActivityDescriptor(
					this.descriptorEditor.CurrentProcessDescriptor.FindNotUsedActivityKey(),
					WfActivityType.NormalActivity);

				activityTemplate.Properties.ReplaceExistedPropertyValues(this.propertyGrid.Properties);

				this.descriptorEditor.CurrentProcessDescriptor.Activities.Add(activityTemplate);

				if (this.descriptorEditor.CurrentProcessDescriptor.IsMainStream == false)
					WfActivityBase.CreateActivityInstance(activityTemplate, this.descriptorEditor.CurrentProcess);

				WfAdminAddActivityExecutor executor = new WfAdminAddActivityExecutor(
					this.descriptorEditor.CurrentProcess.CurrentActivity,
					this.descriptorEditor.CurrentProcess,
					this.FromActivity,
					activityTemplate,
					this.syncMSObj.Checked);

				executor.Execute();
			}
		}

		private void descriptorEditor_PreRenderControl(object sender, EventArgs e)
		{
			if (this.descriptorEditor.CurrentProcess != null)
			{
				WfActivityDescriptor activityTemplate = new WfActivityDescriptor(
					this.descriptorEditor.CurrentProcessDescriptor.FindNotUsedActivityKey(),
					WfActivityType.NormalActivity);

				this.ClientScript.RegisterHiddenField("template", JSONSerializerExecute.Serialize(activityTemplate));

				this.descriptorEditor.RegisterAfterProcessDeserializedFunction(
					"WFWeb.BindPropertyGrid(process.Key, Sys.Serialization.JavaScriptSerializer.deserialize($get(\"template\").value));");
			}

			if (this.descriptorEditor.ShowMainStream)
				this.syncMSObj.Disabled = true;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		private string FromActivityKey
		{
			get
			{
				return WebUtility.GetRequestQueryString("fromActivityKey", string.Empty);
			}
		}

		private IWfActivityDescriptor FromActivity
		{
			get
			{
				IWfActivityDescriptor result = null;

				if (this.descriptorEditor.CurrentProcessDescriptor != null && this.FromActivityKey.IsNotEmpty())
					result = this.descriptorEditor.CurrentProcessDescriptor.Activities[this.FromActivityKey];

				return result;
			}
		}
	}
}