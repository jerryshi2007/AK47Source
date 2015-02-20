using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.OGUPermission;

namespace MCS.Library.PermissionBridge.Adapters
{
	internal class QueryByGuidsAdapter<T> : QueryByGuidsAdapterBase<OguObjectCollection<T>> where T : IOguObject
	{
		public QueryByGuidsAdapter(string[] schemaTypes, string[] ids)
			: base(schemaTypes, ids, Util.GetContextIncludeDeleted())
		{
		}

		public QueryByGuidsAdapter(string[] schemaTypes, string[] ids, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
		}

		//public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		//{
		//    return SCSnapshotAdapter.Instance.QueryObjectAndRelationByIDs(this.SchemaTypes, this.IDs, this.IncludeDeleted, DateTime.MinValue);
		//}

		protected override OguObjectCollection<T> ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return new OguObjectCollection<T>(relations.ConvertToOguObjects<T>());
		}
	}
}
