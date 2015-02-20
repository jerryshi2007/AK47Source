using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户操作日志
	/// </summary>
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.USER_OPERATION_TASKS_LOG")]
	public class UserOperationTasksLog
	{
		private int _ID = 0;

		/// <summary>
		/// 排序ID
		/// </summary>
		[ORFieldMapping("LOG_ID", PrimaryKey = true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
			}
		}

		private string _TaskID = string.Empty;

		/// <summary>
		/// 文件ID
		/// </summary>
		[ORFieldMapping("TASK_ID")]
		public string TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}


		private string _SendToUserID = string.Empty;
		/// <summary>
		/// 文件ID
		/// </summary>
		[ORFieldMapping("SEND_TO_USER_ID")]
		public string SendToUserID
		{
			get
			{
				return this._SendToUserID;
			}
			set
			{
				this._SendToUserID = value;
			}
		}

		private string _SendToUserName = string.Empty;
		/// <summary>
		/// 文件ID
		/// </summary>
		[ORFieldMapping("SEND_TO_USER_NAME")]
		public string SendToUserName
		{
			get
			{
				return this._SendToUserName;
			}
			set
			{
				this._SendToUserName = value;
			}
		}

		public static UserOperationTasksLog FromUserTask(UserTask userTask)
		{
			return new UserOperationTasksLog() { SendToUserID = userTask.SendToUserID, SendToUserName = userTask.SendToUserName, TaskID = userTask.TaskID };
		}
	}

	[Serializable]
	[XElementSerializable]
	public sealed class UserOperationTasksLogCollection : EditableDataObjectCollectionBase<UserOperationTasksLog>
	{

	}
}
