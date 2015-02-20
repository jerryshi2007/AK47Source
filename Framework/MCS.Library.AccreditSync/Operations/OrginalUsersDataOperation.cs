using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal class OrginalUsersDataOperation : OriginalDataTableOperationBase
	{
		public OrginalUsersDataOperation(AD2DBTransferContext ctx)
			: base(ctx)
		{
		}

		public override void CompareAndModifyData()
		{
			CompareAndDeleteAndUpdateUsersData(this.Context);

			CompareAndAddUserData(this.Context.ADData.Tables["USERS"], this.Context.OriginalData.Tables["USERS"], this.Context.UsersGuidView);
		}

		private static void CompareAndDeleteAndUpdateUsersData(AD2DBTransferContext context)
		{
			foreach (DataRow row in context.OriginalData.Tables["USERS"].Rows)
			{
				string guid = row["GUID"].ToString();
				int index = context.ADUsersGuidView.Find(guid);

				if (index == -1)
				{
					row.Delete();

					DataRowView[] drvs = context.OriginalOUUserUserGuidView.FindRows(guid);

					Array.ForEach(drvs, drv => drv.Row.Delete());
				}
				else
				{
					CompareAndUpdateUsersData(context.ADUsersGuidView[index], row);
				}
			}
		}

		private static void CompareAndUpdateUsersData(DataRowView drv, DataRow dr)
		{
			//比较并修改原来的记录
			if (CompareUserIsDifferent(drv, dr))
			{
				AD2DBHelper.CopyDataRow(drv.Row, dr,
							"LOGON_NAME",
							"E_MAIL",
							"FIRST_NAME",
							"LAST_NAME");
			}
		}

		/// <summary>
		/// 查询用户表的不同
		/// </summary>
		private static bool CompareUserIsDifferent(DataRowView newdrv, DataRow olddr)
		{
			return AD2DBHelper.CompareDataRowIsDifferent(newdrv, olddr,
			   "FIRST_NAME", "LAST_NAME", "LOGON_NAME", "E_MAIL");
		}

		private static void CompareAndAddUserData(DataTable srcTable, DataTable targetTable, DataView compareView)
		{
			//提出要添加的行
			foreach (DataRow dr in srcTable.Rows)
			{
				int index = compareView.Find(dr["GUID"]);

				if (index == -1)
				{
					//添加新纪录
					DataRow newdr = targetTable.NewRow();

					AD2DBHelper.CopyDataRow(dr, newdr,
						"GUID", "FIRST_NAME", "LAST_NAME", "LOGON_NAME", "PWD_TYPE_GUID", "USER_PWD",
						"RANK_CODE", "POSTURAL", "PINYIN", "E_MAIL");

					targetTable.Rows.Add(newdr);
				}
			}
		}

		public override void DeleteOperation()
		{
			DataView users = new DataView(this.Context.OriginalData.Tables["USERS"], string.Empty, "GUID", DataViewRowState.Deleted);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView drv in users)
			{
				try
				{
					string user_delete = string.Format("DELETE  USERS WHERE GUID = '{0}'", drv["GUID"].ToString());
					int count = db.ExecuteNonQuery(CommandType.Text, user_delete);

					if (count == 1)
					{
						this.DeleteCount++;
						this.Context.InitialParams.Log.Write(string.Format("USERS表GUID是{0},LogonName是{1}的记录删除成功",
						drv["GUID"].ToString(), drv["LOGON_NAME"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("用户表GUID 是{0},LogonName是{1}的记录执行删除时出错,错误是{2}",
						drv["GUID"].ToString(), drv["LOGON_NAME"].ToString(), ex.Message));
				}
			}
		}

		public override void AddOperation()
		{
			//添加用户表
			DataView users = new DataView(this.Context.OriginalData.Tables["USERS"], string.Empty, "GUID", DataViewRowState.Added);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);
			foreach (DataRowView drs in users)
			{
				InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

				builder.AppendItem("GUID", drs["GUID"].ToString());
				builder.AppendItem("FIRST_NAME", drs["FIRST_NAME"].ToString());
				builder.AppendItem("LAST_NAME", drs["LAST_NAME"].ToString());
				builder.AppendItem("LOGON_NAME", drs["LOGON_NAME"].ToString());

				builder.AppendItem("PWD_TYPE_GUID", drs["PWD_TYPE_GUID"].ToString());
				builder.AppendItem("USER_PWD", drs["USER_PWD"].ToString());

				builder.AppendItem("RANK_CODE", drs["RANK_CODE"].ToString());
				builder.AppendItem("POSTURAL", drs["POSTURAL"].ToString());
				builder.AppendItem("PINYIN", drs["PINYIN"].ToString());
				builder.AppendItem("E_MAIL", drs["E_MAIL"].ToString());

				string sql = string.Format("INSERT INTO USERS {0}", builder.ToSqlString(TSqlBuilder.Instance));

				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, sql);

					if (count == 1)
					{
						this.AddCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS表GUID是{0}的记录添加成功",
						//   drs["GUID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("USERS表GUID 是{0},LogonName是{1}的记录执行插入时出错,错误是{2}",
						drs["GUID"].ToString(), drs["LOGON_NAME"].ToString(), ex.Message));
				}

			}
		}

		public override void UpdateOperation()
		{
			//更新用户表

			DataView users = new DataView(this.Context.OriginalData.Tables["USERS"], string.Empty, "GUID", DataViewRowState.ModifiedCurrent);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView drs in users)
			{

				UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

				uBuilder.AppendItem("FIRST_NAME", drs["FIRST_NAME"].ToString());
				uBuilder.AppendItem("LAST_NAME", drs["LAST_NAME"].ToString());
				uBuilder.AppendItem("LOGON_NAME", drs["LOGON_NAME"].ToString());
				uBuilder.AppendItem("USER_PWD", drs["USER_PWD"].ToString());
				uBuilder.AppendItem("PINYIN", drs["PINYIN"].ToString());
				uBuilder.AppendItem("E_MAIL", drs["E_MAIL"].ToString());

				uBuilder.AppendItem("MODIFY_TIME", TSqlBuilder.Instance.DBCurrentTimeFunction, "=", true);

				WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();
				wBuilder.AppendItem("GUID", drs["GUID"].ToString());

				string updatesql = string.Format("UPDATE USERS SET {0} where {1}", uBuilder.ToSqlString(TSqlBuilder.Instance), wBuilder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, updatesql);

					if (count == 1)
					{
						this.UpdateCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS表GUID是{0}的记录更新成功",
						//   drs["GUID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("用户表GUID 是{0},LogonName是{1}的记录执行更新时出错,错误是{2}",
						drs["GUID"].ToString(), drs["LOGON_NAME"].ToString(), ex.Message));
				}

			}
		}
	}
}
