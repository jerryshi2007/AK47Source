using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace WorkflowDesigner.ExternalDialogs
{
	internal class DeleteObjectHelper
	{
		private IWfProcess _Process = null;
		private IWfProcessDescriptor _ProcessDescriptor = null;
		private bool _SyncMainStream = false;

		public DeleteObjectHelper(IWfProcess process, IWfProcessDescriptor processDesp, bool syncMSObject)
		{
			process.NullCheck("process");
			processDesp.NullCheck("processDesp");

			this._Process = process;
			this._ProcessDescriptor = processDesp;
			this._SyncMainStream = syncMSObject;
		}

		public WfExecutorBase GetExecutor()
		{
			return (WfExecutorBase)ControllerHelper.ExecuteMethodByRequest(this);
		}

		[ControllerMethod]
		private object ExecuteGetActivityExecutor(string processID, string activityKey)
		{
			IWfActivityDescriptor targetDescriptor = this._ProcessDescriptor.Activities[activityKey];

			(targetDescriptor != null).FalseThrow("不能在流程中找到Key为{0}的活动", targetDescriptor.Key);

			return new WfAdminDeleteActivityExecutor(this._Process.CurrentActivity, targetDescriptor, this._SyncMainStream);
		}

		[ControllerMethod]
		private object ExecuteGetTransitionExecutor(string processID, string transitionKey)
		{
			IWfTransitionDescriptor matchedTransition = null;

			foreach (IWfActivityDescriptor actDesp in this._ProcessDescriptor.Activities)
			{
				matchedTransition = actDesp.ToTransitions.Find(t => t.Key == transitionKey);

				if (matchedTransition != null)
					break;
			}

			(matchedTransition != null).FalseThrow("不能在流程中找到Key为{0}的连线", transitionKey);

			return new WfAdminDeleteTransitionExecutor(this._Process.CurrentActivity, this._ProcessDescriptor, matchedTransition, this._SyncMainStream);
		}
	}
}