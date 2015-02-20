using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal class OrginalOUUsersDataOperation : OriginalDataTableOperationBase
	{
		public OrginalOUUsersDataOperation(AD2DBTransferContext ctx)
			: base(ctx)
		{
		}

		public override void CompareAndModifyData()
		{
			CompareAndDeleteAndUpdateOuUsersData(this.Context);
			CompareAndAddOuUsersData(this.Context.ADData.Tables["OU_USERS"], this.Context.OriginalData.Tables["OU_USERS"],
				this.Context.OriginalOUUserParentUserGuidView, this.Context);
		}

		private static void CompareAndDeleteAndUpdateOuUsersData(AD2DBTransferContext context)
		{
			foreach (DataRow row in context.OriginalData.Tables["OU_USERS"].Rows)
			{
				if (row.RowState != DataRowState.Deleted)
				{
					string userGuid = row["USER_GUID"].ToString();
					string parentID = row["PARENT_GUID"].ToString();

					DataRowView[] drvs = context.ADOuUserParentGuidView.FindRows(new string[] { userGuid, parentID });

					if (drvs.Length != 0)
					{
						//在AD中存在，且OU和AD中的相同
						CompareAndUpdateOuUsersData(drvs[0], row);
					}
					else
					{
						//在AD中不存在或者OU和AD匹配不上
						if (context.ADUsersGuidView.FindRows(new string[] { userGuid }).Length > 0)
						{
							//在AD中User存在，但是OU不匹配
							//特殊情况（有后来在数据库维护的兼职情况）改职位是否为兼职（0、主职；1、兼职）
							int sideline = Convert.ToInt32(row["SIDELINE"]);

							if (sideline == 0)
							{
								row.Delete();
							}
							else
							{
								int oguIndex = context.ADOguGuidView.Find(row["PARENT_GUID"]);

								if (oguIndex == -1)
								{
									row.Delete();
								}
								else
								{
									//修改这一段，代码，需要在ORIGINAL_SORT后面再加上自己的序号，并且调整"CHILDREN_COUNTER"
									DataRowView oguDrv = context.ADOguGuidView[oguIndex];
									string ogSort = (string)oguDrv["ORIGINAL_SORT"];
									string newSort = string.Format("{0}{1:000000}", ogSort, oguDrv["CHILDREN_COUNTER"]);

									if ((string)row["ORIGINAL_SORT"] != newSort)
									{
										row["ORIGINAL_SORT"] = newSort;
										row["GLOBAL_SORT"] = newSort;
										oguDrv["CHILDREN_COUNTER"] = (int)oguDrv["CHILDREN_COUNTER"] + 1;
									}
								}
							}
						}
						else
						{
							//在AD的User中彻底不存在
							row.Delete();
						}
					}
				}
			}
		}

		private static void CompareAndAddOuUsersData(DataTable srcTable, DataTable targetTable, DataView compareView, AD2DBTransferContext context)
		{
			//提出要添加的行
			foreach (DataRow dr in srcTable.Rows)
			{
				string userGuid = dr["USER_GUID"].ToString();
				string parentID = dr["PARENT_GUID"].ToString();

				DataRowView[] drv = compareView.FindRows(new string[] { userGuid, parentID });

				if (drv.Length == 0)
				{
					//添加新纪录
					DataRow newdr = targetTable.NewRow();

					UpdateSort(dr, context);

					DataRowView[] sameUsersInfo = context.OriginalOUUserUserGuidView.FindRows(userGuid);

					//将和新插入记录相同GUID的用户都设置为兼职
					Array.ForEach(sameUsersInfo, d => d["SIDELINE"] = 1);

					AD2DBHelper.CopyDataRow(dr, newdr,
						"USER_GUID", "PARENT_GUID", "ALL_PATH_NAME", "DISPLAY_NAME", "OBJ_NAME", "STATUS", "ORIGINAL_SORT",
						"GLOBAL_SORT", "RANK_NAME", "DESCRIPTION", "ATTRIBUTES", "SEARCH_NAME", "INNER_SORT");

					targetTable.Rows.Add(newdr);
				}
			}
		}

		private static void CompareAndUpdateOuUsersData(DataRowView drv, DataRow dr)
		{
			//比较并修改原来的记录
			if (CompareOuUserIsDifferent(drv, dr))
			{
				AD2DBHelper.CopyDataRow(drv.Row, dr,
							"DESCRIPTION",
							"DISPLAY_NAME",
							"OBJ_NAME",
							"ALL_PATH_NAME",
							"RANK_NAME",
							"SEARCH_NAME",
							"STATUS",
							"INNER_SORT",
							"ORIGINAL_SORT",
							"GLOBAL_SORT",
							"SIDELINE");
			}
		}

		private static bool CompareOuUserIsDifferent(DataRowView newdrv, DataRow olddr)
		{
			return AD2DBHelper.CompareDataRowIsDifferent(newdrv, olddr,
				"RANK_NAME", "DISPLAY_NAME", "OBJ_NAME", "DESCRIPTION", "ALL_PATH_NAME", "SEARCH_NAME", "STATUS", "INNER_SORT", "ORIGINAL_SORT", "GLOBAL_SORT");
		}

		public override void DeleteOperation()
		{
			//删除关系表
			DataView ou_users = new DataView(this.Context.OriginalData.Tables["OU_USERS"], string.Empty, "PARENT_GUID,USER_GUID", DataViewRowState.Deleted);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);
			foreach (DataRowView drv in ou_users)
			{
				try
				{
					string ouuser_delete = string.Format("DELETE OU_USERS WHERE PARENT_GUID = '{0}' and  USER_GUID = '{1}'", drv["PARENT_GUID"].ToString(), drv["USER_GUID"].ToString());
					int count = db.ExecuteNonQuery(CommandType.Text, ouuser_delete);

					if (count == 1)
					{
						this.DeleteCount++;
						this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0},USER_GUID是{1}的记录删除成功，ALL_PATH_NAME是'{2}'",
						drv["PARENT_GUID"].ToString(), drv["USER_GUID"].ToString(), drv["ALL_PATH_NAME"]).ToString());
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0},USER_GUID 是{1}的记录执行删除时出错,错误是{2}",
						drv["PARENT_GUID"].ToString(), drv["USER_GUID"].ToString(), ex.Message));
				}
			}

			//改为逻辑删除
			//DataView ou_users = new DataView(this.Context.OriginalData.Tables["OU_USERS"], string.Empty, "PARENT_GUID,USER_GUID", DataViewRowState.ModifiedCurrent);
			//Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);
			//foreach (DataRowView drv in ou_users)
			//{
			//    UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

			//    uBuilder.AppendItem("PARENT_GUID", "950e9a2e-c1e0-4c70-a0f2-0f7c668ce48d");
			//    uBuilder.AppendItem("ALL_PATH_NAME", "机构人员\\离职人员\\" + drv["OBJ_NAME"].ToString() + drv["USER_GUID"].ToString());
			//    uBuilder.AppendItem("OUSYSCONTENT2", "机构人员\\离职人员\\" + drv["OBJ_NAME"].ToString() + drv["USER_GUID"].ToString());
			//    uBuilder.AppendItem("SEARCH_NAME", "");
			//    uBuilder.AppendItem("END_TIME", "2000-01-01 00:00:00.000");
			//    uBuilder.AppendItem("MODIFY_TIME", TSqlBuilder.Instance.DBCurrentTimeFunction, "=", true);

			//    WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

			//    wBuilder.AppendItem("PARENT_GUID", drv["PARENT_GUID"].ToString());
			//    wBuilder.AppendItem("USER_GUID", drv["USER_GUID"].ToString());

			//    string delsql = string.Format("UPDATE OU_USERS SET {0} where {1}", uBuilder.ToSqlString(TSqlBuilder.Instance), wBuilder.ToSqlString(TSqlBuilder.Instance));

			//    try
			//    {
			//        int count = db.ExecuteNonQuery(CommandType.Text, delsql);

			//        if (count == 1)
			//        {
			//            this.UpdateCount++;
			//        }
			//    }
			//    catch (Exception ex)
			//    {
			//        this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0}USER_GUID 是{1}的记录执行删除时出错,错误是{2}",
			//            drv["PARENT_GUID"].ToString(), drv["USER_GUID"].ToString(), ex.Message));
			//    }
			//}
		}

		public override void AddOperation()
		{
			//添加用户表
			DataView ouusers = new DataView(this.Context.OriginalData.Tables["OU_USERS"], string.Empty, "PARENT_GUID,USER_GUID", DataViewRowState.Added);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView dr in ouusers)
			{
				InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

				builder.AppendItem("USER_GUID", dr["USER_GUID"].ToString());
				builder.AppendItem("PARENT_GUID", dr["PARENT_GUID"].ToString());
				builder.AppendItem("ALL_PATH_NAME", dr["ALL_PATH_NAME"].ToString());
				builder.AppendItem("DISPLAY_NAME", dr["DISPLAY_NAME"].ToString());

				builder.AppendItem("OBJ_NAME", dr["OBJ_NAME"].ToString());
				builder.AppendItem("STATUS", dr["STATUS"].ToString());
				builder.AppendItem("ORIGINAL_SORT", dr["ORIGINAL_SORT"].ToString());
				builder.AppendItem("GLOBAL_SORT", dr["GLOBAL_SORT"].ToString());
				builder.AppendItem("RANK_NAME", dr["RANK_NAME"].ToString());
				builder.AppendItem("DESCRIPTION", dr["DESCRIPTION"].ToString());
				builder.AppendItem("ATTRIBUTES", Convert.ToInt32(dr["ATTRIBUTES"]));
				builder.AppendItem("SEARCH_NAME", dr["SEARCH_NAME"]);

				string sql = string.Format("INSERT INTO OU_USERS {0}", builder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, sql);

					if (count == 1)
					{
						this.AddCount++;
						//this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0},USER_GUID是{1}的记录添加成功，ALL_PATH_NAME是'{2}'",
						//dr["PARENT_GUID"].ToString(), dr["USER_GUID"].ToString(), dr["ALL_PATH_NAME"]).ToString());
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0},USER_GUID 是{1}的记录执行插入时出错,错误是{2}",
						dr["PARENT_GUID"].ToString(), dr["USER_GUID"].ToString(), ex.Message));
				}
			}
		}

		public override void UpdateOperation()
		{
			DataView ouusers = new DataView(this.Context.OriginalData.Tables["OU_USERS"], string.Empty, "PARENT_GUID,USER_GUID", DataViewRowState.ModifiedCurrent);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);
			foreach (DataRowView drs in ouusers)
			{
				UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

				uBuilder.AppendItem("DISPLAY_NAME", drs["DISPLAY_NAME"].ToString());
				uBuilder.AppendItem("OBJ_NAME", drs["OBJ_NAME"].ToString());
				uBuilder.AppendItem("ALL_PATH_NAME", drs["ALL_PATH_NAME"].ToString());
				uBuilder.AppendItem("INNER_SORT", drs["INNER_SORT"].ToString());

				uBuilder.AppendItem("GLOBAL_SORT", drs["GLOBAL_SORT"].ToString());
				uBuilder.AppendItem("ORIGINAL_SORT", drs["ORIGINAL_SORT"].ToString());
				uBuilder.AppendItem("SEARCH_NAME", drs["SEARCH_NAME"].ToString());
				uBuilder.AppendItem("MODIFY_TIME", TSqlBuilder.Instance.DBCurrentTimeFunction, "=", true);
                uBuilder.AppendItem("STATUS", drs["STATUS"].ToString());//by majun 20120216

				WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

				wBuilder.AppendItem("PARENT_GUID", drs["PARENT_GUID"].ToString());
				wBuilder.AppendItem("USER_GUID", drs["USER_GUID"].ToString());
                //wBuilder.AppendItem("STATUS", drs["STATUS"].ToString());//by majun 20120216

				string updatesql = string.Format("UPDATE OU_USERS SET {0} where {1}", uBuilder.ToSqlString(TSqlBuilder.Instance), wBuilder.ToSqlString(TSqlBuilder.Instance));

				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, updatesql);

					if (count == 1)
					{
						this.UpdateCount++;
						//this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0},USER_GUID是{1}的记录更新成功，ALL_PATH_NAME是'{2}'",
						//drs["PARENT_GUID"].ToString(), drs["USER_GUID"].ToString(), drs["ALL_PATH_NAME"]).ToString());
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("OU_USERS表 PARENT_GUID是{0}USER_GUID 是{1}的记录执行更新时出错,错误是{2}",
						drs["PARENT_GUID"].ToString(), drs["USER_GUID"].ToString(), ex.Message));
				}

			}
		}
	}
}
