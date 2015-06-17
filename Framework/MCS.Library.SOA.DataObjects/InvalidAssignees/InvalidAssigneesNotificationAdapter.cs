using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
	public class InvalidAssigneesNotificationAdapter
	//: UpdatableAndLoadableAdapterBase<InvalidAssigneesNotification, InvalidAssigneesNotificationCollection>
	{
		public static readonly InvalidAssigneesNotificationAdapter Instance = new InvalidAssigneesNotificationAdapter();

		protected const string InsertSql = @"INSERT INTO WF.INVALID_ASSIGNEES_NOTIFICATION (NOTIFICATION_ID, DESCRIPTION, CREATE_TIME) VALUES (@NOTIFICATION_ID, @DESCRIPTION, @CREATETIME) ";

		private InvalidAssigneesNotificationAdapter()
		{
		}

		/// <summary>
		/// 添加单条管理员处消息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool AddData(InvalidAssigneesNotification data)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(context);
					DbCommand command = db.GetSqlStringCommand(InsertSql);

					this.SetCommandParames(db, command, data);

					int count = command.ExecuteNonQuery();

					scope.Complete();

					return count == 1 ? true : false;
				}
			}
		}

		public void BulkAdd(InvalidAssigneesNotificationCollection data)
		{
			if (data.Count == 0)
				return;

			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<InvalidAssigneesNotification>();

			StringBuilder sqlStrB = new StringBuilder();

			foreach (InvalidAssigneesNotification item in data)
			{
				if (sqlStrB.Length > 0)
					sqlStrB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				sqlStrB.Append(ORMapping.GetInsertSql(item, mappings, TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(sqlStrB.ToString(), this.GetConnectionName());
		}

		private void SetCommandParames(Database db, DbCommand command, InvalidAssigneesNotification data)
		{
			db.AddInParameter(command, "NOTIFICATION_ID", DbType.String, string.IsNullOrEmpty(data.NotificationID) ? UuidHelper.NewUuidString() : data.NotificationID);
			db.AddInParameter(command, "Description", DbType.String, data.Description);
			db.AddInParameter(command, "CreateTime", DbType.String, data.CreateTime);
		}

		private string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
