using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal class OrginalOguDataOperation : OriginalDataTableOperationBase
	{
		public OrginalOguDataOperation(AD2DBTransferContext ctx)
			: base(ctx)
		{
		}

		public override void CompareAndModifyData()
		{
			CompareAndDeleteAndUpdateOguData(this.Context);

			CompareAndAddOguData(this.Context.ADData.Tables["ORGANIZATIONS"], this.Context.OriginalData.Tables["ORGANIZATIONS"],
				this.Context.OriginalOguGuidView, this.Context);
		}

		private static void CompareAndDeleteAndUpdateOguData(AD2DBTransferContext context)
		{
			foreach (DataRow row in context.OriginalData.Tables["ORGANIZATIONS"].Rows)
			{
				string guid = row["GUID"].ToString();
				int index = context.ADOguGuidView.Find(guid);

				if (index == -1)
				{
					row.Delete();
					DataRowView[] drvs = context.OriginalOuUserParentGuidView.FindRows(guid);

					Array.ForEach(drvs, drv => drv.Row.Delete());
				}
				else
				{
					CompareAndUpdateOguData(context.ADOguGuidView[index], row);
				}
			}
		}

		private static void CompareAndAddOguData(DataTable srcTable, DataTable targetTable, DataView compareView, AD2DBTransferContext context)
		{
			//提出要添加的行
			foreach (DataRow dr in srcTable.Rows)
			{
				int index = compareView.Find(dr["GUID"]);

				if (index == -1)
				{
					//添加新纪录
					DataRow newdr = targetTable.NewRow();

					UpdateSort(dr, context);
					AD2DBHelper.CopyDataRow(dr, newdr,
						"GUID", "PARENT_GUID", "OBJ_NAME", "DISPLAY_NAME", "DESCRIPTION", "DESCRIPTION",
						"ALL_PATH_NAME", "ORIGINAL_SORT", "CHILDREN_COUNTER", "GLOBAL_SORT", "RANK_CODE",
						"INNER_SORT", "ORG_TYPE", "ORG_CLASS", "STATUS", "SEARCH_NAME");

					targetTable.Rows.Add(newdr);
				}
			}
		}

		private static void CompareAndUpdateOguData(DataRowView drv, DataRow dr)
		{
			//比较并修改原来的记录
			if (CompareOrganizationIsDifferent(drv, dr))
			{
				AD2DBHelper.CopyDataRow(drv.Row, dr,
							"PARENT_GUID",
							"DISPLAY_NAME",
							"OBJ_NAME",
							"ALL_PATH_NAME",
							"SEARCH_NAME",
							"ORIGINAL_SORT",
							"CHILDREN_COUNTER",
							"GLOBAL_SORT");
			}
		}

		/// <summary>
		/// 比较组织表的字段 true 代表不同 false 代表相同
		/// </summary>
		private static bool CompareOrganizationIsDifferent(DataRowView newdrv, DataRow olddr)
		{
			return AD2DBHelper.CompareDataRowIsDifferent(newdrv, olddr,
				"PARENT_GUID", "DISPLAY_NAME", "OBJ_NAME", "ALL_PATH_NAME", "SEARCH_NAME", "ORIGINAL_SORT", "GLOBAL_SORT", "CHILDREN_COUNTER");
		}

		public override void DeleteOperation()
		{
			//1删除组织表
			DataView organizations = new DataView(this.Context.OriginalData.Tables["ORGANIZATIONS"], string.Empty, "GUID", DataViewRowState.Deleted);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView drv in organizations)
			{
				try
				{
					string organization_delete = string.Format("DELETE ORGANIZATIONS WHERE GUID = '{0}'", drv["GUID"].ToString());
					int count = db.ExecuteNonQuery(CommandType.Text, organization_delete);

					if (count > 0)
					{
						this.DeleteCount++;
						this.Context.InitialParams.Log.Write(string.Format("ORGANIZATIONS表GUID 是{0}的记录删除成功，ALL_PATH_NAME是'{1}'",
						drv["GUID"].ToString(), drv["ALL_PATH_NAME"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("ORGANIZATIONS表GUID 是{0}的记录执行删除时出错,错误是{1}",
						drv["GUID"].ToString(), ex.Message));
				}
			}
		}

		public override void AddOperation()
		{
			DataView organizations = new DataView(this.Context.OriginalData.Tables["ORGANIZATIONS"], string.Empty, "GUID", DataViewRowState.Added);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView drs in organizations)
			{
				InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

				builder.AppendItem("GUID", drs["GUID"].ToString());
				builder.AppendItem("PARENT_GUID", drs["PARENT_GUID"].ToString());
				builder.AppendItem("OBJ_NAME", drs["OBJ_NAME"].ToString());
				builder.AppendItem("DISPLAY_NAME", drs["DISPLAY_NAME"].ToString());

				builder.AppendItem("DESCRIPTION", drs["DESCRIPTION"].ToString());
				builder.AppendItem("ALL_PATH_NAME", drs["ALL_PATH_NAME"].ToString());
				builder.AppendItem("ORIGINAL_SORT", drs["ORIGINAL_SORT"].ToString());
				builder.AppendItem("CHILDREN_COUNTER", drs["CHILDREN_COUNTER"].ToString());
				builder.AppendItem("GLOBAL_SORT", drs["GLOBAL_SORT"].ToString());
				builder.AppendItem("RANK_CODE", drs["RANK_CODE"].ToString());
				builder.AppendItem("INNER_SORT", drs["INNER_SORT"].ToString());
				builder.AppendItem("ORG_TYPE", Convert.ToInt32(drs["ORG_TYPE"]));
				builder.AppendItem("ORG_CLASS", Convert.ToInt32(drs["ORG_CLASS"]));
				builder.AppendItem("STATUS", Convert.ToInt32(drs["STATUS"]));
				builder.AppendItem("SEARCH_NAME", drs["SEARCH_NAME"]);

				string sql = string.Format("INSERT INTO ORGANIZATIONS {0}", builder.ToSqlString(TSqlBuilder.Instance));
				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, sql);

					if (count == 1)
					{
						this.AddCount++;
						//this.Context.InitialParams.Log.Write(string.Format("ORGANIZATIONS表GUID 是{0}的记录添加成功，ALL_PATH_NAME是'{1}'",
						//drs["GUID"].ToString(), drs["ALL_PATH_NAME"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("ORGANIZATIONS表GUID 是{0}的记录执行插入时出错,错误是{1}",
						drs["GUID"].ToString(), ex.Message));
				}
			}
		}

		public override void UpdateOperation()
		{
			DataView organizations = new DataView(this.Context.OriginalData.Tables["ORGANIZATIONS"], string.Empty, "GUID", DataViewRowState.ModifiedCurrent);
			Database db = DatabaseFactory.Create(this.Context.InitialParams.AccreditAdminConnectionName);

			foreach (DataRowView drs in organizations)
			{
				UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

				uBuilder.AppendItem("PARENT_GUID", drs["PARENT_GUID"].ToString());
				uBuilder.AppendItem("DISPLAY_NAME", drs["DISPLAY_NAME"].ToString());
				uBuilder.AppendItem("OBJ_NAME", drs["OBJ_NAME"].ToString());
				uBuilder.AppendItem("ALL_PATH_NAME", drs["ALL_PATH_NAME"].ToString());
				uBuilder.AppendItem("INNER_SORT", drs["INNER_SORT"].ToString());
				uBuilder.AppendItem("GLOBAL_SORT", drs["GLOBAL_SORT"].ToString());
				uBuilder.AppendItem("ORIGINAL_SORT", drs["ORIGINAL_SORT"].ToString());
				uBuilder.AppendItem("SEARCH_NAME", drs["SEARCH_NAME"].ToString());

				uBuilder.AppendItem("CHILDREN_COUNTER", (int)drs["CHILDREN_COUNTER"]);
				uBuilder.AppendItem("MODIFY_TIME", TSqlBuilder.Instance.DBCurrentTimeFunction, "=", true);

				WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();
				wBuilder.AppendItem("GUID", drs["GUID"].ToString());

				string updatesql = string.Format("UPDATE ORGANIZATIONS SET {0} where {1}", uBuilder.ToSqlString(TSqlBuilder.Instance), wBuilder.ToSqlString(TSqlBuilder.Instance));

				try
				{
					int count = db.ExecuteNonQuery(CommandType.Text, updatesql);

					if (count == 1)
					{
						this.UpdateCount++;
						//this.Context.InitialParams.Log.Write(string.Format("ORGANIZATIONS表GUID 是{0}的记录更新成功，ALL_PATH_NAME是'{1}'",
						//drs["GUID"].ToString(), drs["ALL_PATH_NAME"].ToString()));
					}
				}
				catch (Exception ex)
				{
					this.Context.InitialParams.Log.Write(string.Format("组织表GUID 是{0}的记录执行更新时出错,错误是{1}",
						drs["GUID"].ToString(), ex.Message));
				}
			}
		}
	}
}
