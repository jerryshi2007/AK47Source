using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using System.Transactions;
using MCS.Library.Passport;
using System.Collections.Specialized;

namespace MCS.Web.Library.MVC
{
	[Serializable]
	public class WfClientContext : CommandStateBase, ISerializable
	{
		private WfClientLockCollection _Locks = new WfClientLockCollection();
		private SetLockResult _LockResult = null;

		private int _OriginalUpdateTag = 0;

		private Uri _EntryUri = null;
		private Uri _AbsoluteEntryUri = null;
		private Uri _EntryPath = null;
		private IUser _User = null;

		[NonSerialized]
		private IWfActivity _OriginalActivity = null;

		[NonSerialized]
		private IWfActivity _OriginalCurrentActivity = null;

		[NonSerialized]
		private string defaultOpinionText = string.Empty;

		private WfClientContext()
		{
		}

		/// <summary>
		/// 是否是仿真状态
		/// </summary>
		public static bool SimulationEnabled
		{
			get
			{
				return WebUtility.GetRequestQueryValue("simulation", false);
			}
		}

		/// <summary>
		/// 当前上下文中用到的锁
		/// </summary>
		public WfClientLockCollection Locks
		{
			get
			{
				return this._Locks;
			}
		}

		/// <summary>
		/// 锁检查状态
		/// </summary>
		public SetLockResult LockResult
		{
			get
			{
				return this._LockResult;
			}
		}

		public IWfActivity OriginalActivity
		{
			get
			{

				return this._OriginalActivity;
			}
			set
			{
				this._OriginalActivity = value;
				WfRuntime.ProcessContext.OriginalActivity = value;
			}
		}

		public IWfActivity CurrentActivity
		{
			get
			{
				IWfActivity result = null;

				if (this._OriginalActivity != null)
					result = this._OriginalActivity.Process.CurrentActivity;

				return result;
			}
		}

		/// <summary>
		/// 原始的当前活动
		/// </summary>
		public IWfActivity OriginalCurrentActivity
		{
			get
			{
				return this._OriginalCurrentActivity;
			}
		}

		/// <summary>
		/// 添加表单锁
		/// </summary>
		/// <param name="formID">表单ID</param>
		/// <returns>加锁的结果</returns>
		public SetLockResult AddFormLock(string formID)
		{
			Lock formLockInfo = new Lock(formID, DeluxeIdentity.CurrentUser.ID);

			SetLockResult lockResult = LockAdapter.SetLock(formLockInfo);

			if (lockResult.Succeed)
				this._Locks.Add(lockResult.NewLock);

			return lockResult;
		}

		/// <summary>
		/// 添加表单锁
		/// </summary>
		/// <param name="formID">表单ID</param>
		/// <param name="activityID">工作流节点ID</param>
		/// <returns>加锁的结果</returns>
		public SetLockResult AddFormLock(string formID, string activityID)
		{
			Lock formLockInfo = new Lock(formID, DeluxeIdentity.CurrentUser.ID);

			SetLockResult finalResult = null;

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SetLockResult formLockResult = LockAdapter.SetLock(formLockInfo);

				if (formLockResult.Succeed == false && formLockResult.OriginalLock.LockType == LockType.AdminLock)
				{
					scope.Complete();

					return formLockResult;
				}

				if (formLockResult.Succeed)
					this._Locks.Add(formLockResult.NewLock);

				Lock activityLockInfo = new Lock(activityID, formID, DeluxeIdentity.CurrentUser.ID);

				activityLockInfo.LockType = LockType.ActivityLock;

				SetLockResult activityLockResult = LockAdapter.SetLock(activityLockInfo);

				if (activityLockResult.Succeed)
					this._Locks.Add(activityLockResult.NewLock);

				if (formLockResult.Succeed)
					finalResult = formLockResult;
				else
					finalResult = activityLockResult;

				scope.Complete();

				this._LockResult = finalResult;

				return finalResult;
			}
		}

		/// <summary>
		/// 添加管理员锁
		/// </summary>
		/// <param name="formID">表单锁</param>
		/// <param name="forceLock">是否强制加锁</param>
		/// <returns>加锁的结果</returns>
		public SetLockResult AddAdminLock(string formID, bool forceLock)
		{
			Lock adminLockInfo = new Lock(formID, DeluxeIdentity.CurrentUser.ID);

			adminLockInfo.LockType = LockType.AdminLock;
			SetLockResult adminLockResult = null;

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				adminLockResult = LockAdapter.SetLock(adminLockInfo, forceLock);

				if (adminLockResult.Succeed)
					this._Locks.Add(adminLockResult.NewLock);

				scope.Complete();

				return adminLockResult;
			}
		}

		/// <summary>
		/// 检查锁是否有效，只需一个无效，则抛异常
		/// </summary>
		public void CheckLocksAvailable()
		{
			foreach (Lock lockInfo in this._Locks)
			{
				CheckLockResult result = LockAdapter.CheckLock(lockInfo);

				ExceptionHelper.FalseThrow(
					result.CurrentLockStatus == LockStatus.LockedByRight || result.CurrentLockStatus == LockStatus.LockedByRightAndExpire,
					result.GetCheckLockStatusText());
			}
		}

		/// <summary>
		/// 解锁
		/// </summary>
		public void UnlockAll()
		{
			Lock[] lockArray = new Lock[this._Locks.Count];

			//this._Locks.CopyTo(lockArray, 0);
			LockAdapter.Unlock(lockArray);

			this._Locks.Clear();
		}

		/// <summary>
		/// 执行Executor，同时执行保存逻辑
		/// </summary>
		/// <param name="executor"></param>
		public void Execute(WfExecutorBase executor)
		{
			executor.NullCheck("executor");

			executor.AfterModifyWorkflow += new ExecutorEventHandler(executor_AfterModifyWorkflow);

			try
			{
				IWfProcess currentProcess = executor.Execute();
			}
			finally
			{
				executor.AfterModifyWorkflow -= new ExecutorEventHandler(executor_AfterModifyWorkflow);
			}
		}

		/// <summary>
		/// 执行Executor，但是不执行保存逻辑
		/// </summary>
		/// <param name="executor"></param>
		public void ExecuteNoPersist(WfExecutorBase executor)
		{
			executor.NullCheck("executor");

			executor.AfterModifyWorkflow += new ExecutorEventHandler(executor_AfterModifyWorkflow);

			try
			{
				IWfProcess currentProcess = executor.ExecuteNotPersist();
			}
			finally
			{
				executor.AfterModifyWorkflow -= new ExecutorEventHandler(executor_AfterModifyWorkflow);
			}
		}

		private void executor_AfterModifyWorkflow(WfExecutorDataContext dataContext)
		{
			if (this.OriginalActivity == null)
				OriginalActivity = dataContext.CurrentProcess.CurrentActivity;

			if (this._OriginalCurrentActivity == null)
				this._OriginalCurrentActivity = dataContext.CurrentProcess.CurrentActivity;
		}

		public static WfClientContext Current
		{
			get
			{
				WfClientContext context = (WfClientContext)CommandStateHelper.GetCommandState(typeof(WfClientContext));

				if (context == null)
				{
					context = new WfClientContext();

					context._EntryUri = GetEntryUri();
					context._EntryPath = HttpContext.Current.Request.Url;
					context._AbsoluteEntryUri = GetAbsoluteEntryUri();
					ControllerHelper.ExecuteMethodByRequest(context);
					CommandStateHelper.RegisterState(context);
				}

				return context;
			}
		}

		/// <summary>
		/// 表单入口的Url。相对地址，不含参数
		/// </summary>
		public Uri EntryUri
		{
			get
			{
				return this._EntryUri;
			}
		}

		/// <summary>
		/// 表单入口的Url。绝对地址，不含参数
		/// </summary>
		public Uri AbsoluteEntryUri
		{
			get
			{
				return this._AbsoluteEntryUri;
			}
		}

		/// <summary>
		/// 表单入口的Url。绝对地址，含参数
		/// </summary>
		public Uri EntryPath
		{
			get
			{
				return this._EntryPath;
			}
		}

		public IUser User
		{
			get
			{
				if (this._User == null)
					this._User = GetCurrentUserInfo();

				return this._User;
			}
		}

		/// <summary>
		/// 缺省的意见正文
		/// </summary>
		public string DefaultOpinionText
		{
			get
			{
				return defaultOpinionText;
			}
			set
			{
				defaultOpinionText = value;
			}
		}

		/// <summary>
		/// 流程是否变化了（OriginalActivity和CurrentActivity不一样或者状态不一样）
		/// </summary>
		public bool HasProcessChanged
		{
			get
			{
				return this._OriginalCurrentActivity != this.CurrentActivity;
			}
		}

		/// <summary>
		/// 是否在可流转的状态下
		/// </summary>
		public bool InMoveToMode
		{
			get
			{
				bool result = false;

				IWfActivity oriActivity = this.OriginalActivity;

				if (oriActivity != null)
				{
					//锁判断
					result = this.LockResult == null || this.LockResult.Succeed;

					if (result)
					{
						IWfActivity currentActivity = oriActivity.Process.CurrentActivity;

						result = this._OriginalCurrentActivity.Process.Status == WfProcessStatus.Running
									&& this._OriginalCurrentActivity.Status == WfActivityStatus.Running
									&& currentActivity.Status == WfActivityStatus.Running
									&& this._OriginalCurrentActivity == currentActivity;
						//&& this._OriginalActivity == currentActivity;	//沈峥注释，先不进行此限制

						if (result)
							result = IsUserInAssignees(currentActivity.Assignees);
					}
				}

				return result;
			}
		}

		public bool InCirculateMode
		{
			get
			{
				return false;
			}
		}

		public bool InAdminMode
		{
			get
			{
				bool result = false;

				if (this.OriginalActivity != null)
					result = IsProcessAdmin(DeluxeIdentity.CurrentUser, this.OriginalActivity.Process);

				return result;
			}
		}

		/// <summary>
		/// 用户是否是流程管理员
		/// </summary>
		/// <param name="user"></param>
		/// <param name="process"></param>
		/// <returns></returns>
		public static bool IsProcessAdmin(IUser user, IWfProcess process)
		{
			user.NullCheck("user");
			process.NullCheck("process");

			bool result = RolesDefineConfig.GetConfig().IsCurrentUserInRoles(user, "ProcessAdmin");

			if (result == false)
				result = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.CurrentUser).Contains(
					process.Descriptor.ApplicationName, process.Descriptor.ProgramName, WfApplicationAuthType.FormAdmin);

			return result;
		}

		/// <summary>
		/// 是否是流程的查看者。本方法仅返回流程分类授权的信息，即使是流程环节中的人，也可能返回为False
		/// </summary>
		/// <param name="user"></param>
		/// <param name="process"></param>
		/// <returns></returns>
		public static bool IsProcessViewer(IUser user, IWfProcess process)
		{
			user.NullCheck("user");
			process.NullCheck("process");

			return WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.CurrentUser).Contains(
					process.Descriptor.ApplicationName, process.Descriptor.ProgramName, WfApplicationAuthType.FormViewer);
		}

		/// <summary>
		/// 切换到某个活动，该切换会影响到OriginalActivity、OriginalCurrentActivity以及CurrentActivity属性
		/// </summary>
		/// <param name="originalActivity">原始活动</param>
		public void ChangeTo(IWfActivity originalActivity)
		{
			originalActivity.NullCheck("originalActivity");

			this.OriginalActivity = originalActivity;
			this._OriginalCurrentActivity = originalActivity.Process.CurrentActivity;
		}

		internal string ReplaceEntryPathByProcessID()
		{
			return UriHelper.ReplaceUriParams(this._EntryPath.ToUriString(), uriParams =>
			{
				if (this._OriginalActivity != null)
				{
					if (uriParams["resourceID"] == null)
						uriParams.Add("resourceID", this._OriginalActivity.Process.ResourceID);

					uriParams["processID"] = this._OriginalActivity.Process.ID;

					if (uriParams["activityID"] != null)
						uriParams.Remove("activityID");
				}
			});
		}

		internal string ReplaceEntryPathByActivityID()
		{
			return UriHelper.ReplaceUriParams(this._EntryPath.ToUriString(), uriParams =>
			{
				if (this._OriginalActivity != null)
				{
					if (uriParams["resourceID"] == null)
						uriParams.Add("resourceID", this._OriginalActivity.Process.ResourceID);

					uriParams["activityID"] = this.CurrentActivity.ID;

					//add By Ray 2012/8/6
					uriParams.Remove("sourceResourceID");
				}
			});
		}

		private IUser GetCurrentUserInfo()
		{
			IUser result = null;

			if (OriginalActivity != null)
			{
				//先检查能否办理
				WfAssignee assignee = OriginalActivity.Assignees.FindFirstUser(DeluxeIdentity.CurrentUser.ID);

				if (assignee != null)
					result = assignee.User;
			}

			if (result != null && result.AllRelativeUserInfo.Count > 1)
			{
				//如果可以办理，则检查兼职信息
				if (OriginalActivity != null)
				{
					result = FindFirstUserInDepartment(result.AllRelativeUserInfo,
									OriginalActivity.Process.OwnerDepartment);
				}
			}

			if (result == null)
				result = DeluxeIdentity.CurrentUser;

			return result;
		}

		private IUser FindFirstUserInDepartment(IEnumerable<IUser> users, IOrganization dept)
		{
			IUser result = null;

			foreach (IUser user in users)
			{
				try
				{
					if (user.IsChildrenOf(dept))
					{
						if (result != null)
						{
							if (user.Levels < result.Levels)
								result = user;
						}
						else
							result = user;
					}
				}
				catch (System.Exception)
				{
				}
			}

			return result;
		}

		#region Controller Method
		[ControllerMethod]
		private void InitOriginalActivityByActivityID(string resourceID, string activityID)
		{
			if (activityID.IsNotEmpty())
			{
				IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);

				//取得OriginalActivity
				this._OriginalCurrentActivity = process.CurrentActivity;
				this.OriginalActivity = process.Activities[activityID];

				if (this.OriginalActivity == null)
					this.OriginalActivity = process.CurrentActivity;
			}
		}

		private void InitOriginalActivityByActivityID(string resourceID, string activityID, string originalCurrentActivityID)
		{
			if (activityID.IsNotEmpty())
			{
				IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);

				//取得OriginalActivity
				this._OriginalCurrentActivity = process.Activities[originalCurrentActivityID];
				OriginalActivity = process.Activities[activityID];
			}
		}

		[ControllerMethod]
		private void InitOriginalActivityByResourceID(string resourceID)
		{
			if (resourceID.IsNotEmpty())
			{
				WfProcessCollection procCollection = WfRuntime.GetProcessByResourceID(resourceID);

				(procCollection.Count > 0).FalseThrow<WfRuntimeException>("不能根据'{0}'找到ResourceID对应的流程", resourceID);

				IWfProcess process = procCollection.Find(p =>
				{
					return p.HasParentProcess == false;
				});

				if (process == null)
					process = procCollection[0].SameResourceRootProcess;

				OriginalActivity = process.CurrentActivity;
				this._OriginalCurrentActivity = OriginalActivity;
			}
		}

		[ControllerMethod]
		private void InitOriginalActivityByResourceID(string resourceID, string processID)
		{
			if (processID.IsNotEmpty())
			{
				IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

				process.NullCheck<WfRuntimeException>("不能找到ID为'{0}'的流程", processID);

				OriginalActivity = process.CurrentActivity;
				this._OriginalCurrentActivity = OriginalActivity;
			}
		}
		#endregion

		#region Private
		private static Uri GetEntryUri()
		{
			string url = HttpContext.Current.Request.Url.LocalPath;

			return new Uri(url, UriKind.RelativeOrAbsolute);
		}

		private static Uri GetAbsoluteEntryUri()
		{
			return HttpContext.Current.Request.Url;
		}

		/// <summary>
		/// 当前用户是否在Assignee中，同时也会检查Url
		/// </summary>
		/// <returns></returns>
		private bool IsUserInAssignees(WfAssigneeCollection currentActivityAssignees)
		{
			bool result = false;

			IList<WfAssignee> assignees = currentActivityAssignees.FindAll(a => string.Compare(a.User.ID, this.User.ID, true) == 0);

			if (assignees.Count > 0)
			{
				int urlEmptyCount = 0;

				foreach (WfAssignee assignee in assignees)
				{
					if (assignee.Url.IsNullOrEmpty())
					{
						urlEmptyCount++;
					}
					else
						if (AssigneeUriIsSameAsEntryUri(new Uri(assignee.Url, UriKind.RelativeOrAbsolute)))
						{
							result = true;
							break;
						}
				}

				//如果所有assignees的url都是空，则不比较url了。
				if (result == false && urlEmptyCount == assignees.Count)
					result = true;
			}

			return result;
		}

		private bool AssigneeUriIsSameAsEntryUri(Uri assigneeUri)
		{
			return IsSameUri(AbsoluteEntryUri, assigneeUri);
		}

		private static bool IsSameUri(Uri entry, Uri assigneeUri)
		{
			bool result = true;

			if (assigneeUri.IsAbsoluteUri == false)
				assigneeUri = new Uri(entry, assigneeUri);

			result = Uri.Compare(entry, assigneeUri,
				UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
				UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;

			if (result)
			{
				NameValueCollection entryParams = UriHelper.GetUriParamsCollection(entry);
				NameValueCollection assigneeParams = UriHelper.GetUriParamsCollection(assigneeUri);

				result = entryParams["resourceID"] == assigneeParams["resourceID"];
			}

			return result;
		}
		#endregion

		#region ISerializable Members
		private WfClientContext(SerializationInfo info, StreamingContext context)
		{
			this._EntryUri = (Uri)info.GetValue("EntryUri", typeof(Uri));
			this._AbsoluteEntryUri = (Uri)info.GetValue("AbsoluteEntryUri", typeof(Uri));
			this._EntryPath = (Uri)info.GetValue("EntryPath", typeof(Uri));

			if (info.GetBoolean("HasActivity"))
			{
				string originalActivityID = info.GetString("OriginalActivityID");
				this._OriginalUpdateTag = info.GetInt32("OriginalUpdateTag");

				string originalCurrentActivityID = info.GetString("OriginalCurrentActivity");
				InitOriginalActivityByActivityID(string.Empty, originalActivityID, originalCurrentActivityID);
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("EntryUri", this._EntryUri);
			info.AddValue("AbsoluteEntryUri", this._AbsoluteEntryUri);
			info.AddValue("EntryPath", this._EntryPath);

			if (this.OriginalActivity != null)
			{
				info.AddValue("HasActivity", true);
				info.AddValue("OriginalActivityID", this.OriginalActivity.ID);
				info.AddValue("OriginalCurrentActivity", this._OriginalCurrentActivity.ID);
				info.AddValue("OriginalUpdateTag", this.OriginalActivity.Process.UpdateTag);
			}
			else
			{
				info.AddValue("HasActivity", false);
			}
		}

		#endregion
	}
}
