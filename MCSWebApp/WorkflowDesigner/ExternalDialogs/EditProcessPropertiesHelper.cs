using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace WorkflowDesigner.ExternalDialogs
{
	/// <summary>
	/// 编辑流程相关属性的帮助类
	/// </summary>
	internal class EditProcessPropertiesHelper
	{
		private IWfProcess _Process = null;
		private IWfProcessDescriptor _ProcessDescriptor = null;
		private bool _SyncMainStream = false;
		private PropertyValueCollection _Properties;

		public EditProcessPropertiesHelper(IWfProcess process, IWfProcessDescriptor processDesp, PropertyValueCollection properties, bool syncMSObject)
		{
			process.NullCheck("process");
			processDesp.NullCheck("processDesp");
			properties.NullCheck("properties");

			this._Process = process;
			this._ProcessDescriptor = processDesp;
			this._Properties = properties;
			this._SyncMainStream = syncMSObject;
		}

		public WfExecutorBase GetExecutor()
		{
			return (WfExecutorBase)ControllerHelper.ExecuteMethodByRequest(this);
		}

		[ControllerMethod]
		private object ExecuteGetProcessExecutor(string processID)
		{
			this._ProcessDescriptor.Properties.ReplaceExistedPropertyValues(this._Properties);

			return new WfEditProcessPropertiesExecutor(this._Process.CurrentActivity, this._Process, this._ProcessDescriptor, this._SyncMainStream);
		}

		[ControllerMethod]
		private object ExecuteGetActivityExecutor(string processID, string activityKey)
		{
			IWfActivityDescriptor targetDescriptor = this._ProcessDescriptor.Activities[activityKey];

			targetDescriptor.Properties.ReplaceExistedPropertyValues(this._Properties);

			return new WfEditActivityPropertiesExecutor(this._Process.CurrentActivity, this._Process, targetDescriptor, this._SyncMainStream);
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

			matchedTransition.Properties.ReplaceExistedPropertyValues(this._Properties);

			return new WfEditTransitionPropertiesExecutor(this._Process.CurrentActivity, this._Process, matchedTransition, this._SyncMainStream);
		}
	}
}