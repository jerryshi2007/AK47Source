using System;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 用户口令的适配器。主要是用户口令的读取和设置方法。
	/// </summary>
	public class UserPasswordAdapter
	{
		/// <summary>
		/// 应用密码类型
		/// </summary>
		private const string DefaultPasswordType = "MCS.Authentication";

		/// <summary>
		/// 默认加密类型
		/// </summary>
		public const string DefaultAlgorithmType = "MCS.MD5";

		/// <summary>
		/// 默认密码
		/// </summary>
		private const string DefaultPassword = "password";

		/// <summary>
		/// <see cref="UserPasswordAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly UserPasswordAdapter Instance = new UserPasswordAdapter();

		private UserPasswordAdapter()
		{
		}

		/// <summary>
		/// 设置用户的密码
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="passwordType">密码类型</param>
		/// <param name="password">密码</param>
		/// <param name="algorithmType">表示算法类型的字符串</param>
		public void SetPassword(string userID, string passwordType, string password, string algorithmType = "")
		{
			string updateSql = @"UPDATE SC.UserPassword SET Password = @Password, AlgorithmType = @AlgorithmType WHERE  (UserID = @UserID) AND (PasswordType = @PasswordType)";
			string insertSql = @"INSERT INTO SC.UserPassword(Password, AlgorithmType, UserID, PasswordType) VALUES (@Password, @AlgorithmType, @UserID, @PasswordType)";

			string calculatedPassword = CalculatepasswordByAlgorithmType(algorithmType, password);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					Database db = DatabaseFactory.Create(context);

					DbCommand updateCommand = db.GetSqlStringCommand(updateSql);

					SetDbCommandParameter(db, updateCommand, calculatedPassword, userID, passwordType, algorithmType);

					if (updateCommand.ExecuteNonQuery() == 0)
					{
						DbCommand insertCommand = db.GetSqlStringCommand(insertSql);
						SetDbCommandParameter(db, insertCommand, calculatedPassword, userID, passwordType, algorithmType);

						insertCommand.ExecuteNonQuery();
					}
				}

				scope.Complete();
			}
		}

		private static void SetDbCommandParameter(Database db, DbCommand command, string calculatepassword, string userID, string passwordType, string algorithmType)
		{
			db.AddInParameter(command, "Password", DbType.String, calculatepassword);
			db.AddInParameter(command, "UserID", DbType.String, userID);
			db.AddInParameter(command, "PasswordType", DbType.String, passwordType);
			db.AddInParameter(command, "AlgorithmType", DbType.String, string.IsNullOrEmpty(algorithmType) == true ? DefaultAlgorithmType : algorithmType);
		}

		/// <summary>
		/// 检查密码是否正确。
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="passwordType"></param>
		/// <param name="password"></param>
		public bool CheckPassword(string userID, string passwordType, string password)
		{
			string selectSql = @"SELECT COUNT(*) FROM SC.UserPassword AS UP WHERE (UserID = @userID) AND (Password = @password) AND (PasswordType = @passwordType)";
			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);
				DbCommand command = db.GetSqlStringCommand(selectSql);
				db.AddInParameter(command, "userID", DbType.String, userID);

				db.AddInParameter(command, "password", DbType.String, CalculatepasswordByAlgorithmType(DefaultAlgorithmType, password));
				db.AddInParameter(command, "passwordType", DbType.String, passwordType);

				return (int)db.ExecuteScalar(command) == 1 ? true : false;
			}
		}

		private static string CalculatepasswordByAlgorithmType(string algorithmType, string password)
		{
			string result = password;

			if (string.Compare(algorithmType, DefaultAlgorithmType, true) == 0 || algorithmType.IsNullOrEmpty())
			{
				result = PwdCalculate(algorithmType, password);
			}

			return result;
		}

		public static string GetPasswordType()
		{
			return DefaultPasswordType;
		}

		public static string GetDefaultPassword()
		{
			return DefaultPassword;
		}

		#region "加密方式"
		/// <summary>
		/// 按照一定的加密算法生成转换后的加密数据（用于密码值计算）
		/// </summary>
		/// <param name="strPwdType">指定的加密算法类型</param>
		/// <param name="strPwd">指定要求被加密的数据</param>
		/// <returns>按照一定的加密算法生成转换后的加密数据（用于密码值计算）</returns>
		public static string PwdCalculate(string strPwdType, string strPwd)
		{
			string strResult = strPwd;

			using (MD5 md = new MD5CryptoServiceProvider())
			{
				strResult = BitConverter.ToString(md.ComputeHash((new UnicodeEncoding()).GetBytes(strPwd)));
			}

			return strResult;
		}
		#endregion

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		protected string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
