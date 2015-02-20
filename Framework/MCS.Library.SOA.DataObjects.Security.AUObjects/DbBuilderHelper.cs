using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	internal static class DbBuilderHelper
	{
		internal static WhereSqlClauseBuilder NormalFor(this WhereSqlClauseBuilder instance, string fieldName)
		{
			instance.AppendItem(fieldName, (int)SchemaObjectStatus.Normal);
			return instance;
		}

		internal static WhereSqlClauseBuilder StatusFor(this WhereSqlClauseBuilder instance, string fieldName, SchemaObjectStatus status)
		{
			instance.AppendItem(fieldName, (int)status);
			return instance;
		}

		internal static WhereSqlClauseBuilder AppendCondition<T>(this WhereSqlClauseBuilder instance, string fieldName, T value)
		{
			instance.AppendItem(fieldName, value);
			return instance;
		}

		internal static InSqlClauseBuilder In<T>(this InSqlClauseBuilder instance, params  T[] values)
		{
			instance.AppendItem(values);

			return instance;
		}

		internal static IConnectiveSqlClause In<T>(string fieldName, params T[] values)
		{
			if (values.Length == 1)
				return new WhereSqlClauseBuilder().AppendCondition(fieldName, values[0]);
			return
				new InSqlClauseBuilder(fieldName).In(values);
		}
	}
}
