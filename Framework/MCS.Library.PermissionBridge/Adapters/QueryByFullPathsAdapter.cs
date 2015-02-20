using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.OGUPermission;

namespace MCS.Library.PermissionBridge.Adapters
{
	internal class QueryByFullPathsAdapter<T> : QueryByFullPathsAdapterBase<OguObjectCollection<T>> where T : IOguObject
	{
		public QueryByFullPathsAdapter(string[] schemaTypes, string[] ids)
			: base(schemaTypes, ids, Util.GetContextIncludeDeleted())
		{
		}

		public QueryByFullPathsAdapter(string[] schemaTypes, string[] ids, bool includeDeleted)
			: base(schemaTypes, ids, includeDeleted)
		{
		}

		//public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		//{
		//    SCObjectAndRelationCollection result = SCSnapshotAdapter.Instance.QueryByMultiFullPaths(this.IDs);

		//    result.Remove(sor => Array.Exists(this.SchemaTypes, s => s == sor.SchemaType) == false);

		//    return result;
		//}

		protected override OguObjectCollection<T> ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return new OguObjectCollection<T>(relations.ConvertToOguObjects<T>());
		}
	}
}
