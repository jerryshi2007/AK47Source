using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;

namespace WorkflowDesigner
{
	public static class WfProcessDescHelper
	{
		/// <summary>
		/// 保存流程模板
		/// </summary>
		/// <param name="wfProcess"></param>
		public static void SaveWfProcess(IWfProcessDescriptor wfProcess)
		{
			var pManager = WorkflowSettings.GetConfig().GetDescriptorManager();
			pManager.SaveDescriptor(wfProcess);

			//change import time
            WfProcessDescriptorInfoAdapter.Instance.UpdateImportTime(wfProcess.Key, DeluxeIdentity.CurrentUser);

			//write log
			UserOperationLog log = new UserOperationLog()
			{
				ResourceID = wfProcess.Key,
				Operator = DeluxeIdentity.CurrentUser,
				OperationDateTime = DateTime.Now,
				Subject = "导入流程模板",
				OperationName = "导入",
				OperationDescription = "导入流程模板"
			};
			UserOperationLogAdapter.Instance.Update(log);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="defaultProcOnly">load default process only</param>
		/// <returns>a anonymous object list, the object contains Key, Text field.</returns>
		public static List<object> GetAllProcessDescKeys(bool defaultProcOnly)
		{
			var result = new List<object>();

			result.Add(new
			{
				Key = WfProcessDescriptorManager.DefaultApprovalProcessKey,
				Text = WfProcessDescriptorManager.DefaultApprovalProcessKey + "-默认审批流程"
			});
			result.Add(new
			{
				Key = WfProcessDescriptorManager.DefaultConsignProcessKey,
				Text = WfProcessDescriptorManager.DefaultConsignProcessKey + "-默认会签流程"
			});
			result.Add(new
			{
				Key = WfProcessDescriptorManager.DefaultCirculationProcessKey,
				Text = WfProcessDescriptorManager.DefaultCirculationProcessKey + "-默认传阅流程"
			});

			if (!defaultProcOnly)
			{
				WfProcessDescriptorInfoCollection processDescInfos =
					WfProcessDescriptorInfoAdapter.Instance.LoadWfProcessDescriptionInfos(builder => builder.AppendItem("PROCESS_KEY", "", "<>"), true);

				foreach (var item in processDescInfos)
				{
					result.Add(new { Key = item.ProcessKey, Text = item.ProcessKey + "-" + item.ProcessName });
				}
			}

			return result;
		}
	}
}