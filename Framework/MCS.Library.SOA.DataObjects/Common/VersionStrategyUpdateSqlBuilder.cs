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
	/// 与时间版本信息处理相关的Update和Insert语句的构造器
	/// </summary>
	public class VersionStrategyUpdateSqlBuilder<T> where T : IVersionDataObject
	{
		public string ToUpdateSql(T obj, ORMappingItemCollection mapping)
		{
			return VersionStrategyUpdateSqlHelper.ConstructUpdateSql(null, (strB, context) =>
			{
				if (obj.VersionStartTime != DateTime.MinValue)
				{
					strB.Append(PrepareUpdateSql(obj, mapping));

					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

					strB.AppendFormat("IF @@ROWCOUNT > 0\n");
					strB.AppendFormat("\t{0}\n", PrepareInsertSql(obj, mapping));
					strB.AppendFormat("ELSE\n");
					strB.AppendFormat("\tRAISERROR ({0}, 16, 1)",
						TSqlBuilder.Instance.CheckUnicodeQuotationMark(string.Format("对象\"{0}\"的版本不是最新的，不能更新", obj.ID)));
				}
				else
				{
					strB.Append(PrepareInsertSql(obj, mapping));
				}
			});
		}

		protected virtual string GetTableName(T obj, ORMappingItemCollection mapping)
		{
			return mapping.TableName;
		}

		protected virtual InsertSqlClauseBuilder PrepareInsertSqlBuilder(T obj, ORMappingItemCollection mapping)
		{
			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(obj, mapping);

			string startTimeFieldName = GetPropertyFieldName("VersionStartTime", mapping);
			string endTimeFieldName = GetPropertyFieldName("VersionEndTime", mapping);

			builder.Remove(b => ((SqlClauseBuilderItemIUW)b).DataField == startTimeFieldName);
			builder.Remove(b => ((SqlClauseBuilderItemIUW)b).DataField == endTimeFieldName);

			builder.AppendItem(startTimeFieldName, "@currentTime", "=", true);
			builder.AppendItem(endTimeFieldName, ConnectionDefine.MaxVersionEndTime);

			return builder;
		}

		protected virtual UpdateSqlClauseBuilder PrepareUpdateSqlBuilder(T obj, ORMappingItemCollection mapping)
		{
			UpdateSqlClauseBuilder updateBuilder = new UpdateSqlClauseBuilder();

			updateBuilder.AppendItem(GetPropertyFieldName("VersionEndTime", mapping), "@currentTime", "=", true);

			return updateBuilder;
		}

		protected virtual WhereSqlClauseBuilder PrepareWhereSqlBuilder(T obj, ORMappingItemCollection mapping)
		{
			WhereSqlClauseBuilder primaryKeyBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(obj);

			string vsFieldName = GetPropertyFieldName("VersionStartTime", mapping);

			if (primaryKeyBuilder.Exists(item => ((SqlClauseBuilderItemIUW)item).DataField == vsFieldName) == false)
				primaryKeyBuilder.AppendItem(vsFieldName, obj.VersionStartTime);

			return primaryKeyBuilder;
		}

		protected virtual string PrepareInsertSql(T obj, ORMappingItemCollection mapping)
		{
			InsertSqlClauseBuilder builder = PrepareInsertSqlBuilder(obj, mapping);

			return string.Format("INSERT INTO {0}{1}", GetTableName(obj, mapping), builder.ToSqlString(TSqlBuilder.Instance));
		}

		protected virtual string PrepareUpdateSql(T obj, ORMappingItemCollection mapping)
		{
			WhereSqlClauseBuilder primaryKeyBuilder = PrepareWhereSqlBuilder(obj, mapping);
			UpdateSqlClauseBuilder updateBuilder = PrepareUpdateSqlBuilder(obj, mapping);

			return string.Format("UPDATE {0} SET {1} WHERE {2}",
					GetTableName(obj, mapping),
					updateBuilder.ToSqlString(TSqlBuilder.Instance),
					primaryKeyBuilder.ToSqlString(TSqlBuilder.Instance));
		}

		protected static string GetPropertyFieldName(string propertyName, ORMappingItemCollection mapping)
		{
			ORMappingItem item = mapping[propertyName];

			(item != null).FalseThrow("不能在{0}的OR Mapping信息中找到属性{1}", mapping.TableName, propertyName);

			return item.DataFieldName;
		}
	}
}
