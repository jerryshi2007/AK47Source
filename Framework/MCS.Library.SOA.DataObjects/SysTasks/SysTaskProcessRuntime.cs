using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	public static class SysTaskProcessRuntime
	{
		/// <summary>
		/// 启动流程，持久化，然后发送执行活动的任务
		/// </summary>
		/// <param name="process"></param>
		public static void StartProcess(SysTaskProcess process)
		{
			process.NullCheck("process");

			process.Status = SysTaskProcessStatus.Running;

            ProcessContext.AffectedProcesses.AddOrReplace(process);

            if (process.Activities.Count == 0)
                process.Status = SysTaskProcessStatus.Completed;

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                Persist();

                if (process.Activities.Count > 0)
                    ExecuteSysTaskActivityTask.SendTask(process.Activities[0]);

                scope.Complete();
            }
		}

		/// <summary>
		/// 根据流程ID获取任务流程，如果不存在则抛出异常
		/// </summary>
		/// <param name="processID"></param>
		/// <returns></returns>
		public static SysTaskProcess GetProcessByID(string processID)
		{
			processID.CheckStringIsNullOrEmpty(processID);

			SysTaskProcess result = ProcessContext.LoadedProcesses[processID];

			if (result == null)
			{
				result = SysTaskProcessAdapter.Instance.Load(processID);
				ProcessContext.LoadedProcesses.Add(result);
			}

			return result;
		}

		/// <summary>
		/// 根据流程活动的ID获取任务流程，如果不存在则抛出异常
		/// </summary>
		/// <param name="activityID"></param>
		/// <returns></returns>
		public static SysTaskProcess GetProcessByActivityID(string activityID)
		{
			activityID.CheckStringIsNullOrEmpty("activityID");

			SysTaskProcess result = SysTaskProcessAdapter.Instance.LoadByActivityID(activityID);

			if (ProcessContext.LoadedProcesses.ContainsKey(result.ID))
			{
				result = ProcessContext.LoadedProcesses[result.ID];
			}
			else
			{
				ProcessContext.LoadedProcesses.Add(result);
			}

			return result;
		}

		public static SysTaskProcessRuntimeContext ProcessContext
		{
			get
			{
				object processContext = null;

				processContext = ObjectContextCache.Instance.GetOrAddNewValue("SysTaskProcessRuntimeContext", (cache, key) =>
				{
					processContext = new SysTaskProcessRuntimeContext();
					cache.Add("SysTaskProcessRuntimeContext", processContext);

					return processContext;
				});

				return (SysTaskProcessRuntimeContext)processContext;
			}
		}

		public static void Persist()
		{
			ProcessContext.AffectedProcesses.ForEach(p => p.NormalizeActivities());

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				ProcessContext.DeletedActivities.ForEach(a => SysTaskActivityAdapter.Instance.Delete(a));
				ProcessContext.AffectedActivities.ForEach(a => SysTaskActivityAdapter.Instance.Update(a));
				ProcessContext.AffectedProcesses.ForEach(p => SysTaskProcessAdapter.Instance.Update(p));

				scope.Complete();
			}

			ClearCache();
		}

		public static void ClearCache()
		{
			ProcessContext.AffectedActivities.Clear();
			ProcessContext.DeletedActivities.Clear();
			ProcessContext.AffectedProcesses.Clear();
			ProcessContext.LoadedProcesses.Clear();
		}
	}
}
