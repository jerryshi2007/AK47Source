using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// 带版本信息的更新类SQL语句的Helper类
	/// </summary>
	public static class VersionStrategyUpdateSqlHelper
	{
		/// <summary>
		/// 根据上下文，构造初始化@currentTime的SQL语句块，然后调用action，来构造中间的语句
		/// </summary>
		/// <param name="context"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static string ConstructUpdateSql(object context, Action<StringBuilder, object> action)
		{
			StringBuilder strB = new StringBuilder();

			string currentTimeSql = TSqlBuilder.Instance.DBCurrentTimeFunction;

			DBTimePointActionContext.Current.TimePoint.IsNotMinValue(tp => currentTimeSql = TSqlBuilder.Instance.FormatDateTime(tp));

			strB.Append("DECLARE @currentTime DATETIME");
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			strB.AppendFormat("SET @currentTime = {0}", currentTimeSql);
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);


			if (action != null)
			{
				action(strB, context);
			}

			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			strB.Append("SELECT @currentTime");

			return strB.ToString();
		}
	}
}
