#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	DataAdapter.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Transactions;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Data.SqlClient;
using MCS.Library.Globalization;
using System.Reflection;
using System.Xml;
using System.IO;

namespace MCS.Library.Passport
{
    /// <summary>
    /// SignInInfo和Ticket信息持久化类
    /// </summary>
    internal class DataAdapter : IPersistSignInInfo, IPersistOpenIDBinding
    {
        internal const string DBConnectionName = "DeluxeWorksPassport";

        internal DataAdapter()
        {
        }

        #region IPersistSignInInfo
        /// <summary>
        /// 保持SignInInfo信息
        /// </summary>
        /// <param name="signInInfo">SignInInfo信息</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoPersistTest" lang="cs" title="保存SignInInfo对象" />
        /// </remarks>
        public void SaveSignInInfo(ISignInInfo signInInfo)
        {
            ORMappingItemCollection mapping = LoadMappingFromResource("MCS.Library.Passport.DataObjects.SignInInfoMapping.xml", typeof(ISignInInfo));

            string sql = string.Format("UPDATE PASSPORT_SIGNIN_INFO SET {0} WHERE {1}",
                ORMapping.GetUpdateSqlClauseBuilder(signInInfo, mapping).ToSqlString(TSqlBuilder.Instance),
                ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(signInInfo, mapping).ToSqlString(TSqlBuilder.Instance));

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
                {
                    Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                    if (db.ExecuteNonQuery(CommandType.Text, sql) == 0)
                    {
                        sql = string.Format("INSERT INTO PASSPORT_SIGNIN_INFO {0}",
                            ORMapping.GetInsertSqlClauseBuilder(signInInfo, mapping).ToSqlString(TSqlBuilder.Instance));

                        db.ExecuteNonQuery(CommandType.Text, sql);
                    }
                }

                scope.Complete();
            }
        }
        /// <summary>
        /// 保持ITicket信息
        /// </summary>
        /// <param name="ticket">ITicket信息</param>
        /// <param name="signInUrl">登录Url</param>
        /// <param name="logOffCBUrl">注销的Url</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="TicketPersistTest" lang="cs" title="保存Ticket对象" />
        /// </remarks>
        public void SaveTicket(ITicket ticket, Uri signInUrl, Uri logOffCBUrl)
        {
            ORMappingItemCollection mapping = LoadMappingFromResource("MCS.Library.Passport.DataObjects.TicketInfoMapping.xml", typeof(ITicket));

            WhereSqlClauseBuilder whereBuilder =
                ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(ticket, mapping);

            whereBuilder.AppendItem("SIGNIN_ID", ticket.SignInInfo.SignInSessionID);

            UpdateSqlClauseBuilder updateBuilder = ORMapping.GetUpdateSqlClauseBuilder(ticket, mapping);

            updateBuilder.AppendItem("APP_SIGNIN_URL", signInUrl.ToString());
            updateBuilder.AppendItem("APP_LOGOFF_URL", logOffCBUrl.ToString());

            string sql = string.Format("UPDATE PASSPORT_TICKET SET {0} WHERE {1}",
                updateBuilder.ToSqlString(TSqlBuilder.Instance),
                whereBuilder.ToSqlString(TSqlBuilder.Instance));

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
                {
                    Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);
                    if (db.ExecuteNonQuery(CommandType.Text, sql) == 0)
                    {
                        InsertSqlClauseBuilder insertBuilder = ORMapping.GetInsertSqlClauseBuilder(ticket, mapping);

                        insertBuilder.AppendItem("SIGNIN_ID", ticket.SignInInfo.SignInSessionID);
                        insertBuilder.AppendItem("APP_SIGNIN_URL", signInUrl.ToString());
                        insertBuilder.AppendItem("APP_LOGOFF_URL", logOffCBUrl.ToString());

                        sql = string.Format("INSERT INTO PASSPORT_TICKET {0}",
                            insertBuilder.ToSqlString(TSqlBuilder.Instance));

                        db.ExecuteNonQuery(CommandType.Text, sql);
                    }
                }

                scope.Complete();
            }
        }
        /// <summary>
        /// 删除sessionID相关的SignInInfo信息
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="SignInInfoPersistTest" lang="cs" title="删除SignInInfo对象" />
        /// </remarks>
        public void DeleteRelativeSignInInfo(string sessionID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(sessionID, "sessionID");

            StringBuilder strB = new StringBuilder(256);

            strB.Append(TSqlBuilder.Instance.DBStatementBegin);
            strB.AppendFormat("DELETE PASSPORT_SIGNIN_INFO WHERE SIGNIN_ID = {0}" +
                TSqlBuilder.Instance.DBStatementSeperator +
                "DELETE PASSPORT_TICKET WHERE SIGNIN_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(sessionID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementEnd);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
                {
                    Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                    db.ExecuteNonQuery(CommandType.Text, strB.ToString());
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// 获取注销时所有需要注销的Url回调地址
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        /// <returns>相关应用的注销回调地址列表</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\DataObjectsTest.cs" region="GetAppLogOffCallBackTest" lang="cs" title="获取相关应用的注销回调地址列表" />
        /// </remarks>
        public List<AppLogOffCallBackUrl> GetAllRelativeAppsLogOffCallBackUrl(string sessionID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(sessionID, "sessionID");

            string sql = string.Format("SELECT APP_ID, APP_LOGOFF_URL FROM PASSPORT_TICKET WHERE SIGNIN_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(sessionID, true));

            DataTable table;

            using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
            {
                Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0];
            }

            Dictionary<string, AppLogOffCallBackUrl> dict =
                new Dictionary<string, AppLogOffCallBackUrl>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in table.Rows)
            {
                AppLogOffCallBackUrl au = new AppLogOffCallBackUrl();

                au.AppID = row["APP_ID"].ToString();
                au.LogOffCallBackUrl = row["APP_LOGOFF_URL"].ToString();

                dict[au.AppID + "-" + au.LogOffCallBackUrl] = au;
            }

            List<AppLogOffCallBackUrl> list = new List<AppLogOffCallBackUrl>();

            foreach (KeyValuePair<string, AppLogOffCallBackUrl> kv in dict)
            {
                list.Add(kv.Value);
            }

            return list;
        }
        #endregion IPersistSignInInfo

        #region IPersistOpenIDBinding
        /// <summary>
        /// 保存OpenIDBinding
        /// </summary>
        /// <param name="binding"></param>
        public void SaveOpenIDBinding(OpenIDBinding binding)
        {
            binding.NullCheck("binding");

            string sql = ORMapping.GetInsertSql(binding, TSqlBuilder.Instance);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
                {
                    Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                    try
                    {
                        db.ExecuteNonQuery(CommandType.Text, sql);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                            throw new OpenIDBindingException(Translator.Translate(Define.DefaultCategory, "OpenID已经绑定到某用户了，不能重复绑定"));

                        throw;
                    }
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// 删除OpenID的绑定信息
        /// </summary>
        /// <param name="openID"></param>
        public void RemoveOpenIDBinding(string openID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(openID, "openID");

            ORMappingItemCollection mappings = ORMapping.GetMappingInfo(typeof(OpenIDBinding));

            string sql = string.Format("DELETE FROM {0} WHERE OPEN_ID = {1}",
                mappings.TableName,
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(openID));

            using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
            {
                Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                db.ExecuteNonQuery(CommandType.Text, sql);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public OpenIDBinding GetBindingByOpenID(string openID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(openID, "openID");

            ORMappingItemCollection mappings = ORMapping.GetMappingInfo(typeof(OpenIDBinding));

            string sql = string.Format("SELECT * FROM {0} WHERE OPEN_ID = {1}",
                mappings.TableName,
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(openID));

            DataTable table;
            using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
            {
                Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0];
            }

            OpenIDBinding binding = null;

            if (table.Rows.Count > 0)
            {
                binding = new OpenIDBinding();

                ORMapping.DataRowToObject(table.Rows[0], binding);
            }

            return binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public OpenIDBindingCollection GetBindingsByUserID(string userID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(userID, "userID");

            ORMappingItemCollection mappings = ORMapping.GetMappingInfo(typeof(OpenIDBinding));

            string sql = string.Format("SELECT * FROM {0} WHERE USER_ID = {1}",
                mappings.TableName,
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(userID));

            OpenIDBindingCollection result = new OpenIDBindingCollection();

            using (DbContext context = DbContext.GetContext(DataAdapter.DBConnectionName))
            {
                Database db = DatabaseFactory.Create(DataAdapter.DBConnectionName);

                DataView view = db.ExecuteDataSet(CommandType.Text, sql).Tables[0].DefaultView;

                ORMapping.DataViewToCollection(result, view);
            }

            return result;
        }
        #endregion IPersistOpenIDBinding

        private static ORMappingItemCollection LoadMappingFromResource(string path, Type type)
        {
            ORMappingItemCollection mapping = new ORMappingItemCollection();

            Stream stream = null;

            try
            {
                using (XmlReader reader = ResourceHelper.LoadXmlReaderFromResource(Assembly.GetExecutingAssembly(),
                    path, out stream))
                {
                    mapping.ReadFromXml(reader, type);
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return mapping;
        }
    }
}
