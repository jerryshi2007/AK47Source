using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;
using MCS.Library.OGUPermission;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元操作的快照
	/// </summary>
	[ORTableMapping("SC.SCOperationSnapshot")]
	[Serializable]
	public class AUOperationSnapshot
	{
		[ORFieldMapping("OperationType", PrimaryKey = true)]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		public AUOperationType OperationType
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
	public class AUOperationSnapshotCollection : SerializableEditableKeyedDataObjectCollectionBase<AUOperationType, AUOperationSnapshot>
	{
		public AUOperationSnapshotCollection()
		{
		}

		protected AUOperationSnapshotCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override AUOperationType GetKeyForItem(AUOperationSnapshot item)
		{
			return item.OperationType;
		}
	}
}
