using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Globalization;
using MCS.Web.WebControls;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;

namespace WorkflowDesigner.ExternalDialogs
{
	public partial class AdminAutoMoveToDialog : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.operationEditor.PreRenderControl += new EventHandler(descriptorEditor_PreRenderControl);
			this.operationEditor.SaveButtonClicked += new EventHandler(descriptorEditor_SaveButtonClicked);
		}

		protected void descriptorEditor_SaveButtonClicked(object sender, EventArgs e)
		{
			IWfTransitionDescriptor transition = GetNextMoveToTransition(this.operationEditor.CurrentProcess);

			(transition != null).FalseThrow("不能找到下一步能够流转的连线");

			//如果下一步不允许为人员为空，则校验。
			if (transition.ToActivity.Properties.GetValue("AllowEmptyCandidates", true) == false)
			{
				(this.userSelector.SelectedOuUserData.Count > 0).FalseThrow("必须指定下一步的流转人员");
			}

			WfTransferParams transferParams = new WfTransferParams(transition.ToActivity);

			transferParams.FromTransitionDescriptor = transition;
			transferParams.Operator = DeluxeIdentity.CurrentUser;

			foreach (IUser candidate in this.userSelector.SelectedOuUserData)
				transferParams.Assignees.Add(candidate);

			WfMoveToExecutor executor = new WfMoveToExecutor(
				this.operationEditor.CurrentProcess.CurrentActivity,
				this.operationEditor.CurrentProcess.CurrentActivity,
				transferParams);

			executor.Execute();
		}

		protected void descriptorEditor_PreRenderControl(object sender, EventArgs e)
		{
			IWfTransitionDescriptor transition = GetNextMoveToTransition(this.operationEditor.CurrentProcess);

			if (transition != null)
			{
				this.userSelector.SelectedOuUserData.CopyFrom(transition.ToActivity.Instance.Candidates.ToUsers());
			}
		}

		private static IWfTransitionDescriptor GetNextMoveToTransition(IWfProcess process)
		{
			IWfTransitionDescriptor result = null;

			if (process.CurrentActivity != null)
			{
				result = process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions().FindDefaultSelectTransition();
			}

			return result;
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}