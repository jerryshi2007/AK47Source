using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class UserOperationLogAdapter : UpdatableAndLoadableAdapterBase<UserOperationLog, UserOperationLogCollection>
	{
		public static readonly UserOperationLogAdapter Instance = new UserOperationLogAdapter();

		private UserOperationLogAdapter()
		{
		}

		/// <summary>
		/// 添加一行数据，并返回新主键
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public int InsertData(UserOperationLog data)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<UserOperationLog>();

			string sql = ORMapping.GetInsertSql(data, mappings, TSqlBuilder.Instance);

			decimal result = (decimal)DbHelper.RunSqlReturnScalar(string.Format("{0} \n SELECT @@IDENTITY", sql), this.GetConnectionName());

			return decimal.ToInt32(result);
		}

		public UserOperationLog Load(int id)
		{
			UserOperationLogCollection logCollection = Load(builder => builder.AppendItem("ID", id));

			(logCollection.Count > 0).FalseThrow("不能找到ID为{0}的USER_OPERATION_LOG记录", id);

			return logCollection[0];
		}

		public UserOperationLogCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			UserOperationLogCollection result = Load(builder => builder.AppendItem("RESOURCE_ID", resourceID));

			result.Sort((l1, l2) =>
			{
				int r = 0;

				if (l1.OperationDateTime > l2.OperationDateTime)
					r = -1;
				else
					if (l1.OperationDateTime < l2.OperationDateTime)
						r = 1;

				return r;
			});

			return result;
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(AppLogSettings.GetConfig().ConnectionName);
		}
	}
}
