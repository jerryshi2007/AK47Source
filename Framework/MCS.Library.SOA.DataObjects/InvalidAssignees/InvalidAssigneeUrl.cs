using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.INVALID_ASSIGNEES_URLS")]
	public class InvalidAssigneeUrl
	{
		private string _NotificationID = string.Empty;

		/// <summary>
		/// 发送消息标识
		/// </summary>

		[ORFieldMapping("NOTIFICATION_ID", PrimaryKey = true)]
		public string NotificationID
		{
			get { return this._NotificationID; }
			set { this._NotificationID = value; }
		}

		private string _ProcessID = string.Empty;
		/// <summary>
		/// 流程ID
		/// </summary>
		[ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
		public string ProcessID
		{
			get { return this._ProcessID; }
			set { this._ProcessID = value; }
		}

		private string _ProcessName = string.Empty;
		/// <summary>
		/// 流程名称
		/// </summary>
		[ORFieldMapping("PROCESS_NAME")]
		public string ProcessName
		{
			get { return this._ProcessName; }
			set { this._ProcessName = value; }
		}

		private string _ActivityID = string.Empty;

		/// <summary>
		/// 环节ID
		/// </summary>
		[ORFieldMapping("ACTIVITY_ID")]
		public string ActivityID
		{
			get { return this._ActivityID; }
			set { this._ActivityID = value; }
		}

		private string _ActivityKey = string.Empty;
		/// <summary>
		/// 环节KEY
		/// </summary>
		[ORFieldMapping("ACTIVITY_KEY")]
		public string ActivityKey
		{
			get { return this._ActivityKey; }
			set { this._ActivityKey = value; }
		}

		private string _Url = string.Empty;
		/// <summary>
		/// 表单地址（链接)
		/// </summary>
		[ORFieldMapping("URL")]
		public string Url
		{
			get { return this._Url; }
			set { this._Url = value; }
		}

		[NoMapping]
		public string AllUsers
		{
			get;
			set;
		}
	}

	[Serializable]
	public class InvalidAssignessUrlCollection : EditableDataObjectCollectionBase<InvalidAssigneeUrl>
	{
	}
}
