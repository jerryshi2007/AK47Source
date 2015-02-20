using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class WorkflowArchiveOperation : IArchiveOperation
	{
		public static readonly IArchiveOperation Instance = new WorkflowArchiveOperation();

		#region WfProcessDataPack
		private class WfProcessDataPack
		{
			public WfProcessInstanceData ProcessData
			{
				get;
				set;
			}

			public WfProcessCurrentActivityCollection ActivitiesData
			{
				get;
				set;
			}

			public WfProcessCurrentAssigneeCollection AssigneesData
			{
				get;
				set;
			}

			public WfRelativeProcessCollection RelativeProcessesData
			{
				get;
				set;
			}

			public WfProcessRelativeParamCollection ProcessRelativeParams
			{
				get;
				set;
			}
		}
		#endregion WfProcessDataPack

		private WorkflowArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			WfProcessInstanceDataCollection processesData = WfProcessInstanceDataAdapter.Instance.LoadByResourceID(info.ResourceID);

			Dictionary<string, WfProcessDataPack> processesDict = new Dictionary<string, WfProcessDataPack>();

			foreach (WfProcessInstanceData processData in processesData)
			{
				WfProcessDataPack dataPack = new WfProcessDataPack();

				dataPack.ProcessData = processData;

				dataPack.ActivitiesData = WfProcessCurrentActivityAdapter.Instance.Load(processData.InstanceID);
				dataPack.AssigneesData = WfProcessCurrentAssigneeAdapter.Instance.Load(processData.InstanceID);
				dataPack.ProcessRelativeParams = WfProcessRelativeParamsAdapter.Instance.Load(processData.InstanceID);
				dataPack.RelativeProcessesData = WfRelativeProcessAdapter.Instance.Load(processData.InstanceID);

				processesDict.Add(processData.InstanceID, dataPack);
			}

			info.Context["ProcessDataDict"] = processesDict;
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<Dictionary<string, WfProcessDataPack>>("ProcessDataDict", processesDict =>
			{
				foreach (KeyValuePair<string, WfProcessDataPack> kp in processesDict)
				{
					WfProcessInstanceDataAdapter.Instance.Update(kp.Value.ProcessData);
					WfProcessCurrentActivityAdapter.Instance.Update(kp.Key, kp.Value.ActivitiesData);
					WfProcessCurrentAssigneeAdapter.Instance.Update(kp.Key, kp.Value.AssigneesData);
					WfProcessRelativeParamsAdapter.Instance.Update(kp.Key, kp.Value.ProcessRelativeParams);
					WfRelativeProcessAdapter.Instance.Update(kp.Key, kp.Value.RelativeProcessesData);
				}
			});
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			WfRuntime.DeleteProcessByResourceID(info.ResourceID);
		}

		#endregion
	}
}
