using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.DelegationAuthorized
{
	public class LogUtil
	{
		public static readonly string ResourceID = "9406923C-DFA5-49C2-9746-0031950D3DB7";

		public static int AppendLogToDb(Library.SOA.DataObjects.UserOperationLog log)
		{
			return UserOperationLogAdapter.Instance.InsertData(log);
		}

		public static Library.SOA.DataObjects.UserOperationLog CreateAssignLog(WfDelegation data)
		{
			var log = new Library.SOA.DataObjects.UserOperationLog()
			{
				Operator = DeluxeIdentity.CurrentUser,
				RealUser = DeluxeIdentity.CurrentRealUser,
				OperationDateTime = DateTime.Now,
				OperationName = "指派委托",
				Subject = "指派委托",
				OperationType = Library.SOA.DataObjects.OperationType.Add,
				ResourceID = LogUtil.ResourceID,
				OperationDescription = string.Format("指派委托代理:{0}受{1}委托，有效期由{2:yyyy-MM-dd}到{3:yyyy-MM-dd}", data.DestinationUserName, data.SourceUserName, data.StartTime, data.EndTime),
			};
			return log;
		}

		public static Library.SOA.DataObjects.UserOperationLog CreateDissassignLog(WfDelegation data)
		{
			var log = new Library.SOA.DataObjects.UserOperationLog()
			{
				Operator = DeluxeIdentity.CurrentUser,
				RealUser = DeluxeIdentity.CurrentRealUser,
				OperationDateTime = DateTime.Now,
				OperationName = "解除委托",
				Subject = "解除委托",
				OperationType = Library.SOA.DataObjects.OperationType.Delete,
				ResourceID = LogUtil.ResourceID,
				OperationDescription = string.Format("解除指派委托代理:{0}被撤销与{1}委托", data.DestinationUserName, data.SourceUserName, data.StartTime, data.EndTime),
			};
			return log;
		}
	}
}