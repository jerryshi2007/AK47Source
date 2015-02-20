using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.OGUPermission;

namespace MCS.Library.PermissionBridge.Adapters
{
	internal class QueryByCodeNamesAdapter<T> : QueryByCodeNamesAdapterBase<OguObjectCollection<T>> where T : IOguObject
	{
		public QueryByCodeNamesAdapter(string[] schemaTypes, string[] ids)
			: base(schemaTypes, ids, Util.GetContextIncludeDeleted())
		{
		}

		public QueryByCodeNamesAdapter(string[] schemaTypes, string[] ids, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
		}

		//public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		//{
		//    return SCSnapshotAdapter.Instance.QueryObjectAndRelationByCodeNames(this.SchemaTypes, this.IDs, IncludeDeleted, DateTime.MinValue);
		//}

		protected override OguObjectCollection<T> ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return new OguObjectCollection<T>(relations.ConvertToOguObjects<T>());
		}
	}
}
