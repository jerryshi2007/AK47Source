using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 机构人员管理控制类
	/// </summary>
	/// <remarks>
	/// 目前仅仅提供密码修改功能
	/// </remarks>
	public class OGUControler
	{
		#region 构造函数
		/// <summary>
		/// 构造函数
		/// </summary>
		public OGUControler()
		{

		}

		#endregion

		#region public function

		/// <summary>
		/// 用户修改口令接口
		/// </summary>
		/// <param name="strUserGuid">要求被修改口令的用户</param>
		/// <param name="strOldPwd">用户的旧口令</param>
		/// <param name="strNewPwd">使用的新口令</param>
		/// <param name="strConfirmPwd">新口令的确认</param>
		/// <returns>本次修改是否成功</returns>
		public bool UpdateUserPwd(string strUserGuid, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			return UpdateUserPwd(strUserGuid, SearchObjectColumn.SEARCH_GUID, strOldPwd, strNewPwd, strConfirmPwd);
		}

		#endregion

		#region public function (with DataAccess)
		/// <summary>
		/// 用户修改口令接口
		/// </summary>
		/// <param name="strUserValue">要求被修改口令的用户</param>
		/// <param name="socu">strUserValue对应的数据类型</param>
		/// <param name="strOldPwd">用户的旧口令</param>
		/// <param name="strNewPwd">使用的新口令</param>
		/// <param name="strConfirmPwd">新口令的确认</param>
		/// <returns>本次修改是否成功</returns>
		public bool UpdateUserPwd(string strUserValue, SearchObjectColumn socu, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strNewPwd.Trim()), "对不起，用户的登录口令不能为空！");
			ExceptionHelper.FalseThrow(strNewPwd == strConfirmPwd, "对不起，用户的“新口令”必须与“确认口令”一致！");

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
					string strSql = @"SELECT USERS.GUID 
				FROM USERS, OU_USERS
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND " + DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS")
						  + " = " + TSqlBuilder.Instance.CheckQuotationMark(strUserValue, true) + @";
				SELECT TOP 1 GUID FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID;";

					DataSet ds = database.ExecuteDataSet(CommandType.Text, strSql);

					ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count == 0, "对不起，系统中没有找到您指定的用户！");
					ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count > 1, "对不起，您指定的用户在系统中不唯一！");
					ExceptionHelper.TrueThrow(ds.Tables[1].Rows.Count < 1, "对不起，系统中找的不到数据表PWD_ARITHMETIC的数据！");
					string secNewPwd = SecurityCalculate.PwdCalculate(ds.Tables[1].Rows[0][0].ToString(), strNewPwd);
					string secOldPwd = SecurityCalculate.PwdCalculate(ds.Tables[1].Rows[0][0].ToString(), strOldPwd);

					strSql = "UPDATE USERS SET USER_PWD = "
						+ TSqlBuilder.Instance.CheckQuotationMark(secNewPwd, true) + @"
				WHERE USERS.GUID = " + TSqlBuilder.Instance.CheckQuotationMark((string)ds.Tables[0].Rows[0]["GUID"], true) + @"
					AND USERS.USER_PWD = "
						+ TSqlBuilder.Instance.CheckQuotationMark(secOldPwd, true);

					ExceptionHelper.FalseThrow(database.ExecuteNonQuery(CommandType.Text, strSql) == 1, "对不起，用户的旧口令不正确！");
				}
				scope.Complete();
			}
			return true;
		}

		#endregion

		#region private function

		//private static DataSet _DataSet_Schema = null;
		///// <summary>
		///// 根据数据表名称以及要求获取的数据表中的数据列名称组合成为核数据表相关的数据查询语言
		///// </summary>
		///// <param name="strAttrs">数据表中的数据列名称组合</param>
		///// <param name="da">数据库操作对象</param>
		///// <param name="strTables">指定的数据表名称</param>
		///// <returns>数据查询语言SQL</returns>
		//private static string GetTableColumns(string strAttrs, params string[] strTables)
		//{
		//    StringBuilder strB = new StringBuilder(1024);
		//    ExceptionHelper.TrueThrow(strAttrs.Length == 0, "对不起,程序没有指定要求查询的列名称！请验证！");
		//    ExceptionHelper.TrueThrow(strTables.Length == 0, "对不起，没有确定的数据表名称！");

		//    if (strAttrs.Trim() == "*")
		//        strB.Append(strTables[0] + "." + strAttrs.Trim());
		//    else
		//    {
		//        string[] strAttrArr = strAttrs.Split(',');
		//        InitHashTable(da);
		//        for (int i = 0; i < strAttrArr.Length; i++)
		//        {
		//            strAttrArr[i] = strAttrArr[i].Trim();

		//            if (strB.Length > 0)
		//                strB.Append(", ");

		//            bool bComplicated = false;

		//            for (int j = 0; j < strTables.Length; j++)
		//            {
		//                DataTable table = _DataSet_Schema.Tables[strTables[j]];

		//                DataRow[] drs = table.Select("CNAME=" + GetSqlStr.AddCheckQuotationMark(strAttrArr[i]));
		//                if (drs.Length > 0)
		//                {
		//                    strB.Append(XmlHelper.DBValueToString(drs[0]["TNAME"]) + "." + strAttrArr[i]);
		//                    bComplicated = true;
		//                    break;
		//                }
		//            }

		//            if (false==bComplicated)
		//                strB.Append(" NULL AS " + strAttrArr[i]);
		//        }
		//    }

		//    return strB.ToString();
		//}

//        /// <summary>
//        /// 
//        /// </summary>
//        private static void InitHashTable(DataAccess da)
//        {
//            if (_DataSet_Schema == null)
//            {
//                string[] strAllTables = { "ORGANIZATIONS", "GROUPS", "USERS", "OU_USERS", "RANK_DEFINE", "GROUP_USERS" };
//                StringBuilder strB = new StringBuilder(512);
//                for (int i = 0; i < strAllTables.Length; i++)
//                {
//                    strB.Append(@"
//						SELECT SYSOBJECTS.NAME AS TNAME, SYSCOLUMNS.NAME AS CNAME
//						FROM SYSOBJECTS, SYSCOLUMNS
//						WHERE SYSOBJECTS.ID = SYSCOLUMNS.ID
//							AND SYSOBJECTS.NAME IN (" + GetSqlStr.AddCheckQuotationMark(strAllTables[i]) + @")
//						ORDER BY TNAME, SYSCOLUMNS.COLID;
//						");
//                }
//                _DataSet_Schema = OGUCommonDefine.ExecuteDataset(strB.ToString(), da, strAllTables);
//            }
//        }

		#endregion
	}
}
