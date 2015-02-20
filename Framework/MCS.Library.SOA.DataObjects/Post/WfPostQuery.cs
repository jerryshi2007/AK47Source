using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public class WfPostQuery : ObjectDataSourceQueryAdapterBase<WfPost, WfPostCollection>
	{
		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.OrderByClause = qc.OrderByClause.IsNotEmpty() ? qc.OrderByClause : "POST_NAME";
			qc.SelectFields = "*";
		}
	}
}
