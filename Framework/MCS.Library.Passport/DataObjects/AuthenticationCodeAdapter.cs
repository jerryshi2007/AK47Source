using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Globalization;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 认证码的适配器
	/// </summary>
	public class AuthenticationCodeAdapter
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly AuthenticationCodeAdapter Instance = new AuthenticationCodeAdapter();

		private AuthenticationCodeAdapter()
		{
		}

		/// <summary>
		/// 颁发认证码
		/// </summary>
		/// <param name="authenticationType"></param>
		/// <param name="codeLength"></param>
		/// <returns></returns>
		public AuthenticationCode IssueCode(string authenticationType, int codeLength)
		{
			AuthenticationCode result = AuthenticationCode.Create(authenticationType, codeLength);

			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(result);

			builder.AppendItem("EXPIRE_TIME", string.Format("DATEADD(second, {0}, GETDATE())", 1800), "=", true);

			string sql = string.Format("INSERT INTO {0} {1}", this.GetMappingInfo().TableName, builder.ToSqlString(TSqlBuilder.Instance));

			ExecuteNonQuery(sql);

			return result;
		}

		/// <summary>
		/// 重发一个新的Code。
		/// </summary>
		/// <param name="authenticationID"></param>
		/// <param name="codeLength"></param>
		/// <returns>重新生成的Code</returns>
		public string ReissueCode(string authenticationID, int codeLength)
		{
			string result = AuthenticationCode.GenerateCode(codeLength);

			UpdateSqlClauseBuilder builder = new UpdateSqlClauseBuilder();

			builder.AppendItem("AUTHENTICATION_CODE", result);
			builder.AppendItem("EXPIRE_TIME", string.Format("DATEADD(second, {0}, GETDATE())", 1800), "=", true);

			string sql = string.Format("UPDATE {0} SET {1} WHERE AUTHENTICATION_ID = {2}",
				this.GetMappingInfo().TableName,
				builder.ToSqlString(TSqlBuilder.Instance),
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(authenticationID));

			ExecuteNonQuery(sql);

			return result;
		}

		/// <summary>
		/// 拿到已存在的验证码信息，如果不存在，返回NULL
		/// </summary>
		/// <param name="authenticationID"></param>
		/// <returns></returns>
		public AuthenticationCode Load(string authenticationID)
		{
			authenticationID.CheckStringIsNullOrEmpty("AUTHENTICATION_ID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("AUTHENTICATION_ID", authenticationID);

			string sql = string.Format("SELECT *, CASE WHEN EXPIRE_TIME > GETDATE() THEN 1 ELSE 0 END IS_VALID FROM {0} WHERE {1}",
				this.GetMappingInfo().TableName, builder.ToSqlString(TSqlBuilder.Instance));

			return this.Query(sql);
		}

		/// <summary>
		/// 修改过期时间。主要用于测试
		/// </summary>
		/// <param name="authenticationID"></param>
		/// <param name="expireTime"></param>
		public void UpdateExpireTime(string authenticationID, DateTime expireTime)
		{
			UpdateSqlClauseBuilder builder = new UpdateSqlClauseBuilder();

			builder.AppendItem("EXPIRE_TIME", expireTime);

			string sql = string.Format("UPDATE {0} SET {1} WHERE AUTHENTICATION_ID = {2}",
				this.GetMappingInfo().TableName,
				builder.ToSqlString(TSqlBuilder.Instance),
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(authenticationID));

			ExecuteNonQuery(sql);
		}

		private AuthenticationCode Query(string sql)
		{
			AuthenticationCode result = null;

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(this.GetConnectionName());

				DataTable table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0];

				if (table.Rows.Count > 0)
				{
					result = new AuthenticationCode();
					ORMapping.DataRowToObject(table.Rows[0], result);
				}
			}

			return result;
		}

		private int ExecuteNonQuery(string sql)
		{
			int result = 0;

			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(this.GetConnectionName());

					result = db.ExecuteNonQuery(CommandType.Text, sql);
				}

				scope.Complete();
			}

			return result;
		}

		/// <summary>
		/// 是否是合法的认证码
		/// </summary>
		/// <param name="authenticationID"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool IsValidAuthenticationCode(string authenticationID, string code)
		{
			authenticationID.CheckStringIsNullOrEmpty("AUTHENTICATION_ID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("AUTHENTICATION_ID", authenticationID);
			builder.AppendItem("AUTHENTICATION_CODE", code);
			builder.AppendItem("EXPIRE_TIME", "GETDATE()", ">", true);

			string sql = string.Format("SELECT AUTHENTICATION_CODE FROM {0} WHERE {1}",
				this.GetMappingInfo().TableName, builder.ToSqlString(TSqlBuilder.Instance));

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(this.GetConnectionName());

				object returnCode = db.ExecuteScalar(CommandType.Text, sql);

				return returnCode != null;
			}
		}

		/// <summary>
		/// 检查已有的验证码是否合法
		/// </summary>
		/// <param name="authenticationID"></param>
		/// <param name="code"></param>
		public void CheckAuthenticationCode(string authenticationID, string code)
		{
			IsValidAuthenticationCode(authenticationID, code).FalseThrow(Translator.Translate(Define.DefaultCategory, "认证码\"{0}\"不合法或已过期", code));
		}

		private ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<AuthenticationCode>();
		}

		private string GetConnectionName()
		{
			return DataAdapter.DBConnectionName;
		}
	}
}
