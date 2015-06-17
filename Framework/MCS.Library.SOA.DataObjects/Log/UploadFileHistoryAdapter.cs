using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.IO;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
	public class UploadFileHistoryAdapter
	{
		public static readonly UploadFileHistoryAdapter Instance = new UploadFileHistoryAdapter();

		private UploadFileHistoryAdapter()
		{
		}

		public UploadFileHistory Load(int id)
		{
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("ID", id);
            builder.AppendTenantCode(typeof(UploadFileHistory));

            string sql = string.Format("SELECT * FROM WF.UPLOAD_FILE_HISTORY WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, AppLogSettings.GetConfig().ConnectionName).Tables[0];

			ExceptionHelper.FalseThrow(table.Rows.Count > 0, "不能找到ID为{0}的UploadFileHistory的数据");

			UploadFileHistory history = new UploadFileHistory();

			ORMapping.DataRowToObject(table.Rows[0], history);

			return history;
		}

		public void Insert(UploadFileHistory history, Stream fileStream)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(fileStream != null, "fileStream");

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				Insert(history);

				MaterialContent content = PrepareMaterialContent(history, fileStream);

				MaterialContentAdapter.Instance.Update(content, fileStream);

				scope.Complete();
			}
		}

		public void Insert(UploadFileHistory history)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(history != null, "history");

			string sql = ORMapping.GetInsertSql(history, TSqlBuilder.Instance) + TSqlBuilder.Instance.DBStatementSeperator +
				"SELECT SCOPE_IDENTITY()";

			decimal identity = (decimal)DbHelper.RunSqlReturnScalar(sql, AppLogSettings.GetConfig().ConnectionName);

			history.ID = Decimal.ToInt32(identity);
		}

		private static MaterialContent PrepareMaterialContent(UploadFileHistory history, Stream fileStream)
		{
			MaterialContent content = new MaterialContent();

			content.ContentID = GetMaterialContentID(history);
			content.Creator = history.Operator;
			content.FileName = history.OriginalFileName;
			content.FileSize = fileStream.Length;
			content.RelativeID = content.ContentID;
			content.Class = "UploadFileHistory";

			return content;
		}

		internal static string GetMaterialContentID(UploadFileHistory history)
		{
			string result = string.Format("{0}-{1}-{2}", history.ID, history.ApplicationName, history.ProgramName);

			if (result.Length > 36)
				result = result.Substring(0, 36);

			return result;
		}
	}
}
