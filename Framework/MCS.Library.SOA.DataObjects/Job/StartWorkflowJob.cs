using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.JOB_START_WORKFLOW")]
	public class StartWorkflowJob : JobBase
	{
		public StartWorkflowJob()
			: base()
		{
			this.JobType = JobType.StartWorkflow;
		}

		#region Properties
		[ORFieldMapping("PROCESS_KEY")]
		public string ProcessKey { get; set; }

		private IUser _Operator = null;
		[SubClassORFieldMapping("ID", "OPERATOR_ID")]
		[SubClassORFieldMapping("DisplayName", "OPERATOR_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser Operator
		{
			get
			{
				return this._Operator;
			}
			set
			{
				this._Operator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}
		#endregion

		public override void Start()
		{
			StartWorkflow();
		}

		private void StartWorkflow()
		{
			WfProcessStartupParams startupParams = GenerateStartupParams();
			WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(null, startupParams);

			executor.Execute();
		}

		private WfProcessStartupParams GenerateStartupParams()
		{
			var wfDescriptor = WfProcessDescriptorManager.GetDescriptor(this.ProcessKey);

			wfDescriptor.InitialActivity.Resources.Add(new WfUserResourceDescriptor(this.Creator));

			WfProcessStartupParams startupParams = new WfProcessStartupParams();
			startupParams.ProcessDescriptor = wfDescriptor;
			startupParams.Creator = this.Operator;
			startupParams.Department = this.Operator.TopOU;
			startupParams.Assignees.Add(this.Operator);
			startupParams.DefaultTaskTitle = startupParams.ProcessDescriptor.Name;
			startupParams.ResourceID = UuidHelper.NewUuidString();
			startupParams.AutoCommit = true;

			return startupParams;
		}
	}

	[Serializable]
	public class StartWorkflowJobCollection : EditableKeyedDataObjectCollectionBase<string, StartWorkflowJob>
	{
		protected override string GetKeyForItem(StartWorkflowJob item)
		{
			return item.JobID;
		}
	}
}
