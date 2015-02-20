using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class AddTransitionDialog : System.Web.UI.Page
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
				IWfActivityDescriptor toActivity = this.descriptorEditor.CurrentProcessDescriptor.Activities[this.Request.Form["ToActivityKey"]];

				IWfTransitionDescriptor transition = this.FromActivity.ToTransitions.AddForwardTransition(toActivity);

				transition.Properties.ReplaceExistedPropertyValues(this.propertyGrid.Properties);

				WfAdminAddTransitionExecutor executor = new WfAdminAddTransitionExecutor(
					this.descriptorEditor.CurrentProcess.CurrentActivity,
					this.descriptorEditor.CurrentProcess,
					transition,
					this.syncMSObj.Checked);

				executor.Execute();
			}
		}

		private void descriptorEditor_PreRenderControl(object sender, EventArgs e)
		{
			if (this.descriptorEditor.CurrentProcess != null)
			{
				WfForwardTransitionDescriptor transitionTemplate = new WfForwardTransitionDescriptor();

				transitionTemplate.Key = this.descriptorEditor.CurrentProcessDescriptor.FindNotUsedTransitionKey();

				PrepareTargetActivityDescriptors();

				this.ClientScript.RegisterHiddenField("template", JSONSerializerExecute.Serialize(transitionTemplate));

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

		private void PrepareTargetActivityDescriptors()
		{
			HtmlSelect select = new HtmlSelect();

			select.Attributes["name"] = "ToActivityKey";
			select.ID = "ToActivityKey";
			select.Style["width"] = "100%";

			if (this.descriptorEditor.CurrentProcessDescriptor != null && this.FromActivityKey.IsNotEmpty())
			{
				foreach (IWfActivityDescriptor actDesp in this.descriptorEditor.CurrentProcessDescriptor.Activities)
				{
					if (actDesp.Key != this.FromActivityKey)
					{
						ListItem opinion = new ListItem(GenerateActivityDescription(this.FromActivity, actDesp), actDesp.Key);

						select.Items.Add(opinion);
					}
				}
			}

			this.targetLiteral.Text = WebControlUtility.GetControlHtml(select);
		}

		private static string GenerateActivityDescription(IWfActivityDescriptor fromActDesp, IWfActivityDescriptor toActDesp)
		{
			StringBuilder strB = new StringBuilder();

			string name = toActDesp.Name;

			strB.AppendFormat(name);

			if (name.IsNotEmpty())
				strB.AppendFormat("({0})", toActDesp.Key);
			else
				strB.Append(toActDesp.Key);

			if (toActDesp.FromTransitions.GetTransitionByFromActivity(fromActDesp) != null)
				strB.Append("-已连线");

			return strB.ToString();
		}
	}
}