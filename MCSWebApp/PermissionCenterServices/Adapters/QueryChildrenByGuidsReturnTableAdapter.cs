using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Adapters
{
	public class QueryChildrenByGuidsReturnTableAdapter : QueryChildrenByGuidsAdapterBase<DataTable>
	{
		public QueryChildrenByGuidsReturnTableAdapter(string[] schemaTypes, string[] ids, bool recursively, bool includeNonDefault, bool includeDeleted) :
			base(schemaTypes, ids, recursively, includeNonDefault, includeDeleted)
		{
		}

		protected override DataTable ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return QueryHelper.GetOguTableBuilder(this.SchemaTypes).Convert(relations);
		}
	}
}