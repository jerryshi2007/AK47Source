using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// UserTask ID的类型
	/// </summary>
	[Flags]
	public enum UserTaskIDType
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,

		/// <summary>
		/// 消息ID
		/// </summary>
		[EnumItemDescription("TASK_GUID")]
		TaskID = 1,

		/// <summary>
		/// 对应的文件ID
		/// </summary>
		[EnumItemDescription("RESOURCE_ID")]
		ResourceID = 2,

		/// <summary>
		/// 流程ID
		/// </summary>
		[EnumItemDescription("PROCESS_ID")]
		ProcessID = 4,

		/// <summary>
		/// 流程的活动点ID
		/// </summary>
		[EnumItemDescription("ACTIVITY_ID")]
		ActivityID = 8,

		/// <summary>
		/// 用户ID
		/// </summary>
		[EnumItemDescription("SEND_TO_USER")]
		SendToUserID = 16
	}

	/// <summary>
	/// UserTask中的属性（字段）
	/// </summary>
	[Flags]
	public enum UserTaskFieldDefine
	{
		None = 0,

		/// <summary>
		/// 消息ID
		/// </summary>
		TaskID = 0x1,

		/// <summary>
		/// 
		/// </summary>
		ApplicationName = 0x2,

		/// <summary>
		/// 
		/// </summary>
		ProgramName = 0x4,

		/// <summary>
		/// 
		/// </summary>
		TaskTitle = 0x8,

		/// <summary>
		/// 
		/// </summary>
		ResourceID = 0x10,

		/// <summary>
		/// 
		/// </summary>
		SourceID = 0x20,

		/// <summary>
		/// 
		/// </summary>
		SourceName = 0x40,

		/// <summary>
		/// 
		/// </summary>
		SendToUserID = 0x80,

		/// <summary>
		/// 
		/// </summary>
		Body = 0x100,

		/// <summary>
		/// 
		/// </summary>
		Level = 0x200,

		/// <summary>
		/// 
		/// </summary>
		ProcessID = 0x400,

		/// <summary>
		/// 
		/// </summary>
		ActivityID = 0x800,

		/// <summary>
		/// 
		/// </summary>
		Url = 0x1000,

		/// <summary>
		/// 
		/// </summary>
		Emergency = 0x2000,

		/// <summary>
		/// 
		/// </summary>
		Purpose = 0x4000,

		/// <summary>
		/// 
		/// </summary>
		Status = 0x8000,

		/// <summary>
		/// 
		/// </summary>
		TaskStartTime = 0x10000,

		/// <summary>
		/// 
		/// </summary>
		ExpireTime = 0x20000,

		/// <summary>
		/// 
		/// </summary>
		ReadTime = 0x40000,

		/// <summary>
		/// 
		/// </summary>
		TopFlag = 0x80000,

		/// <summary>
		/// 
		/// </summary>
		Category = 0x100000,

		/// <summary>
		/// 
		/// </summary>
		DeliverTime = 0x200000,

		/// <summary>
		/// 
		/// </summary>
		DraftDepartmentName = 0x400000,

		/// <summary>
		/// 
		/// </summary>
		DraftUserID = 0x800000,

		/// <summary>
		/// 
		/// </summary>
		DraftUserName = 0x1000000,

		/// <summary>
		/// 
		/// </summary>
		All = 0x1FFFFFF
	}

	public delegate void BeforeSendUserTasksDelegete(UserTaskCollection tasks, Dictionary<object, object> context);
	public delegate void SendUserTasksDelegete(UserTaskCollection tasks, Dictionary<object, object> context);

	public delegate void BeforeDeleteUserTaskDelegete(UserTaskCollection tasks, Dictionary<object, object> context);
	public delegate void DeleteUserTasksDelegete(UserTaskCollection tasks, Dictionary<object, object> context);

	public delegate void BeforeDeleteUserAccomplishedTasksDelegete(UserTaskCollection tasks, Dictionary<object, object> context);
	public delegate void DeleteUserAccomplishedTasksDelegete(UserTaskCollection tasks, Dictionary<object, object> context);

	public delegate void BeforeUpdateUserTaskDelegete(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context);
	public delegate int UpdateUserTaskDelegete(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context);

	public delegate void BeforeSetUserTasksAccomplishedDelegete(UserTaskCollection tasks, Dictionary<object, object> context);
	public delegate void SetUserTasksAccomplishedDelegete(UserTaskCollection tasks, Dictionary<object, object> context);

	public class UserTaskOpEventContainer
	{
		public event BeforeSendUserTasksDelegete BeforeSendUserTasks;
		public event SendUserTasksDelegete SendUserTasks;

		public event BeforeDeleteUserTaskDelegete BeforeDeleteUserTasks;
		public event DeleteUserTasksDelegete DeleteUserTasks;

		public event BeforeDeleteUserAccomplishedTasksDelegete BeforeDeleteUserAccomplishedTasks;
		public event DeleteUserAccomplishedTasksDelegete DeleteUserAccomplishedTasks;

		public event BeforeUpdateUserTaskDelegete BeforeUpdateUserTask;
		public event UpdateUserTaskDelegete UpdateUserTask;

		public event BeforeSetUserTasksAccomplishedDelegete BeforeSetUserTasksAccomplished;
		public event SetUserTasksAccomplishedDelegete SetUserTasksAccomplished;

		internal void OnBeforeSendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (BeforeSendUserTasks != null)
				BeforeSendUserTasks(tasks, context);
		}

		internal void OnSendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (SendUserTasks != null)
				SendUserTasks(tasks, context);
		}

		internal void OnBeforeDeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (BeforeDeleteUserTasks != null)
				BeforeDeleteUserTasks(tasks, context);
		}

		internal void OnDeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (DeleteUserTasks != null)
				DeleteUserTasks(tasks, context);
		}

		internal void OnBeforeDeleteUserAccomplishedTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (BeforeDeleteUserAccomplishedTasks != null)
				BeforeDeleteUserAccomplishedTasks(tasks, context);
		}

		internal void OnDeleteUserAccomplishedTasks(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (DeleteUserAccomplishedTasks != null)
				DeleteUserAccomplishedTasks(tasks, context);
		}

		internal void OnBeforeUpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context)
		{
			if (BeforeUpdateUserTask != null)
				BeforeUpdateUserTask(task, idType, fields, context);
		}

		internal int OnUpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context)
		{
			int result = 0;

			if (UpdateUserTask != null)
				result = UpdateUserTask(task, idType, fields, context);

			return result;
		}

		internal void OnBeforeSetUserTasksAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (BeforeSetUserTasksAccomplished != null)
				BeforeSetUserTasksAccomplished(tasks, context);
		}

		internal void OnSetUserTasksAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
		{
			if (SetUserTasksAccomplished != null)
				SetUserTasksAccomplished(tasks, context);
		}
	}

	#region UserOpContext
	internal class UserOpContext
	{
		public readonly List<UserTaskOpEventContainer> EventContainers = new List<UserTaskOpEventContainer>();
		public readonly Dictionary<object, object> Context = new Dictionary<object, object>();

		public void OnBeforeSendUserTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnBeforeSendUserTasks(tasks, Context);
		}

		public void OnSendUserTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnSendUserTasks(tasks, Context);
		}

		public void OnBeforeDeleteUserTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnBeforeDeleteUserTasks(tasks, Context);
		}

		public void OnDeleteUserTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnDeleteUserTasks(tasks, Context);
		}

		public void OnBeforeDeleteUserAccomplishedTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnBeforeDeleteUserAccomplishedTasks(tasks, Context);
		}

		public void OnDeleteUserAccomplishedTasks(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnDeleteUserAccomplishedTasks(tasks, Context);
		}

		public void OnBeforeUpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnBeforeUpdateUserTask(task, idType, fields, Context);
		}

		public int OnUpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields)
		{
			int result = 0;

			foreach (UserTaskOpEventContainer container in EventContainers)
			{
				int r = container.OnUpdateUserTask(task, idType, fields, Context);

				if (r > result)
					result = r;
			}

			return result;
		}

		public void OnBeforeSetUserTasksAccomplished(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnBeforeSetUserTasksAccomplished(tasks, Context);
		}

		public void OnSetUserTasksAccomplished(UserTaskCollection tasks)
		{
			foreach (UserTaskOpEventContainer container in EventContainers)
				container.OnSetUserTasksAccomplished(tasks, Context);
		}
	}
	#endregion

	/// <summary>
	/// UserTask相关操作的接口定义
	/// </summary>
	public interface IUserTaskOperation
	{
		void Init(UserTaskOpEventContainer eventContainer);
	}

	/*
	/// <summary>
	/// 通知发送的接口实现
	/// </summary>
	public interface INotifySender : IDisposable
	{
		void Initialize();
		void Send(EmailNotify notify, IUser receiver);
	}

	public interface INotifySender2 : IDisposable
	{
		void Initialize(Dictionary<object, object> context);
		void PrepareUsers(IList<EmailNotify> notifies, Dictionary<object, object> context);
		void Send(NotificationTarget target, EmailNotify notify, Dictionary<object, object> context);
	}*/
}
