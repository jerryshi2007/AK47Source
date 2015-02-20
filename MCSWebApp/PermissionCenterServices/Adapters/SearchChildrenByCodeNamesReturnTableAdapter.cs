using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Adapters
{
	public class SearchChildrenByCodeNamesReturnTableAdapter : QueryChildrenByGuidsAdapterBase<DataTable>
	{
		private string _Keyword;
		private int _MaxCount = 15;

		public SearchChildrenByCodeNamesReturnTableAdapter(string[] schemaTypes, string[] ids, string keyword, int maxCount, bool recursively, bool includeNonDefault, bool includeDeleted) :
			base(schemaTypes, ids, recursively, includeNonDefault, includeDeleted)
		{
			this._MaxCount = maxCount;
			this._Keyword = keyword;
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryObjectAndRelationByKeywordAndParentCodeNames(
				this.SchemaTypes, DecorateIDs(this.IDs), this._Keyword, this._MaxCount, this.Recursively, this.IncludeNonDefault, this.IncludeDeleted, DateTime.MinValue);
		}

		protected override DataTable ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return QueryHelper.GetOguTableBuilder(this.SchemaTypes).Convert(relations);
		}
	}
}