using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public abstract class QueryChildrenByGuidsAdapterBase<TResult> : QueryByIDsAdapterBase<TResult>
	{
		private bool _IncludeNonDefault = true;
		private bool _Recursively = false;

		public QueryChildrenByGuidsAdapterBase(string[] schemaTypes, string[] ids, bool recursively, bool includeNonDefault, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
			this._IncludeNonDefault = includeNonDefault;
			this._Recursively = recursively;
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(this.SchemaTypes, DecorateIDs(this.IDs), this._Recursively, this._IncludeNonDefault, this.IncludeDeleted, DateTime.MinValue);
		}

		public bool IncludeNonDefault
		{
			get
			{
				return this._IncludeNonDefault;
			}
		}

		public bool Recursively
		{
			get
			{
				return this._Recursively;
			}
		}
	}
}
