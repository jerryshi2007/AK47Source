using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public class SCOperationSnapshotAdapter : UpdatableAndLoadableAdapterBase<SCOperationSnapshot, SCOperationSnapshotCollection>
	{
		public static readonly SCOperationSnapshotAdapter Instance = new SCOperationSnapshotAdapter();

		private SCOperationSnapshotAdapter()
		{
		}

		/// <summary>
		/// 得到最后的操作时间。实际上查询了SCOperationSnapshot表。如果没有任何操作，则返回“9999-9-9”
		/// </summary>
		/// <returns></returns>
		public DateTime GetLastestOperationTime()
		{
			Dictionary<string, object> context = new Dictionary<string, object>();

			string sql = string.Format("SELECT TOP 1 OperateTime FROM {0} ORDER BY OperateTime DESC", GetMappingInfo(context));

			object maxTime = DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

			if (maxTime == null)
				maxTime = SCConnectionDefine.MaxVersionEndTime;

			return (DateTime)maxTime;
		}

		protected override void BeforeInnerUpdate(SCOperationSnapshot data, Dictionary<string, object> context)
		{
			data.OperateTime = DateTime.MinValue;

			if (DeluxePrincipal.IsAuthenticated)
				data.Operator = DeluxeIdentity.CurrentUser;
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
