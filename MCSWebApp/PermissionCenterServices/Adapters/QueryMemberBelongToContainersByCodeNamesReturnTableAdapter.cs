using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Adapters
{
	public class QueryMemberBelongToContainersByCodeNamesReturnTableAdapter : QueryByGuidsAdapterBase<DataTable>
	{
		public QueryMemberBelongToContainersByCodeNamesReturnTableAdapter(string[] schemaTypes, string[] ids, bool includeDeleted) :
			base(schemaTypes, ids, includeDeleted)
		{
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryMemberBelongToContainersByCodeNames(this.SchemaTypes, this.IDs, this.IncludeDeleted, DateTime.MinValue);
		}

		protected override DataTable ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return QueryHelper.GetOguTableBuilder(this.SchemaTypes).Convert(relations);
		}
	}
}