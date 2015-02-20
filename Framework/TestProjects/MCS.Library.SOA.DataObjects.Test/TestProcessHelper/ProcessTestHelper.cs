using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace MCS.Library.SOA.DataObjects.Test
{
	public class ProcessTestHelper
	{
		public const string JSONConverter = "JSON Converter";
		public const string XElementSerialize = "XElement Serialize";
		public const string BranchProcess = "BranchProcess";
		public const string ProcessBehavior_Cancel = "ProcessBehavior_Cancel";
		public const string ProcessBehavior_Withdraw = "ProcessBehavior_Withdraw";
		public const string ProcessBehavior_Moveto = "ProcessBehavior_Moveto";
		public const string ProcessBehavior_Persist = "ProcessBehavior_Persist";
		public const string ProcessBehavior_Start = "ProcessBehavior_Start";
		public const string ProcessBehavior_Return = "ProcessBehavior_Return";
		public const string ExecuteTime = "ExecuteTime";

		public const string Data = "Data";
		public const string Delegation = "Delegation";
		public const string Notify = "Notify";
		public const string Resource = "Resource";
		public const string Lock = "Lock";
		public const string Executor = "Executor";
		public const string ReturnExecutor = "ReturnExecutor";
		public const string WorkItem = "WorkItem";
		public const string Properties = "Properties";
		public const string UserRecentData = "UserRecentData";
		public const string DynamicActivity = "DynamicActivity";
		public const string Simulation = "Simulation";
		public const string WfApplication = "WfApplication";
		public const string Decorator = "Decorator";
		public const string ProcessType = "ProcessType";
		public const string CreateActivityParams = "CreateActivityParams";

		/// <summary>
		/// 与工作流无关的基本数据类型测试
		/// </summary>
		public const string BasicDataObject = "BasicDataObject";

		/// <summary>
		/// 如果不能抽出合适的类型，运行时的（包括点上的人或条件及线等），则放入此类中
		/// </summary>
		public const string Runtime = "Runtime";

		/// <summary>
		/// 如果不能抽出合适的类型，描述的（包括节点的），则放入此类中，针对非运行时的
		/// </summary>
		public const string Descriptor = "Descriptor";

		public static int GetResourceUserTasksAccomplished(string resourceId, string userId)
		{
			string sql = string.Format("select COUNT(*) from WF.USER_ACCOMPLISHED_TASK where RESOURCE_ID='{0}' and SEND_TO_USER='{1}'", resourceId, userId);

			int i = (int)DbHelper.RunSqlReturnScalar(sql, WorkflowSettings.GetConfig().ConnectionName);

			return i;
		}

		/// <summary>
		///获取节点相关的待办/待阅集合
		/// </summary>
		/// <param name="activityId"></param>
		/// <returns></returns>
		public static UserTaskCollection GetActivityUserTasks(string activityId, TaskStatus taskStatus)
		{
			return UserTaskAdapter.Instance.LoadUserTasks(builder =>
			{
				builder.AppendItem("ACTIVITY_ID", activityId);
				builder.AppendItem("STATUS", (int)taskStatus);
			});
		}

		/// <summary>
		/// 查看节点是否收到的待办/待阅
		/// </summary>
		public static bool ExistsActivityUserTasks(string activityID, TaskStatus taskStatus)
		{
			return UserTaskAdapter.Instance.LoadUserTasks(builder =>
			{
				builder.AppendItem("ACTIVITY_ID", activityID);
				builder.AppendItem("STATUS", (int)taskStatus);
			}).Count > 0;
		}

		/// <summary>
		/// 查看某个用户是否收到待办或待阅通知
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="processID"></param>
		/// <param name="taskStatus"></param>
		/// <returns></returns>
		public static bool ExistsActivityUserTasks(string userID, string activityID, TaskStatus taskStatus)
		{
			return UserTaskAdapter.Instance.LoadUserTasks(builder =>
				{
					builder.AppendItem("SEND_TO_USER", userID);
					builder.AppendItem("ACTIVITY_ID", activityID);
					builder.AppendItem("STATUS", (int)taskStatus);
				}).Count > 0;
		}


		/// <summary>
		/// 设置被挂起节点的状态为Running
		/// </summary>
		/// <param name="activityID"></param>
		public static void ProcessPendingActivity(string activityID)
		{
			WfPendingActivityInfoCollection pendingActivities = WfPendingActivityInfoAdapter.Instance.Load(builder => builder.AppendItem("ACTIVITY_ID", activityID));
			pendingActivities.ForEach(pai => WfRuntime.ProcessPendingActivity(pai));
		}

		/// <summary>
		/// 获取参数实例,即设置即将流转的节点为带分支流程
		/// </summary>
		/// <param name="process"></param>
		/// <param name="sequence"></param>
		/// <param name="blockingType"></param>
		/// <returns></returns>
		public static WfTransferParams GetInstanceOfWfTransferParams(IWfProcess process, WfBranchProcessExecuteSequence sequence, WfBranchProcessBlockingType blockingType)
		{
			WfTransferParams tp = new WfTransferParams(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity);

			tp.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object);

			tp.BranchTransferParams.Add(new WfBranchProcessTransferParams(
					CreateConsignTemplate(sequence, blockingType),
					new IUser[] {(IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
						(IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object}));

			return tp;
		}

		/// <summary>
		/// 保证主流程继续执行，使某个节点的所有子流程达到一定条件
		/// </summary>
		/// <param name="procGroupColl"></param>
		public static void CompleteActivityBranchProcessesByBlockingType(IWfActivity activity, WfBranchProcessBlockingType blockingType)
		{
			WfBranchProcessGroupCollection procGroupColl = activity.BranchProcessGroups;

			foreach (WfBranchProcessGroup procGroup in procGroupColl)
			{
				switch (blockingType)
				{
					case WfBranchProcessBlockingType.WaitAllBranchProcessesComplete:
						activity.CompleteBranchProcesses(true);
						break;
					case WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete:
						break;
					case WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete:
						activity.BranchProcessGroups[0].Branches[0].CompleteProcess(true);
						break;
					case WfBranchProcessBlockingType.WaitSpecificBranchProcessesComplete:
						//todo:暂时未实现
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// 获取下一节点的参数实例
		/// </summary>
		/// <param name="nextActivityDesp">下一个节点的流程描述</param>
		/// <param name="oguObject">传入人员的名称</param>
		/// <returns></returns>
		public static WfTransferParams GetInstanceOfWfTransferParams(IWfActivityDescriptor nextActivityDesp, OguObject oguObject)
		{
			WfTransferParams tp = new WfTransferParams(nextActivityDesp);
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects[oguObject.ToString()].Object;
			tp.Assignees.Add(user);
			return tp;
		}


		public static void MoveToAndSaveNextActivity(OguObject oguObject, IWfProcess process)
		{
			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			WfTransferParams transferParams = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, oguObject);
			process.MoveTo(transferParams);
			WfRuntime.PersistWorkflows();
		}

		public static void MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject oguObject, IWfProcess process)
		{
			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions.FindDefaultSelectTransition().ToActivity;
			WfTransferParams transferParams = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, oguObject);
			process.MoveTo(transferParams);
			WfRuntime.PersistWorkflows();
		}

		public static void OutputExecutionTime(Action action, string description)
		{
			Console.WriteLine("执行操作: {0}", description);

			DateTime startTime = DateTime.Now;

			Console.WriteLine("开始时间: {0:yyyy-MM-dd HH:mm:ss}", startTime);
			action();

			DateTime endTime = DateTime.Now;
			Console.WriteLine("结束时间: {0:yyyy-MM-dd HH:mm:ss}", endTime);
			Console.WriteLine("经过时间: {0:#,##0.00}ms", (endTime - startTime).TotalMilliseconds);
		}

		public static List<IWfProcess> StartupMultiProcesses(IWfProcessDescriptor processDesp, int totalProcesses)
		{
			List<IWfProcess> processes = new List<IWfProcess>();

			//准备流程实例
			for (int i = 0; i < totalProcesses; i++)
			{
				WfProcessStartupParams startupParams = new WfProcessStartupParams();
				startupParams.ProcessDescriptor = processDesp;
				IWfProcess process = WfRuntime.StartWorkflow(startupParams);

				processes.Add(process);
			}

			return processes;
		}

		public static IWfBranchProcessTemplateDescriptor CreateConsignTemplate(WfBranchProcessExecuteSequence execSequence, WfBranchProcessBlockingType blockingType)
		{
			WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor("ConsignTemplateKey");
			template.BranchProcessKey = WfProcessDescriptorManager.DefaultConsignProcessKey;
			template.ExecuteSequence = execSequence;
			template.BlockingType = blockingType;
			return template;
		}

	}

	public enum OguObject
	{
		requestor,
		approver1,
		approver2
	}
}
