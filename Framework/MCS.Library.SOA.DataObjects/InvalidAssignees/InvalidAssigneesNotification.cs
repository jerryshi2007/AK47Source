using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 有问题的流程消息体
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.INVALID_ASSIGNEES_NOTIFICATION")]
	public class InvalidAssigneesNotification
	{
		private string _NotificationID = string.Empty;

		/// <summary>
		/// 通知ID
		/// </summary>
		[ORFieldMapping("NOTIFICATION_ID", PrimaryKey = true)]
		public string NotificationID
		{
			get { return this._NotificationID; }
			set { this._NotificationID = value; }
		}

		private string _Description = string.Empty;
		/// <summary>
		/// 消息体
		/// </summary>
		[ORFieldMapping("DESCRIPTION")]
		public string Description
		{
			get { return this._Description; }
			set { this._Description = value; }
		}

		private DateTime _CreateTime = DateTime.MinValue;

		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CREATE_TIME")]
		public DateTime CreateTime
		{
			get { return this._CreateTime; }
			set { this._CreateTime = value; }
		}
	}

	[Serializable]
	public class InvalidAssigneesNotificationCollection : EditableDataObjectCollectionBase<InvalidAssigneesNotification>
	{
	}
}
