using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.Configuration;
using MCS.Library.Workflow.Engine;
using MCS.Library.Workflow.Properties;
using MCS.Library.Caching;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 
	/// </summary>
	public static class WfRuntime
	{
		private class ProcessIDInfo
		{
			public WfProcessCollection Processes = new WfProcessCollection();
			public List<string> NotInCacheProcessIDs = new List<string>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="processType"></param>
		/// <param name="startupParams"></param>
		/// <returns></returns>
		public static IWfProcess StartWorkflow(System.Type processType, WfProcessStartupParams startupParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(processType != null, "processType");
			ExceptionHelper.FalseThrow<ArgumentNullException>(startupParams != null, "startupParams");

			IWfProcess process = (IWfProcess)Activator.CreateInstance(processType, true);

			Type[] types = new Type[] { };

			ConstructorInfo constructorInfoObj = process.GetType().GetConstructor(
				BindingFlags.Instance | BindingFlags.Public, null,
				CallingConventions.HasThis, types, null);

			if (constructorInfoObj != null)
				process = (IWfProcess)constructorInfoObj.Invoke(new object[] { });

			process.Creator = startupParams.Operator;
			process.OwnerDepartment = startupParams.Department;
			process.ResourceID = startupParams.ResourceID;

			WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueInitialWfWorkItem(process);

			AddProcessToCache(process.ID, process);

			return process;
		}

		public static WfProcessCollection GetWfProcesses(params string[] processIDs)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(processIDs != null, "processIDs");

			return GetProcessesFromIDs(processIDs);
		}

		public static WfProcessCollection GetWfProcessesByResourceID(string resourceID)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(resourceID == null, "resourceID");

			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;
			IList<string> processIDs = persistProcess.GetProcessIDsByResourceID(resourceID);

			return GetProcessesFromIDs(processIDs);
		}

		public static WfProcessCollection GetUserRelativeProcessesByResourceID(string resourceID, IUser user)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");
			ExceptionHelper.TrueThrow<ArgumentNullException>(user == null, "user");

			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;
			List<string> processIDs = persistProcess.GetUserRelativeProcessIDsByResourceID(resourceID, user);

			return GetWfProcesses(processIDs.ToArray());
		}

		public static IWfActivity GetWfActivity(string activityID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(activityID, "activityID");

			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;
			string processID = persistProcess.GetProcessIDByActivityID(activityID);

			if (string.IsNullOrEmpty(processID))
				throw new WfRuntimeException(WfRuntimeErrorType.ActivityError, 
					string.Format("不能根据activityID:{0}恢复流程数据", activityID));

			IWfProcess wfProcess = GetWfProcesses(processID)[processID];

			return wfProcess.Activities[activityID];
		}

		public static void ClearProcessCache()
		{
			if (WorkflowSettings.GetConfig().UseGlobalCache)
				WfProcessCache.Instance.Clear();
			else
				WfProcessContextCache.Instance.Clear();
		}

		private static void FillProcessesFromPersistence(WfProcessCollection processes, List<string> notInCacheIDs)
		{
			if (notInCacheIDs.Count > 0)
			{
				IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;
				WfProcessCollection tempCollection = persistProcess.LoadProcesses(notInCacheIDs.ToArray());

				foreach (IWfProcess tempProcess in tempCollection)
				{
					processes.Add(tempProcess);
					AddProcessToCache(tempProcess.ID, tempProcess);
				}
			}
		}

		private static WfProcessCollection GetProcessesFromIDs(IList<string> processIDs)
		{
			ProcessIDInfo result = new ProcessIDInfo();

			foreach (string pID in processIDs)
			{
				IWfProcess process = null;

				if (TryGetProcessFromCache(pID, out process))
					result.Processes.Add(process);
				else
					result.NotInCacheProcessIDs.Add(pID);
			}

			FillProcessesFromPersistence(result.Processes, result.NotInCacheProcessIDs);

			return result.Processes;
		}

		private static void AddProcessToCache(string processID, IWfProcess process)
		{
			if (WorkflowSettings.GetConfig().UseGlobalCache)
			{
				SlidingTimeDependency dependency = new SlidingTimeDependency(WorkflowSettings.GetConfig().GlobalCacheTimeOut);

				WfProcessCache.Instance.Add(processID, process, dependency);
			}
			else
				WfProcessContextCache.Instance.Add(processID, process);
		}

		private static bool TryGetProcessFromCache(string processID, out IWfProcess process)
		{
			bool result = false;

			if (WorkflowSettings.GetConfig().UseGlobalCache)
				result = WfProcessCache.Instance.TryGetValue(processID, out process);
			else
				result = WfProcessContextCache.Instance.TryGetValue(processID, out process);

			return result;
		}
	}
}
