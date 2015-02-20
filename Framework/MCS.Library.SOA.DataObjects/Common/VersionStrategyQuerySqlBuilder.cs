using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// 带版本的SQL查询构造器
	/// </summary>
	public class VersionStrategyQuerySqlBuilder
	{
		public static readonly VersionStrategyQuerySqlBuilder Instance = new VersionStrategyQuerySqlBuilder();

		private string _VersionStartTimeFieldName = "VersionStartTime";
		private string _VersionEndTimeFieldName = "VersionEndTime";
		private bool _UseSimulatedTime = true;

		/// <summary>
		/// 构造语句的时候，当传递的TimePoint为空时，是否自动使用仿真时间。
		/// </summary>
		public bool UseSimulatedTime
		{
			get
			{
				return this._UseSimulatedTime;
			}
			set
			{
				this._UseSimulatedTime = value;
			}
		}

		/// <summary>
		/// 版本开始时间的字段名
		/// </summary>
		public string VersionStartTimeFieldName
		{
			get
			{
				return this._VersionStartTimeFieldName;
			}
			set
			{
				this._VersionStartTimeFieldName = value;
			}
		}

		/// <summary>
		/// 版本结束时间的字段名
		/// </summary>
		public string VersionEndTimeFieldName
		{
			get
			{
				return this._VersionEndTimeFieldName;
			}
			set
			{
				this._VersionEndTimeFieldName = value;
			}
		}

		/// <summary>
		/// 将时间点有关的SQL条件填充到ConnectiveSqlClauseCollection
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="timePoint"></param>
		/// <param name="fieldPrefix">字段前缀。如果是表空间或Schema，不要忘记后面的'.'</param>
		public void FillTimePointInBuilder(ConnectiveSqlClauseCollection builder, DateTime timePoint, string fieldPrefix)
		{
			//输出结果应该是
			//VersionStartTime <= getdate() AND (VersionEndTime > getdate() OR VersionEndTime IS NULL)
			builder.NullCheck("builder");

			string timeString = TSqlBuilder.Instance.DBCurrentTimeFunction;

			if (timePoint == DateTime.MinValue && this._UseSimulatedTime && TimePointContext.Current.UseCurrentTime == false)
				timePoint = TimePointContext.Current.SimulatedTime;

			if (timePoint != DateTime.MinValue)
				timeString = TSqlBuilder.Instance.FormatDateTime(timePoint);

			WhereSqlClauseBuilder sBuilder = new WhereSqlClauseBuilder();

			string vsFieldName = JoinFieldName(fieldPrefix, this.VersionStartTimeFieldName);

			sBuilder.AppendItem(vsFieldName, timeString, "<=", true);

			WhereSqlClauseBuilder eBuilder = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);

			string veFieldName = JoinFieldName(fieldPrefix, this.VersionEndTimeFieldName);

			eBuilder.AppendItem(veFieldName, timeString, ">", true).AppendItem(veFieldName, "NULL", "IS", true);

			builder.LogicOperator = LogicOperatorDefine.And;

			builder.Add(sBuilder).Add(eBuilder);
		}

		/// <summary>
		/// 生成和时间点有关的SQL条件。会调用FillTimePointInBuilder
		/// </summary>
		/// <returns></returns>
		public ConnectiveSqlClauseCollection TimePointToBuilder()
		{
			return TimePointToBuilder(DateTime.MinValue, string.Empty);
		}

		/// <summary>
		/// 生成和时间点有关的SQL条件。会调用FillTimePointInBuilder
		/// </summary>
		/// <param name="fieldPrefix">字段前缀。如果是Schema或表空间名称，需要架上.</param>
		/// <returns></returns>
		public ConnectiveSqlClauseCollection TimePointToBuilder(string fieldPrefix)
		{
			return TimePointToBuilder(DateTime.MinValue, fieldPrefix);
		}

		/// <summary>
		/// 生成和时间点有关的SQL条件。会调用FillTimePointInBuilder
		/// </summary>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public ConnectiveSqlClauseCollection TimePointToBuilder(DateTime timePoint)
		{
			return TimePointToBuilder(timePoint, string.Empty);
		}

		/// <summary>
		/// 生成和时间点有关的SQL条件。会调用FillTimePointInBuilder
		/// </summary>
		/// <param name="timePoint">如果是DateTime.MinValue，则表示使用当前时间</param>
		/// <param name="fieldPrefix"></param>
		/// <returns></returns>
		public ConnectiveSqlClauseCollection TimePointToBuilder(DateTime timePoint, string fieldPrefix)
		{
			ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection();

			FillTimePointInBuilder(builder, timePoint, fieldPrefix);

			return builder;
		}

		private static string JoinFieldName(string fieldPrefix, string fieldName)
		{
			string result = fieldName;

			if (fieldPrefix.IsNotEmpty())
				result = fieldPrefix + fieldName;

			return result;
		}
	}
}
