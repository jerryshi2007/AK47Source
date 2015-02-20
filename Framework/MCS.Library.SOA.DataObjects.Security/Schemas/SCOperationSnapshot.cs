using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 权限中心操作的快照数据
	/// </summary>
	[ORTableMapping("SC.SCOperationSnapshot")]
	[Serializable]
	public class SCOperationSnapshot
	{
		[ORFieldMapping("OperationType", PrimaryKey = true)]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		public SCOperationType OperationType
		{
			get;
			set;
		}

		[ORFieldMapping("OperateTime")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime OperateTime
		{
			get;
			set;
		}

		private IUser _Operator = null;

		[SubClassORFieldMapping("ID", "OperatorID")]
		[SubClassORFieldMapping("DisplayName", "OperatorName")]
		[SubClassType(typeof(OguUser))]
		public IUser Operator
		{
			get
			{
				return this._Operator;
			}
			set
			{
				this._Operator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}
	}

	[Serializable]
	public class SCOperationSnapshotCollection : SerializableEditableKeyedDataObjectCollectionBase<SCOperationType, SCOperationSnapshot>
	{
		public SCOperationSnapshotCollection()
		{
		}

		protected SCOperationSnapshotCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override SCOperationType GetKeyForItem(SCOperationSnapshot item)
		{
			return item.OperationType;
		}
	}
}