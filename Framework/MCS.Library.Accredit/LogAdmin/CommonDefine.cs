using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Transactions;
using System.Diagnostics;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.LogAdmin
{
	internal class CommonDefine
	{
		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次操作所影响的数据条数</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
#if DEBUG
				Debug.WriteLine(strSql);
#endif
				ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strSql), "数据处理语句SQL不能为空串！");

				return database.ExecuteNonQuery(CommandType.Text, strSql);
			}
		}

		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <param name="strTables">要求配置的数据表名称</param>
		/// <returns>本次查询的数据结果集</returns>
		internal static DataSet ExecuteDataset(string strSql, params string[] strTables)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteDatasetWithoutTransaction--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteDataSet(CommandType.Text, strSql, strTables);
			}
		}

		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次查询的结果对象</returns>
		internal static object ExecuteScalar(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteScalar--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}
	}
}
