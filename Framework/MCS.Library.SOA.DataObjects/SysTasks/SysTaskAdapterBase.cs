using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects
{
	public abstract class SysTaskAdapterBase<T, TCollection> : UpdatableAndLoadableAdapterBase<T, TCollection>
		where T : SysTaskBase
		where TCollection : EditableDataObjectCollectionBase<T>, new()
	{
		public T Load(string taskID)
		{
			taskID.CheckStringIsNullOrEmpty("taskID");

			return Load(builder => builder.AppendItem("TASK_GUID", taskID)).FirstOrDefault();
		}

		/// <summary>
		/// 按照ResourceID进行加载
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public TCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			return Load(builder => builder.AppendItem("RESOURCE_ID", resourceID));
		}

		/// <summary>
		/// 更新任务的状态
		/// </summary>
		/// <param name="taskID"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public bool UpdateStatus(string taskID, SysTaskStatus status)
		{
			taskID.CheckStringIsNullOrEmpty("taskID");

			ORMappingItemCollection mapping = GetMappingInfo(new Dictionary<string, object>());

			UpdateSqlClauseBuilder ub = new UpdateSqlClauseBuilder();
			ub.AppendItem("STATUS", status.ToString());
			ub.AppendItem("START_TIME", "GETDATE()", "=", true);

			WhereSqlClauseBuilder wb = new WhereSqlClauseBuilder();
			wb.AppendItem("TASK_GUID", taskID);

			string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
				mapping.TableName,
				ub.ToSqlString(TSqlBuilder.Instance),
				wb.ToSqlString(TSqlBuilder.Instance));

			return DbHelper.RunSql(sql, this.GetConnectionName()) > 0;
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
