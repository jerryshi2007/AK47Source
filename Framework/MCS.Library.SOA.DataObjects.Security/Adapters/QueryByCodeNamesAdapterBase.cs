using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 根据CodeName查询对象的Adapter
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public abstract class QueryByCodeNamesAdapterBase<TResult> : QueryByIDsAdapterBase<TResult>
	{
		public QueryByCodeNamesAdapterBase(string[] schemaTypes, string[] ids, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryObjectAndRelationByCodeNames(this.SchemaTypes, DecorateIDs(this.IDs), this.IncludeDeleted, DateTime.MinValue);
		}
	}
}
