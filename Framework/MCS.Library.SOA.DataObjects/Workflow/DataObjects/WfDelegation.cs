using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 用户设置的委托待办信息
	/// </summary>
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.DELEGATIONS")]
    [TenantRelativeObject]
	public class WfDelegation
	{
		private bool _Enabled = true;

		/// <summary>
		/// 是否启用
		/// </summary>
		[ORFieldMapping("ENABLED")]
		public bool Enabled
		{
			get { return this._Enabled; }
			set { this._Enabled = value; }
		}

		/// <summary>
		/// 委托人的用户ID
		/// </summary>
		[ORFieldMapping("SOURCE_USER_ID", PrimaryKey = true)]
		public string SourceUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 被委托人的用户ID
		/// </summary>
		[ORFieldMapping("DESTINATION_USER_ID", PrimaryKey = true)]
		public string DestinationUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 委托人的用户名称
		/// </summary>
		[ORFieldMapping("SOURCE_USER_NAME")]
		public string SourceUserName
		{
			get;
			set;
		}

		/// <summary>
		/// 被委托人的用户名称
		/// </summary>
		[ORFieldMapping("DESTINATION_USER_NAME")]
		public string DestinationUserName
		{
			get;
			set;
		}

		/// <summary>
		/// 开始时间
		/// </summary>
		[ORFieldMapping("START_TIME")]
		public DateTime StartTime
		{
			get;
			set;
		}

		/// <summary>
		/// 结束时间
		/// </summary>
		[ORFieldMapping("END_TIME")]
		public DateTime EndTime
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 用户委托待办信息列表
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfDelegationCollection : EditableDataObjectCollectionBase<WfDelegation>
	{
	}
}
