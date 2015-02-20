using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 按照Guid查询来返回对象的基类
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public abstract class QueryByGuidsAdapterBase<TResult> : QueryByIDsAdapterBase<TResult>
	{
		public QueryByGuidsAdapterBase(string[] schemaTypes, string[] ids, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryObjectAndRelationByIDs(this.SchemaTypes, DecorateIDs(this.IDs), this.IncludeDeleted, DateTime.MinValue);
		}
	}
}
