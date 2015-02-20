using System;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages
{
	public class UploadFileHistoryQuery : ObjectDataSourceQueryAdapterBase<UploadFileHistory, UploadFileHistoryCollection>
	{

		public UploadFileHistoryQuery()
		{
			//UploadFileHistory
		}

		protected override string GetConnectionName()
		{
			return AppLogSettings.GetConfig().ConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.OrderByClause = "CREATE_TIME DESC";
			base.OnBuildQueryCondition(qc);
		}

	}
}