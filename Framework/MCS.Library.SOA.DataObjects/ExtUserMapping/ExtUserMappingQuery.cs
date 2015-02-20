using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Data;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public class ExtUserMappingQuery : ObjectDataSourceQueryAdapterBase<ExtUserMapping, ExtUserMappingCollection>
	{
		protected override string GetConnectionName()
		{
			return ConnectionDefine.DefaultAccreditInfoConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.OrderByClause = qc.OrderByClause.IsNotEmpty() ? qc.OrderByClause : "USERNAME";
			qc.SelectFields = "*";
		}

		protected override void OnAfterQuery(ExtUserMappingCollection result)
		{
			base.OnAfterQuery(result);
			DataTable dt = ExtUserMappingAdapter.Instance.GetMappingRelationUsers();
			if (dt != null)
			{
				for (int i = 0; i < result.Count; i++)
				{
					SetCorrspondingRelation(result[i], dt);
				}
			}
		}

		private void SetCorrspondingRelation(ExtUserMapping extUser, DataTable dt)
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (dt.Rows[i]["Ext_UserID"].ToString() == extUser.UserID)
				{
					extUser.MappingUserID = dt.Rows[i]["USERID"].ToString();
					extUser.MappingUserLoginName = dt.Rows[i]["LAST_NAME"].ToString() + dt.Rows[i]["FIRST_NAME"].ToString();
				}
			}
		}
	}
}
