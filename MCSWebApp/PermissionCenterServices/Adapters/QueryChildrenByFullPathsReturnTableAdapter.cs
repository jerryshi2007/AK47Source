using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Adapters
{
	public class QueryChildrenByFullPathsReturnTableAdapter : QueryChildrenByFullPathsAdapterBase<DataTable>
	{
		public QueryChildrenByFullPathsReturnTableAdapter(string[] schemaTypes, string[] ids, bool recursively, bool includeNonDefault, bool includeDeleted) :
			base(schemaTypes, ids, recursively, includeNonDefault, includeDeleted)
		{
		}

		protected override DataTable ConvertToResult(MCS.Library.SOA.DataObjects.Security.SCObjectAndRelationCollection relations)
		{
			return QueryHelper.GetOguTableBuilder(this.SchemaTypes).Convert(relations);
		}
	}
}