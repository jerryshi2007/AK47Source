using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	/// <summary>
	/// 分类的适配器
	/// </summary>
	public sealed class SchemaCategoryAdapter
	{
		/// <summary>
		/// 表示<see cref="SchemaCategoryAdapter"/>的唯一实例。
		/// </summary>
		public static readonly SchemaCategoryAdapter Instance = new SchemaCategoryAdapter();

		private class UpdateBuilder : VersionStrategyUpdateSqlBuilder<AUSchemaCategory>
		{
			public static UpdateBuilder Instance = new UpdateBuilder();
		}

		private SchemaCategoryAdapter()
		{
		}

		private string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}

		/// <summary>
		/// 根据指定的ID载入当前时间的<see cref="AUSchemaCategory"/>。
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public AUSchemaCategory LoadByID(string id)
		{
			return LoadByID(id, DateTime.MinValue);
		}

		public AUSchemaCategory LoadByID(string id, DateTime timePoint)
		{
			var conditions = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ID", id);
			conditions.Add(where);
			AUSchemaCategory cate = null;

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					VersionedObjectAdapterHelper.Instance.FillData(ORMapping.GetMappingInfo(typeof(AUSchemaCategory)).TableName, conditions, this.GetConnectionName(),
							reader =>
							{
								if (reader.Read())
								{
									cate = new AUSchemaCategory();
									ORMapping.DataReaderToObject<AUSchemaCategory>(reader, cate);
								}
							});
				}
			});

			return cate;
		}

		AUSchemaCategoryCollection LoadTopCategories()
		{
			return LoadSubCategories(null);
		}

		public AUSchemaCategoryCollection LoadSubCategories(string superID)
		{
			return LoadSubCategories(superID, DateTime.MinValue);
		}

		public AUSchemaCategoryCollection LoadSubCategories(string superID, DateTime timePoint)
		{
			return LoadSubCategories(superID, true, timePoint);
		}

		public AUSchemaCategoryCollection LoadSubCategories(string superID, bool normalOnly, DateTime timePoint)
		{
			var conditions = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			if (string.IsNullOrEmpty(superID) == false)
				where.AppendItem("ParentID", superID);
			else
				where.AppendItem("ParentID", (string)null, "IS");

			if (normalOnly)
				where.NormalFor("Status");

			conditions.Add(where);
			AUSchemaCategoryCollection result = new AUSchemaCategoryCollection();

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					VersionedObjectAdapterHelper.Instance.FillData(ORMapping.GetMappingInfo(typeof(AUSchemaCategory)).TableName, conditions, this.GetConnectionName(),
							reader =>
							{
								ORMapping.DataReaderToCollection(result, reader);
							});
				}
			});

			return result;
		}

		public void UpdateCategory(AUSchemaCategory category)
		{
			if (category == null)
				throw new ArgumentNullException("category");

			string sql = UpdateBuilder.Instance.ToUpdateSql(category, this.GetMappingInfo());

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					DateTime point = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());
					category.VersionStartTime = point;
				}
			});
		}

		private ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<AUSchemaCategory>();
		}
	}
}
