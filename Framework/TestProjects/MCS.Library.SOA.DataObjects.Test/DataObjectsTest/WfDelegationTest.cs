using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using System.Threading;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	/// <summary>
	/// 委托待办相关的测试
	/// </summary>
	[TestClass]
	public class WfDelegationTest
	{
	
		[TestMethod]
		[TestCategory(ProcessTestHelper.Delegation)]
		[Description("流转时，查看委托的待办是否发送成功")]
		public void MovetoDelegationTest()
		{
			WfDelegation delegation = PrepareDelegation();
			WfDelegationAdapter.Instance.Update(delegation); //添加委托

			try
			{
				//创建流程
				IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

				IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
				WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
				process.MoveTo(tp);//流转

				WfRuntime.PersistWorkflows();
				process = WfRuntime.GetProcessByProcessID(process.ID);

				Assert.IsTrue((ProcessTestHelper.GetActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban).Count) >= 2, "至少有两个待办事项。或许其它人也会去为当前人指定相应的委托人");
			}
			finally
			{
				WfDelegationAdapter.Instance.Delete(delegation);　//清理委托
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Delegation)]
		[Description("取消时，查看存在委托的节点的状况")]
		public void CancelDelegationTest()
		{
			WfDelegation delegation = PrepareDelegation();
			WfDelegationAdapter.Instance.Update(delegation);

			try
			{
				IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

				IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
				WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
				process.MoveTo(tp);
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				process.CancelProcess(false);//取消
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				Assert.AreEqual(2, ProcessTestHelper.GetActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Yue).Count, "应该产生两条待阅");
			}
			finally
			{
				WfDelegationAdapter.Instance.Delete(delegation);　//清理委托
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Delegation)]
		[Description("撤回到有存在委托点时，查看存在委托的节点的状况")]
		public void WithdrawDelegationTestReplay()
		{
			WfDelegation delegation = PrepareDelegation();
			WfDelegationAdapter.Instance.Update(delegation);

			try
			{
				IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

				IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
				WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
				process.MoveTo(tp);
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
				tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
				process.MoveTo(tp);
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
				tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
				process.MoveTo(tp);
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				process.Withdraw(process.Activities[2], false); //撤回
				WfRuntime.PersistWorkflows();

				process = WfRuntime.GetProcessByProcessID(process.ID);
				Assert.AreEqual(2, ProcessTestHelper.GetActivityUserTasks(process.Activities[2].ID, TaskStatus.Ban).Count, "应该产生两条待办");
			}
			finally
			{
				WfDelegationAdapter.Instance.Delete(delegation);
			}
		}

		public static WfDelegation PrepareDelegation()
		{
			WfDelegation delegation = new WfDelegation();

			IUser sourceUser = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;
			IUser destinationUser = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			delegation.SourceUserID = sourceUser.ID;
			delegation.SourceUserName = sourceUser.DisplayName;
			delegation.DestinationUserID = destinationUser.ID;
			delegation.DestinationUserName = destinationUser.DisplayName;

			delegation.StartTime = DateTime.Now.AddDays(-1);
			delegation.EndTime = DateTime.Now.AddDays(1);

			return delegation;
		}
	}
}
