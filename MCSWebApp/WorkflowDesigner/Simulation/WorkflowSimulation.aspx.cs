using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Resources;
using MCS.Library.Principal;

namespace WorkflowDesigner.Simulation
{
	public partial class WorkflowSimulation : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(ClientMsgResources));

			if (IsPostBack == false && IsCallback == false)
			{
				ProcessDesckey = WebUtility.GetRequestQueryString("processDesckey", string.Empty);
			}
		}

		private string ProcessDesckey
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "ProcessDesckey", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "ProcessDesckey", value);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			processCreator.SelectedSingleData = DeluxeIdentity.CurrentUser;

			RenderInfo(ProcessDesckey);
		}

		private void RenderInfo(string processDescKey)
		{
			processDescKeyHidden.Value = processDescKey;

			if (processDescKey.IsNotEmpty())
			{
				IWfProcessDescriptor processDesp = WfProcessDescriptorManager.GetDescriptor(processDescKey);

				processDescCaption.InnerText = string.Format("流程Key: {0}, 名称: {1}", processDescKey, processDesp.Name);
			}
		}
	}
}