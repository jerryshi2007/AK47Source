using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 已完成的系统任务的后台适配器
	/// </summary>
	public class SysAccomplishedTaskAdapter : SysTaskAdapterBase<SysAccomplishedTask, SysAccomplishedTaskCollection>
	{
		public static readonly SysAccomplishedTaskAdapter Instance = new SysAccomplishedTaskAdapter();

		private SysAccomplishedTaskAdapter()
		{
		}

		/// <summary>
		/// 根据已经完成系统任务创建新任务
		/// </summary>
		/// <param name="taskID">被移动的任务的ID</param>
		/// <param name="status">重置任务的状态</param>
		public void MoveToNoRunningTask(string taskID)
		{
			var completedTask = this.Load(taskID);
			(completedTask != null).FalseThrow<ArgumentException>("ID为 {0} 的任务不存在", taskID);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("MoveNoRunning({0})", this.GetType().FullName), () =>
			{
				Dictionary<string, object> context = new Dictionary<string, object>();
				ORMappingItemCollection mappingsTask = ORMapping.GetMappingInfo<SysTask>();

				var task = completedTask.CreateNewSystask(UuidHelper.NewUuidString()); ;

				StringBuilder sql = new StringBuilder();

				sql.Append(ORMapping.GetInsertSql(task, mappingsTask, TSqlBuilder.Instance));
				sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					DbHelper.RunSql(sql.ToString(), this.GetConnectionName());

					scope.Complete();
				}
			});
		}
	}
}
