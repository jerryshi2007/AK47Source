using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal class OrginalUserInfoExtendDataOperation : OriginalDataTableOperationBase
	{
		public OrginalUserInfoExtendDataOperation(AD2DBTransferContext ctx)
			: base(ctx)
		{
		}

		public override void CompareAndModifyData()
		{
			CompareAndUpdateUserExtendData(this.Context);
			CompareAndAddUserExtendData(
				this.Context.ADData.Tables["USERS_INFO_EXTEND"],
				this.Context.OriginalData.Tables["USERS_INFO_EXTEND"],
				this.Context.OriginalUserExtendGuidView);
		}

		private static void CompareAndAddUserExtendData(DataTable srcTable, DataTable targetTable, DataView compareView)
		{
			//提出要添加的行
			foreach (DataRow dr in srcTable.Rows)
			{
				int index = compareView.Find(dr["ID"]);

				if (index == -1)
				{
					//添加新纪录
					DataRow newdr = targetTable.NewRow();

					AD2DBHelper.CopyDataRow(dr, newdr,
						"ID", "MOBILE", "OFFICE_TEL", "GENDER", "NATION", "IM_ADDRESS");

					targetTable.Rows.Add(newdr);
				}
			}
		}

		private static void CompareAndUpdateUserExtendData(AD2DBTransferContext context)
		{
			foreach (DataRow row in context.OriginalData.Tables["USERS_INFO_EXTEND"].Rows)
			{
				string userGuid = row["ID"].ToString();

				int index = context.ADUserExtendGuidView.Find(userGuid);

				if (index != -1)
				{
					CompareAndUpdateUserExtendData(context.ADUserExtendGuidView[index], row);
				}
			}
		}

		private static void CompareAndUpdateUserExtendData(DataRowView drv, DataRow dr)
		{
			//比较并修改原来的记录
			if (CompareUserExtendIsDifferent(drv, dr))
			{
				AD2DBHelper.CopyDataRow(drv.Row, dr,
							"MOBILE",
							"OFFICE_TEL",
							"IM_ADDRESS");
			}
		}

		private static bool CompareUserExtendIsDifferent(DataRowView newdrv, DataRow olddr)
		{
			return AD2DBHelper.CompareDataRowIsDifferent(newdrv, olddr,
			 "MOBILE", "OFFICE_TEL", "IM_ADDRESS");
		}

		public override void DeleteOperation()
		{
			//删除用户表,就需要删除扩展表的信息
			DataView users = new DataView(this.Context.OriginalData.Tables["USERS"], string.Empty, "GUID", DataViewRowState.Deleted);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.UserInfoExtend);
			foreach (DataRowView drv in users)
			{
				try
				{
					string user_delete = string.Format("DELETE USERS_INFO_EXTEND WHERE ID = '{0}'", drv["GUID"].ToString());
					int count = db.ExecuteNonQuery(CommandType.Text, user_delete);

					if (count == 1)
					{
						this.DeleteCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表ID是{0}的记录删除成功",
						//drv["GUID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("用户扩展表 ID 是{0}的执行删除时出错,错误是{1}",
						drv["GUID"].ToString(), ex.Message));
				}
			}
		}

		public override void AddOperation()
		{
			//AddOperationByUsers();
			AddOperationByUsersExtend();
		}

		public void AddOperationByUsersExtend()
		{
			DataView usersExtend = new DataView(this.Context.OriginalData.Tables["USERS_INFO_EXTEND"], string.Empty, "ID", DataViewRowState.Added);

			Database db = DatabaseFactory.Create(this.Context.InitialParams.UserInfoExtend);
			foreach (DataRowView drv in usersExtend)
			{
				InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

				builder.AppendItem("ID", drv["ID"].ToString());
				builder.AppendItem("MOBILE", drv["MOBILE"].ToString());
				builder.AppendItem("OFFICE_TEL", drv["OFFICE_TEL"].ToString());
				builder.AppendItem("GENDER", drv["GENDER"].ToString());
				builder.AppendItem("NATION", drv["NATION"].ToString());
				builder.AppendItem("IM_ADDRESS", drv["IM_ADDRESS"].ToString());

				string sql = string.Format("INSERT INTO USERS_INFO_EXTEND {0}", builder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, sql);

					if (count == 1)
					{
						this.AddCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表ID是{0}的记录添加成功",
						//dr["GUID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表 ID 是{0}的执行插入时出错,错误是{1}",
						drv["ID"].ToString(), ex.Message));
				}
			}
		}

		public void AddOperationByUsers()
		{
			DataView users_add = new DataView(this.Context.OriginalData.Tables["USERS"], string.Empty, "GUID", DataViewRowState.Added);

			Database db = DatabaseFactory.Create(this.Context.InitialParams.UserInfoExtend);
			foreach (DataRowView dr in users_add)
			{
				int index = this.Context.ADUserExtendGuidView.Find(dr["GUID"]);
				DataRowView drv = this.Context.ADUserExtendGuidView[index];

				InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

				builder.AppendItem("ID", drv["ID"].ToString());
				builder.AppendItem("MOBILE", drv["MOBILE"].ToString());
				builder.AppendItem("OFFICE_TEL", drv["OFFICE_TEL"].ToString());
				builder.AppendItem("GENDER", drv["GENDER"].ToString());
				builder.AppendItem("NATION", drv["NATION"].ToString());
				builder.AppendItem("IM_ADDRESS", drv["IM_ADDRESS"].ToString());

				string sql = string.Format("INSERT INTO USERS_INFO_EXTEND {0}", builder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, sql);

					if (count == 1)
					{
						this.AddCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表ID是{0}的记录添加成功",
						//dr["GUID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表 ID 是{0}的执行插入时出错,错误是{1}",
						drv["ID"].ToString(), ex.Message));
				}
			}
		}

		public override void UpdateOperation()
		{
			DataView users_modify = new DataView(this.Context.OriginalData.Tables["USERS_INFO_EXTEND"], string.Empty, "ID", DataViewRowState.ModifiedCurrent);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.UserInfoExtend);

			foreach (DataRowView drs in users_modify)
			{
				UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

				uBuilder.AppendItem("MOBILE", drs["MOBILE"].ToString());
				uBuilder.AppendItem("OFFICE_TEL", drs["OFFICE_TEL"].ToString());
				uBuilder.AppendItem("IM_ADDRESS", drs["IM_ADDRESS"].ToString());

				WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();
				wBuilder.AppendItem("ID", drs["ID"].ToString());

				string updatesql = string.Format("UPDATE USERS_INFO_EXTEND SET {0} where {1}", uBuilder.ToSqlString(TSqlBuilder.Instance), wBuilder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, updatesql);

					if (count == 1)
					{
						this.UpdateCount++;
						//this.Context.InitialParams.Log.Write(string.Format("USERS_INFO_EXTEND表ID是{0}的记录更新成功",
						//drs["ID"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("用户扩展表 ID 是{0}的执行更新时出错,错误是{1}",
						drs["ID"].ToString(), ex.Message));
				}
			}
		}
	}
}
