using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	public class MaterialContentAdapter : UpdatableAndLoadableAdapterBase<MaterialContent, MaterialContentCollection>
	{
		public static readonly MaterialContentAdapter Instance = new MaterialContentAdapter();

		private MaterialContentAdapter()
		{
		}

		public void Update(MaterialContent data, Stream stream)
		{
			data.NullCheck("data");
			stream.NullCheck("stream");

			Dictionary<string, object> context = new Dictionary<string, object>();

			context["Stream"] = stream;

			this.BeforeInnerUpdate(data, context);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (this.InnerUpdate(data, context) == 0)
					this.InnerInsert(data, context);

				AfterInnerUpdate(data, context);

				scope.Complete();
			}
		}

		protected override int InnerUpdate(MaterialContent data, Dictionary<string, object> context)
		{
			int result = 0;

			ORMappingItemCollection mappings = GetMappingInfo(context);

			string sql = string.Format("SELECT COUNT(*) FROM {0} WHERE {1}",
				mappings.TableName,
				ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data, mappings).ToSqlString(TSqlBuilder.Instance));

			result = (int)DbHelper.RunSqlReturnScalar(sql, GetConnectionName());

			if (result > 0)
			{
				Stream stream = null;

				if (context.ContainsKey("Stream"))
					stream = (Stream)context["Stream"];

				if (stream == null)
					result = base.InnerUpdate(data, context);
				else
					result = InnerUpdateWithStream(data, stream, context);
			}

			return result;
		}

		protected override void InnerInsert(MaterialContent data, Dictionary<string, object> context)
		{
			Stream stream = null;

			if (context.ContainsKey("Stream"))
				stream = (Stream)context["Stream"];

			if (stream == null)
				base.InnerInsert(data, context);
			else
				InnerInsertWithStream(data, stream, context);
		}

		protected override string GetConnectionName()
		{
			string connectionName = MaterialContentSettings.GetConfig().ConnectionName;

			if (connectionName.IsNullOrEmpty())
				connectionName = WorkflowSettings.GetConfig().ConnectionName;

			return connectionName;
		}

		private int InnerUpdateWithStream(MaterialContent data, Stream stream, Dictionary<string, object> context)
		{
			ORMappingItemCollection mappings = GetMappingInfo(context);

			UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, "ContentData");

			uBuilder.AppendItem("CONTENT_DATA", stream);

			WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data);

			string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
				mappings.TableName,
				uBuilder.ToSqlString(TSqlBuilder.Instance),
				wBuilder.ToSqlString(TSqlBuilder.Instance));

			DbHelper.RunSql(sql, GetConnectionName());

			return 1;
		}

		private void InnerInsertWithStream(MaterialContent data, Stream stream, Dictionary<string, object> context)
		{
			ORMappingItemCollection mappings = GetMappingInfo(context);

			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(data, "ContentData");

			builder.AppendItem("CONTENT_DATA", stream);

			string sql = string.Format("INSERT INTO {0}{1} ",
				mappings.TableName,
				builder.ToSqlString(TSqlBuilder.Instance));

			DbHelper.RunSql(sql, GetConnectionName());
		}
	}
}
