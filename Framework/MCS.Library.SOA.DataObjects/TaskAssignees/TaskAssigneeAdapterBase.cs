using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// TaskAssignee相关的参数
	/// </summary>
	/// <typeparam name="TTaskAssignee"></typeparam>
	/// <typeparam name="TOguObject"></typeparam>
	[Serializable]
	public abstract class TaskAssigneeAdapterBase<TTaskAssignee, TTaskAssigneeCollection, TOguObject> :
		UpdatableAndLoadableAdapterBase<TTaskAssignee, TTaskAssigneeCollection>
		where TOguObject : IOguObject
		where TTaskAssignee : TaskAssigneeBase<TOguObject>, new()
		where TTaskAssigneeCollection : TaskAssigneeCollectionBase<TTaskAssignee, TOguObject>, new()
	{
		/// <summary>
		/// 按照ResourceID加载集合
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public virtual TTaskAssigneeCollection Load(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			return Load(wb => wb.AppendItem("RESOURCE_ID", resourceID),
				ob => ob.AppendItem("INNER_ID", Data.Builder.FieldSortDirection.Ascending));
		}

		/// <summary>
		/// 按照ResourceID来更新数据
		/// </summary>
		/// <param name="resourceID"></param>
		/// <param name="assignees"></param>
		public virtual void Update(string resourceID, TTaskAssigneeCollection assignees)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");
			assignees.NullCheck("assignees");

			StringBuilder strB = new StringBuilder();

			ORMappingItemCollection mappings = GetMappingInfo(new Dictionary<string, object>());

			strB.AppendFormat("DELETE {0} WHERE RESOURCE_ID = {1}",
				mappings.TableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(resourceID));

			foreach (TTaskAssignee assignee in assignees)
			{
				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(assignee, mappings);

				strB.AppendFormat("INSERT INTO {0}{1}",
					mappings.TableName,
					builder.ToSqlString(TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		/// <summary>
		/// 设置对象的连接名称
		/// </summary>
		/// <returns></returns>
		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
