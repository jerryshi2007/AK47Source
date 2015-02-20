using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Adapters
{
	/// <summary>
	/// 根据FullPath查询容器内的人员的Adapter
	/// </summary>
	public class QueryContainersMembersByFullPathsReturnTableAdapter : QueryByGuidsAdapterBase<DataTable>
	{
		public QueryContainersMembersByFullPathsReturnTableAdapter(string[] schemaTypes, string[] ids, bool includeDeleted) :
			base(schemaTypes, ids, includeDeleted)
		{
		}

		public override SCObjectAndRelationCollection QueryObjectsAndRelations()
		{
			return SCSnapshotAdapter.Instance.QueryContainerContainsMembersByFullPaths(this.SchemaTypes, this.IDs, this.IncludeDeleted, DateTime.MinValue);
		}

		protected override DataTable ConvertToResult(SCObjectAndRelationCollection relations)
		{
			return QueryHelper.GetOguTableBuilder(this.SchemaTypes).Convert(relations);
		}
	}
}