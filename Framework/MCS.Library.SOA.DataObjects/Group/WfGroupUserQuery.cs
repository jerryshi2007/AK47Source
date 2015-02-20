using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class WfGroupUserQuery : ObjectDataSourceQueryAdapterBase<WfGroupUser, WfGroupUserCollection>
	{
		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}

		protected override void OnBuildQueryCondition(MCS.Library.Data.DataObjects.QueryCondition qc)
		{
			qc.OrderByClause = qc.OrderByClause.IsNotEmpty() ? qc.OrderByClause : "USER_NAME";
			qc.SelectFields = "*";
		}
	}
}
