using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 表示模式定义的适配器，用于将配置文件中定义的Schema信息保存到数据库中
	/// </summary>
	public class SchemaDefineAdapter : UpdatableAndLoadableAdapterBase<SchemaDefine, SchemaDefineCollection>
	{
		/// <summary>
		/// 获取<see cref="SchemaDefineAdapter"/>的实例，此字段为只读。
		/// </summary>
		public static readonly SchemaDefineAdapter Instance = new SchemaDefineAdapter();

		private SchemaDefineAdapter()
		{
		}

		/// <summary>
		/// 在执行了更新之后进行其他操作。
		/// </summary>
		/// <param name="data"><see cref="SchemaDefine"/>对象。</param>
		/// <param name="context">一个<see cref="T:Dictionary^2"/>，表示操作上下文。</param>
		protected override void AfterInnerUpdate(SchemaDefine data, Dictionary<string, object> context)
		{
			StringBuilder sql = new StringBuilder();

			sql.Append(PrepareDeletePropertyDefineSql(data));

			foreach (SchemaPropertyDefine pd in data.Properties)
			{
				sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
				sql.Append(PrepareInsertPropertyDefineSql(data, pd));

				DbHelper.RunSql(sql.ToString(), this.GetConnectionName());
			}
		}

		/// <summary>
		/// 在执行了删除之后进行其他操作。
		/// </summary>
		/// <param name="data"><see cref="SchemaDefine"/>对象。</param>
		/// <param name="context">一个<see cref="T:Dictionary^2"/>，表示操作上下文。</param>
		protected override void AfterInnerDelete(SchemaDefine data, Dictionary<string, object> context)
		{
			DbHelper.RunSql(PrepareDeletePropertyDefineSql(data), this.GetConnectionName());
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		private static string PrepareInsertPropertyDefineSql(SchemaDefine data, SchemaPropertyDefine pd)
		{
			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(pd);

			builder.AppendItem("SchemaName", data.Name);

			return string.Format("INSERT INTO {0}{1}", ORMapping.GetMappingInfo(pd.GetType()).TableName, builder.ToSqlString(TSqlBuilder.Instance));
		}

		private static string PrepareDeletePropertyDefineSql(SchemaDefine data)
		{
			return string.Format("DELETE {0} WHERE SchemaName = {1}",
				ORMapping.GetMappingInfo(typeof(SchemaPropertyDefine)).TableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(data.Name));
		}
	}
}
