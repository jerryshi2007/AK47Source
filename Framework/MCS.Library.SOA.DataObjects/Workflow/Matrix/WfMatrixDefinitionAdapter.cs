using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfMatrixDefinitionAdapter : UpdatableAndLoadableAdapterBase<WfMatrixDefinition, WfMatrixDefinitionCollection>
	{
		internal static readonly string DELETE_SQL_CLAUSE = "DELETE WF.MATRIX_DEFINITION WHERE ";
		internal static readonly string DELETE_DIMENSION_SQL_CLAUSE = "DELETE WF.MATRIX_DIMENSION_DEFINITION WHERE ";
		public static readonly WfMatrixDefinitionAdapter Instance = new WfMatrixDefinitionAdapter();

		private WfMatrixDefinitionAdapter()
		{
		}

		public WfMatrixDefinition Load(string key)
		{
			key.CheckStringIsNullOrEmpty("key");

			WfMatrixDefinitionCollection mds = Load(builder => builder.AppendItem("DEF_KEY", key));

			(mds.Count > 0).FalseThrow("不能找到Key为{0}的授权矩阵定义", key);

			return mds[0];
		}

		/// <summary>
		/// 从缓存中得到权限矩阵定义
		/// </summary>
		/// <param name="mdKey"></param>
		/// <returns></returns>
		public WfMatrixDefinition Get(string mdKey)
		{
			mdKey.CheckStringIsNullOrEmpty("mdKey");

			WfMatrixDefinition result = WfMatrixDefinitionCache.Instance.GetOrAddNewValue(mdKey, (cache, key) =>
			{
				WfMatrixDefinition md = Load(key);

				MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

				cache.Add(key, md, dependency);

				return md;
			});

			return result;
		}

		protected override void AfterLoad(WfMatrixDefinitionCollection data)
		{
			if (data.Count == 0) return;

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			data.ForEach(md => builder.AppendItem(md.Key));

			string sql = string.Format("SELECT * FROM WF.MATRIX_DIMENSION_DEFINITION WHERE MATRIX_DEF_KEY {0} ORDER BY MATRIX_DEF_KEY, SORT_ORDER",
				builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				WfMatrixDimensionDefinition dd = new WfMatrixDimensionDefinition();

				ORMapping.DataRowToObject(row, dd);

				WfMatrixDefinition matrixDefinition = data.Find(md => string.Compare(md.Key, dd.MatrixKey, true) == 0);

				if (matrixDefinition != null)
					matrixDefinition.Dimensions.Add(dd);
			}
		}

		protected override void AfterInnerUpdate(WfMatrixDefinition data, Dictionary<string, object> context)
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("{0} MATRIX_DEF_KEY = {1}",
				DELETE_DIMENSION_SQL_CLAUSE,
				TSqlBuilder.Instance.CheckQuotationMark(data.Key, true));

			foreach (WfMatrixDimensionDefinition dd in data.Dimensions)
			{
				if (strB.Length > 0)
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.Append(ORMapping.GetInsertSql(dd, TSqlBuilder.Instance));
			}

			if (strB.Length > 0)
				DbHelper.RunSql(strB.ToString(), GetConnectionName());

			CacheNotifyData notifyData = new CacheNotifyData(typeof(WfMatrixDefinitionCache), data.Key, CacheNotifyType.Invalid);

			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
			MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		protected override void AfterInnerDelete(WfMatrixDefinition data, Dictionary<string, object> context)
		{
			string sql = string.Format("{0} MATRIX_DEF_KEY = {1}",
				DELETE_DIMENSION_SQL_CLAUSE,
				TSqlBuilder.Instance.CheckQuotationMark(data.Key, true));

			DbHelper.RunSql(sql, GetConnectionName());

			CacheNotifyData notifyData = new CacheNotifyData(typeof(WfMatrixDefinitionCache), data.Key, CacheNotifyType.Invalid);

			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
			MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		public int Delete(string[] defKeys)
		{
			StringBuilder strBuilder = new StringBuilder();
			foreach (var defKey in defKeys)
			{
				WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

				whereBuilder.AppendItem("DEF_KEY", defKey);
				strBuilder.Append(DELETE_SQL_CLAUSE);
				strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));
				strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

				whereBuilder.Clear();
				whereBuilder.AppendItem("MATRIX_DEF_KEY", defKey);
				strBuilder.Append(DELETE_DIMENSION_SQL_CLAUSE);
				strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));
				strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);
			}

			return DbHelper.RunSqlWithTransaction(strBuilder.ToString());
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
