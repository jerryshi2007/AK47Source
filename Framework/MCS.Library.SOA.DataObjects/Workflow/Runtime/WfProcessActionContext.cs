using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程Action执行时的上下文
	/// </summary>
	[Serializable]
	public class WfProcessActionContext
	{
		/// <summary>
		/// 是否启用仿真
		/// </summary>
		public bool EnableSimulation
		{
			get;
			set;
		}

		/// <summary>
		/// 是否使用服务调用
		/// </summary>
		public bool EnableServiceCall
		{
			get
			{
				return this.EnableSimulation == false || this.SimulationContext.SimulationParameters.EnableServiceCall;
			}
		}

		[NonSerialized]
		private WfSimulationContext _SimulationContext = null;

		/// <summary>
		/// 使用流程仿真
		/// </summary>
		public WfSimulationContext SimulationContext
		{
			get
			{
				if (this._SimulationContext == null)
					this._SimulationContext = new WfSimulationContext();

				return this._SimulationContext;
			}
			internal set
			{
				this._SimulationContext = value;
			}
		}

		internal void ResetContextByProcess(IWfProcess process)
		{
			this.OriginalActivity = process.CurrentActivity;
			this.CurrentProcess = process;
		}

		[NonSerialized]
		private WfActivityChangingContext _ActivityChangingContext = null;

		/// <summary>
		/// 节点变化的上下文
		/// </summary>
		public WfActivityChangingContext ActivityChangingContext
		{
			get
			{
				if (this._ActivityChangingContext == null)
					this._ActivityChangingContext = new WfActivityChangingContext();

				return this._ActivityChangingContext;
			}
		}

		[NonSerialized]
		private WfActivityChangingContext _OldActivityChangingContext = null;

		public void BeginChangeActivityChangingContext()
		{
			(this._OldActivityChangingContext == null).FalseThrow<WfRuntimeException>("不能连续两次执行BeginChangeActivityChangingContext");
			this._OldActivityChangingContext = this.ActivityChangingContext;

			this._ActivityChangingContext = new WfActivityChangingContext();
		}

		public void RestoreChangeActivityChangingContext()
		{
			(this._OldActivityChangingContext != null).FalseThrow<WfRuntimeException>("没有执行BeginChangeActivityChangingContext，不能执行RestoreChangeActivityChangingContext");

			this._ActivityChangingContext = this._OldActivityChangingContext;
			this._OldActivityChangingContext = null;
		}

		[NonSerialized]
		private IWfProcess _CurrentProcess = null;


		public IWfProcess CurrentProcess
		{
			get
			{
				return this._CurrentProcess;
			}
			private set
			{
				if (this._OriginalActivity != null)
					(this._OriginalActivity.Process == value).FalseThrow("设置WfProcessActionContext的CurrentProcess属性时，和Original对应的流程必须一致");

				this._CurrentProcess = value;
			}
		}

		[NonSerialized]
		private IWfActivity _OriginalActivity = null;

		public IWfActivity OriginalActivity
		{
			get
			{
				return this._OriginalActivity;
			}
			set
			{
				if (value != null)
					this._CurrentProcess = value.Process;
				else
					this._CurrentProcess = null;

				this._OriginalActivity = value;
			}
		}

		public IWfActivity CurrentActivity
		{
			get
			{
				IWfActivity result = null;

				if (OriginalActivity != null)
					result = OriginalActivity.Process.CurrentActivity;
				else
				{
					if (this.CurrentProcess != null)
						result = this.CurrentProcess.CurrentActivity;
				}

				return result;
			}
		}

		private Dictionary<string, object> _Items = new Dictionary<string, object>();
		private WfActionCollection _AffectedActions = new WfActionCollection();
		private WfProcessCollection _StatusChangedProcesses = new WfProcessCollection();
		private WfProcessCollection _AffectedProcesses = new WfProcessCollection();
		private WfProcessCollection _AbortedProcesses = new WfProcessCollection();
		private WfProcessCollection _ClosedProcesses = new WfProcessCollection();
		private WfActivityCollection _PendingActivities = new WfActivityCollection();
		private WfAclItemCollection _Acl = new WfAclItemCollection();
		private WfActivityCollection _ReleasedPendingActivities = new WfActivityCollection();
		private WfServiceOperationDefinitionCollection _ServiceOperations = new WfServiceOperationDefinitionCollection();

		/// <summary>
		/// 需要被延迟调用的服务
		/// </summary>
		public WfServiceOperationDefinitionCollection ServiceOperations
		{
			get
			{
				return _ServiceOperations;
			}
		}

		public FileEmergency Emergency
		{
			get;
			set;
		}

		public string Purpose
		{
			get;
			set;
		}

		/// <summary>
		/// 被解除Pending状态的Activity
		/// </summary>
		internal WfActivityCollection ReleasedPendingActivities
		{
			get { return this._ReleasedPendingActivities; }
		}

		/// <summary>
		/// 被作废的流程
		/// </summary>
		internal WfProcessCollection AbortedProcesses
		{
			get { return this._AbortedProcesses; }
		}

		/// <summary>
		/// 已经完成的流程
		/// </summary>
		internal WfProcessCollection ClosedProcesses
		{
			get { return this._ClosedProcesses; }
		}

		/// <summary>
		/// 挂起的活动
		/// </summary>
		internal WfActivityCollection PendingActivities
		{
			get { return this._PendingActivities; }
		}

		/// <summary>
		/// 保存前受影响的流程
		/// </summary>
		public WfProcessCollection AffectedProcesses
		{
			get { return this._AffectedProcesses; }
		}

		/// <summary>
		/// 状态变化的流程
		/// </summary>
		public WfProcessCollection StatusChangedProcesses
		{
			get { return this._StatusChangedProcesses; }
		}

		/// <summary>
		/// 保存前受影响的Action
		/// </summary>
		internal WfActionCollection AffectedActions
		{
			get { return this._AffectedActions; }
		}

		/// <summary>
		/// 访问控制列表
		/// </summary>
		internal WfAclItemCollection Acl
		{
			get { return this._Acl; }
		}

		#region UserTask Properties

		internal UserTaskCollection MoveToUserTasks
		{
			get
			{
				return GetTasksFromContextCache("MoveToUserTasks");
			}
		}

		internal UserTaskCollection NotifyUserTasks
		{
			get
			{
				return GetTasksFromContextCache("NotifyUserTasks");
			}
		}

		internal UserTaskCollection AccomplishedUserTasks
		{
			get
			{
				return GetTasksFromContextCache("AccomplishedUserTasks");
			}
		}

		internal UserTaskCollection DeletedUserTasks
		{
			get
			{
				return GetTasksFromContextCache("DeletedUserTasks");
			}
		}
		#endregion
		/// <summary>
		/// 附加项
		/// </summary>
		public Dictionary<string, object> Items
		{
			get { return _Items; }
			internal set
			{
				_Items = value;
			}
		}

		/// <summary>
		/// 流转时，目标节点是否可以继续流转。如果不能，那么目标节点是Pending状态
		/// </summary>

		public bool TargetActivityCanMoveTo
		{
			get
			{
				bool result = false;
				Dictionary<IWfActivity, bool> canMoveToCache = null;

				if (Items.ContainsKey("TargetActivityCanMoveTo"))
				{
					canMoveToCache = (Dictionary<IWfActivity, bool>)Items["TargetActivityCanMoveTo"];
				}
				else
				{
					canMoveToCache = new Dictionary<IWfActivity, bool>();

					Items.Add("TargetActivityCanMoveTo", canMoveToCache);
				}

				if (this.CurrentActivity != null)
				{
					if (canMoveToCache.TryGetValue(this.CurrentActivity, out result) == false)
					{
						result = this.CurrentActivity.CanMoveTo;

						canMoveToCache.Add(this.CurrentActivity, result);
					}
				}

				return result;
			}
		}

		public event WfMoveToHandler BeforeMoveTo;

		public event WfMoveToHandler AfterMoveTo;

		public event WfActionHandler LeaveActivityPrepareAction;

		public event WfActionHandler LeaveActivityPersistAction;

		public event WfActionHandler EnterActivityPrepareAction;

		public event WfActionHandler EnterActivityPersistAction;

		public event WfActionHandler CancelProcessPrepareAction;

		public event WfActionHandler RestoreProcessPrepareAction;

		public event WfActionHandler CancelProcessPersistAction;

		public event WfActionHandler RestoreProcessPersistAction;

		public event WfActionHandler WithdrawActivityPrepareAction;

		public event WfActionHandler WithdrawActivityPersistAction;

		public event CalculateUserFunction EvaluateTransitionCondition;

		public event CalculateUserFunction EvaluateActivityCondition;

		public event CalculateUserFunction EvaluateDynamicResourceCondition;

		public event CalculateUserFunction EvaluateBranchTemplateCondition;

		public event CalculateUserFunction EvaluateRoleMatrixCondition;

		public event WfPrepareBranchProcessParamsHandler PrepareBranchProcessParams;

		public event WfAfterStartupBranchProcessHandler AfterStartupBranchProcess;

		public event WfGetProcessDescriptorHandler GetProcessDescriptor;

		public event WfRemoveMatrixMergeableRowsHandler RemoveMatrixMergeableRows;

		public void Clear()
		{
			this.AffectedActions.ClearCache();
			this.AffectedActions.Clear();
			this.StatusChangedProcesses.Clear();
			this.AffectedProcesses.Clear();
			this.AbortedProcesses.Clear();
			this.ClosedProcesses.Clear();
			this.DeletedUserTasks.Clear();
			this.PendingActivities.Clear();
			this.MoveToUserTasks.Clear();
			this.NotifyUserTasks.Clear();
			this.AccomplishedUserTasks.Clear();
			this.Items.Clear();
			this.Acl.Clear();
			this.ReleasedPendingActivities.Clear();
		}

		public void NormalizeTaskTitles()
		{
			this.MoveToUserTasks.ForEach(u => NormalizeOneTaskInfo(u));
			this.NotifyUserTasks.ForEach(u => NormalizeOneTaskInfo(u));
			this.AccomplishedUserTasks.ForEach(u => NormalizeOneTaskInfo(u));
		}

		private void NormalizeOneTaskInfo(UserTask task)
		{
			if (task.ProcessID.IsNotEmpty())
			{
				try
				{
					IWfProcess process = WfRuntime.GetProcessByProcessID(task.ProcessID);

					task.TaskTitle = process.ApplicationRuntimeParameters.GetMatchedString(task.TaskTitle);
					task.Url = process.ApplicationRuntimeParameters.GetMatchedString(task.Url);
				}
				catch (WfRuntimeException)
				{
				}
			}
		}

		internal IWfProcessDescriptor FireGetProcessDescriptor(string processDespKey)
		{
			IWfProcessDescriptor result = null;

			if (this.GetProcessDescriptor != null)
				result = this.GetProcessDescriptor(processDespKey);

			return result;
		}

		internal object FireEvaluateRoleMatrixCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			if (EvaluateRoleMatrixCondition != null)
				result = EvaluateRoleMatrixCondition(funcName, paramObjects, callerContext);

			if (result == null && paramObjects.Count == 0)
				result = GetConditionValueFromAppRuntimeParameters((WfConditionDescriptor)callerContext, funcName);

			if (result == null)
				result = CallUserFunctionsPlugIns(funcName, paramObjects, callerContext);

			return result;
		}

		internal object FireEvaluateBranchTemplateCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			if (EvaluateBranchTemplateCondition != null)
				result = EvaluateBranchTemplateCondition(funcName, paramObjects, callerContext);

			if (result == null && paramObjects.Count == 0)
				result = GetConditionValueFromAppRuntimeParameters((WfConditionDescriptor)callerContext, funcName);

			if (result == null)
				result = CallUserFunctionsPlugIns(funcName, paramObjects, callerContext);

			return result;
		}

		internal object FireEvaluateTransitionCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			if (EvaluateTransitionCondition != null)
				result = EvaluateTransitionCondition(funcName, paramObjects, callerContext);

			if (result == null && paramObjects.Count == 0)
				result = GetConditionValueFromAppRuntimeParameters((WfConditionDescriptor)callerContext, funcName);

			if (result == null)
				result = CallUserFunctionsPlugIns(funcName, paramObjects, callerContext);

			return result;
		}

		internal object FireEvaluateActivityCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			if (EvaluateActivityCondition != null)
				result = EvaluateActivityCondition(funcName, paramObjects, callerContext);

			if (result == null && paramObjects.Count == 0)
				result = GetConditionValueFromAppRuntimeParameters((WfConditionDescriptor)callerContext, funcName);

			if (result == null)
				result = CallUserFunctionsPlugIns(funcName, paramObjects, callerContext);

			return result;
		}

		internal object FireEvaluateDynamicResourceCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			if (EvaluateDynamicResourceCondition != null)
				result = EvaluateDynamicResourceCondition(funcName, paramObjects, callerContext);

			if (result == null && paramObjects.Count == 0)
				result = GetConditionValueFromAppRuntimeParameters((WfConditionDescriptor)callerContext, funcName);

			if (result == null)
				result = CallUserFunctionsPlugIns(funcName, paramObjects, callerContext);

			return result;
		}

		internal void FireRemoveMatrixMergeableRows(SOARolePropertyRowUsersCollection rowsUsers, WfMergeMatrixRowParams eventArgs)
		{
			if (RemoveMatrixMergeableRows != null)
				RemoveMatrixMergeableRows(rowsUsers, eventArgs);
		}

		private static object GetConditionValueFromAppRuntimeParameters(WfConditionDescriptor condition, string funcName)
		{
			WfApplicationParametersContext context = WfApplicationParametersContext.Current;

			object result = null;

			//首先从上下文中查找参数
			if (context != null)
				result = context.ApplicationRuntimeParameters.GetValue(funcName, (object)null);

			if (result == null && condition != null && condition.Owner != null)
				result = GetProcessAppRuntimeParameters(condition.Owner.ProcessInstance, funcName);

			return result;
		}

		private static object GetProcessAppRuntimeParameters(IWfProcess process, string funcName)
		{
			object result = null;

			if (process != null)
			{
				switch (funcName.ToLower())
				{
					case "currentactivityid":
						result = process.CurrentActivity.ID;
						break;
					case "currentprocessid":
						result = process.ID;
						break;
					default:
						result = process.ApplicationRuntimeParameters.GetValueRecursively(funcName, (object)null);
						break;
				}
			}

			return result;
		}

		private static object CallUserFunctionsPlugIns(string funcName, ParamObjectCollection paramObjects, object callerContext)
		{
			object result = null;

			foreach (IWfCalculateUserFunction uf in WfActionSettings.GetConfig().Functions)
			{
				result = uf.CalculateUserFunction(funcName, paramObjects, callerContext);

				if (result != null)
					break;
			}

			return result;
		}

		internal void FireBeforeMoveTo(WfMoveToEventArgs eventArgs)
		{
			if (this.BeforeMoveTo != null)
				this.BeforeMoveTo(eventArgs);
		}

		internal void FireAfterMoveTo(WfMoveToEventArgs eventArgs)
		{
			if (this.AfterMoveTo != null)
				this.AfterMoveTo(eventArgs);
		}

		internal void FireLeaveActivityPrepareAction()
		{
			if (this.LeaveActivityPrepareAction != null)
				this.LeaveActivityPrepareAction();
		}

		internal void FireLeaveActivityPersistAction()
		{
			if (this.LeaveActivityPersistAction != null)
				this.LeaveActivityPersistAction();
		}

		internal void FireEnterActivityPrepareAction()
		{
			if (this.EnterActivityPrepareAction != null)
				this.EnterActivityPrepareAction();
		}

		internal void FireEnterActivityPersistAction()
		{
			if (this.EnterActivityPersistAction != null)
				this.EnterActivityPersistAction();
		}

		internal void FireCancelProcessPrepareAction()
		{
			if (this.CancelProcessPrepareAction != null)
				this.CancelProcessPrepareAction();
		}

		internal void FireCancelProcessPersistAction()
		{
			if (this.CancelProcessPersistAction != null)
				this.CancelProcessPersistAction();
		}

		internal void FireRestoreProcessPrepareAction()
		{
			if (this.RestoreProcessPrepareAction != null)
				this.RestoreProcessPrepareAction();
		}

		internal void FireRestoreProcessPersistAction()
		{
			if (this.RestoreProcessPersistAction != null)
				this.RestoreProcessPersistAction();
		}

		internal void FireWithdrawPrepareAction()
		{
			if (this.WithdrawActivityPrepareAction != null)
				this.WithdrawActivityPrepareAction();
		}

		internal void FireWithdrawPersistAction()
		{
			if (this.WithdrawActivityPersistAction != null)
				this.WithdrawActivityPersistAction();
		}

		/// <summary>
		/// 在每一个分支流程启动之前，刚刚准备好分支流程的参数
		/// </summary>
		/// <param name="group"></param>
		/// <param name="branchParams"></param>
		internal void FirePrepareBranchProcessParams(IWfBranchProcessGroup group, WfBranchProcessStartupParamsCollection branchParams)
		{
			if (this.PrepareBranchProcessParams != null)
				this.PrepareBranchProcessParams(group, branchParams);
		}

		/// <summary>
		/// 每一个分支流程启动之后
		/// </summary>
		/// <param name="process"></param>
		internal void FireAfterStartupBranchProcess(IWfProcess process)
		{
			if (this.AfterStartupBranchProcess != null)
				this.AfterStartupBranchProcess(process);
		}

		internal WfProcessActionContextState SaveDifferentProcessInfo(IWfProcess process)
		{
			WfProcessActionContextState state = new WfProcessActionContextState();

			IWfProcess originalProcess = null;

			if (OriginalActivity != null)
				originalProcess = OriginalActivity.Process;

			state.SavedOriginalActivity = OriginalActivity;
			state.NeedToRestore = (originalProcess != process);

			return state;
		}

		internal void RestoreSavedProcessInfo(WfProcessActionContextState state)
		{
			state.NullCheck("state");

			if (state.NeedToRestore)
				this.OriginalActivity = state.SavedOriginalActivity;
		}

		#region Private
		private UserTaskCollection GetTasksFromContextCache(string cacheKey)
		{
			UserTaskCollection result = (UserTaskCollection)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
			{
				UserTaskCollection tasks = new UserTaskCollection();

				cache.Add(key, tasks);

				return tasks;
			});

			return result;
		}
		#endregion
	}
}
