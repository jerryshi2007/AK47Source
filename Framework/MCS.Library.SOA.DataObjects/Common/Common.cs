#region 作者版本
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	Common.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070724		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Data;
using System.Transactions;
using System.Collections.Generic;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class DbHelper
    {
        #region  数据访问函数

        public static DbContext GetDBContext()
        {
            return DbContext.GetContext(ConnectionDefine.DBConnectionName);
        }

        public static DbContext GetDBContext(string connectionName)
        {
            return DbContext.GetContext(connectionName);
        }

        public static Database GetDBDatabase()
        {
            return DatabaseFactory.Create(ConnectionDefine.DBConnectionName);
        }

        public static Database GetDBDatabase(string connectionName)
        {
            return DatabaseFactory.Create(connectionName);
        }

        public static void RunSql(Action<Database> action)
        {
            RunSql(action, ConnectionDefine.DBConnectionName);
        }

        public static void RunSql(Action<Database> action, string connectionName)
        {
            using (DbContext context = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(connectionName);

                action(db);
            }
        }

        public static object RunSqlReturnScalar(string strSql)
        {
            return RunSqlReturnScalar(strSql, ConnectionDefine.DBConnectionName);
        }

        public static object RunSqlReturnScalar(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteScalar(CommandType.Text, strSql);
            }
        }

        public static int RunSql(string strSql)
        {
            return RunSql(strSql, ConnectionDefine.DBConnectionName);
        }

        /// <summary>
        /// 周杨于20090715修改
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static int RunSql(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);
                return db.ExecuteNonQuery(CommandType.Text, strSql);
            }
        }

        public static int RunSqlWithTransaction(string strSql)
        {
            return RunSqlWithTransaction(strSql, ConnectionDefine.DBConnectionName);
        }

        public static int RunSqlWithTransaction(string strSql, string connectionName)
        {
            using (TransactionScope ts = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                int result = RunSql(strSql, connectionName);

                ts.Complete();

                return result;
            }
        }

        public static IDataReader RunSqlReturnDR(string strSql)
        {
            return RunSqlReturnDR(strSql, ConnectionDefine.DBConnectionName);
        }

        public static IDataReader RunSqlReturnDR(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteReader(CommandType.Text, strSql);
            }
        }

        public static DataSet RunSqlReturnDS(string strSql)
        {
            return RunSqlReturnDS(strSql, ConnectionDefine.DBConnectionName);
        }

        public static DataSet RunSqlReturnDS(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteDataSet(CommandType.Text, strSql);
            }
        }

        public static XmlDocument RunSQLReturnXmlDoc(string strSql)
        {
            return RunSQLReturnXmlDoc(strSql, ConnectionDefine.DBConnectionName);
        }

        public static XmlDocument RunSQLReturnXmlDoc(string strSql, string connectionName)
        {
            string xmlStr = string.Empty;

            if (strSql != string.Empty)
            {
                strSql += " FOR XML AUTO, ELEMENTS";
            }

            object obj = RunSqlReturnScalar(strSql);

            if (obj != null)

                xmlStr = obj.ToString();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xmlStr);

            return xmlDoc;
        }

        public static DataSet RunSPReturnDS(string spName, params object[] parameterValues)
        {
            using (DbContext dbi = GetDBContext())
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteDataSet(spName, parameterValues);
            }
        }

        public static IDataReader RunSPReturnDR(string spName, params object[] parameterValues)
        {
            using (DbContext dbi = GetDBContext())
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteReader(spName, parameterValues);
            }
        }

        /*
    /// <summary>
    /// 返回通用查询的存储过程的查询结果。
    /// </summary>
    /// <param name="fields">返回的字段</param>
    /// <param name="tables">查询涉及到的表，多个表由逗号分割</param>
    /// <param name="where">查询条件</param>
    /// <param name="orderBy">排序条件</param>
    /// <param name="key">查询结果中的关键字</param>
    /// <param name="pageNo">返回第几页</param>
    /// <param name="pageCount">每页的记录数</param>
    /// <param name="retRowCount">第一页的返回集，是否返回总记录数</param>
    /// <returns></returns>
    [Obsolete]
    public static DataSet SplitPageQuery(string fields, string tables, string where, string orderBy, string key, int pageNo, int pageCount, bool retRowCount)
    {

        using (DbContext dbi = GetDBContext())
        {
            Database db = DatabaseFactory.Create(dbi);

            return db.ExecuteDataSet("CommonSplitPageQuery", fields,
                tables,
                where,
                orderBy,
                key,
                pageNo,
                pageCount,
                retRowCount ? (int)1 : (int)0);
        }
    }*/
        #endregion

        #region 数据验证
        public static void ValidateFalseThrow(object validateObject, params string[] unValidates)
        {
            ValidateFalseThrow<SystemSupportException>(validateObject, unValidates);
        }

        public static void ValidateFalseThrow<T>(object validateObject, params string[] unValidates) where T : System.Exception
        {
            List<string> unValidatesList = new List<string>();

            if (unValidates != null)
                unValidatesList.AddRange(unValidates);

            Validator validator = ValidationFactory.CreateValidator(validateObject.GetType(), unValidatesList);
            ValidationResults validationResults = validator.Validate(validateObject);

            CheckValidationResults<T>(validationResults);
        }

        public static void ValidateFalseThrow<T>(object validateObject, string ruleset) where T : System.Exception
        {
            Validator validator = ValidationFactory.CreateValidator(validateObject.GetType(), ruleset);
            ValidationResults validationResults = validator.Validate(validateObject);

            CheckValidationResults<T>(validationResults);
        }

        public static void CheckValidationResults<T>(ValidationResults validationResults) where T : System.Exception
        {
            if (validationResults.IsValid() == false)
            {
                string errorMessage = BuildErrorMessage(validationResults);

                errorMessage.IsNotEmpty().TrueThrow<T>(errorMessage);
            }
        }

        private static string BuildErrorMessage(IEnumerable<ValidationResult> validationResults)
        {
            StringBuilder strB = new StringBuilder();

            foreach (ValidationResult result in validationResults)
                strB.AppendLine(result.Message);

            return strB.ToString();
        }
        #endregion

        /// <summary>
        /// 从Tenant上下文中获取TenantCode并且添加到Builder中
        /// </summary>
        /// <param name="builder"></param>
        public static T AppendTenantCode<T>(this T builder) where T : SqlClauseBuilderIUW
        {
            if (builder != null)
            {
                if (TenantContext.Current.Enabled)
                    builder.AppendItem("TENANT_CODE", TenantContext.Current.TenantCode);
            }

            return builder;
        }
    }
}
