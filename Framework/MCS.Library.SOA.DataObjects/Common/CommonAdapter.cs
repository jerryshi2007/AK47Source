using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
    public class CommonAdapter
    {
        private string connectionName = string.Empty;

        /// <summary>
        /// 获得使用的数据库连接串名
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
        }

        private CommonAdapter() { }

        /// <summary>
        /// 数据库连接串名默认是HB2008，若需要改变连接串名则使用本构造函数，该种用法下本类不是单件模式
        /// </summary>
        /// <param name="connectionName">使用的数据库连接串名</param>
        public CommonAdapter(string connectionName)
        {
            this.connectionName = connectionName;
        }

        public static readonly CommonAdapter Instance = new CommonAdapter();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="qc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public TCollection SplitPageQuery<T, TCollection>(QueryCondition qc, ref int totalCount)
            where T : new()
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            return SplitPageQuery<T, TCollection>(qc, DefaultDataRowToObject<T, TCollection>, ref totalCount);
        }

        public DataView SplitPageQuery(QueryCondition qc, ref int totalCount)
        {
            DataSet ds = this.SplitPageQuery(qc, totalCount <= 0);

            DataView result = ds.Tables[0].DefaultView;

            if (ds.Tables.Count > 1)
                totalCount = (int)ds.Tables[1].Rows[0][0];

            //当页码超出索引的，返回最大页
            if (result.Count == 0 && totalCount > 0)
            {
                int newStartRowIndex = (totalCount - 1) / qc.PageSize * qc.PageSize;

                totalCount = -1;

                qc.RowIndex = newStartRowIndex;

                result = SplitPageQuery(qc, ref totalCount);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="qc"></param>
        /// <param name="rowToObject"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public TCollection SplitPageQuery<T, TCollection>(QueryCondition qc, Action<TCollection, DataRow> rowToObject, ref int totalCount)
            where T : new()
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            qc.NullCheck("qc");
            rowToObject.NullCheck("rowToObject");

            DataSet ds = this.SplitPageQuery(qc, totalCount <= 0);

            TCollection result = new TCollection();

            foreach (DataRow row in ds.Tables[0].Rows)
                rowToObject(result, row);

            if (ds.Tables.Count > 1)
                totalCount = (int)ds.Tables[1].Rows[0][0];

            //当页码超出索引的，返回最大页
            if (result.Count == 0 && totalCount > 0)
            {
                int newStartRowIndex = (totalCount - 1) / qc.PageSize * qc.PageSize;

                totalCount = -1;

                qc.RowIndex = newStartRowIndex;

                result = SplitPageQuery<T, TCollection>(qc, rowToObject, ref totalCount);
            }

            return result;
        }

        public DataSet SplitPageQuery(QueryCondition qc, bool retrieveTotalCount)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(null == qc, "qc");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.SelectFields, "qc.SelectFields");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.FromClause, "qc.FromClause");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.OrderByClause, "qc.OrderByClause");

            DataSet ds = null;

            if (qc.RowIndex == 0 && qc.PageSize == 0)	//一种假设，qc.RowIndex == 0 && qc.PageSize == 0认为不分页
                ds = DoNoSplitPageQuery(qc);
            else
                ds = DoSplitPageQuery(qc, retrieveTotalCount);

            return ds;
        }

        /// <summary>
        /// 得到查询总行数的SQL
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public string GetTotalCountSql(QueryCondition qc)
        {
            string sql = string.Empty;

            if (qc.GroupBy.IsNotEmpty())
            {
                sql = GetNoSplitPageSqlWithoutOrderBy(qc);
                sql = string.Format("SELECT COUNT(1) AS TOTAL_COUNT FROM ({0}) TEMP ", sql);
            }
            else
            {
                sql = string.Format("SELECT COUNT(1) AS TOTAL_COUNT FROM {0} WHERE 1 = 1 {1}",
                        qc.FromClause,
                        string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause);
            }

            return sql;
        }

        public string GetQuerySql(QueryCondition qc, bool retrieveTotalCount)
        {
            string sql = ResourceHelper.LoadStringFromResource(
                            Assembly.GetExecutingAssembly(),
                            "MCS.Library.SOA.DataObjects.Common.SplitPage.sql");

            sql = string.Format(
                sql,
                qc.SelectFields,
                qc.FromClause,
                string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause,
                qc.OrderByClause,
                qc.GroupBy.IsNotEmpty() ? "GROUP BY " + qc.GroupBy : string.Empty,
                qc.RowIndex + 1,
                qc.RowIndex + qc.PageSize);

            if (retrieveTotalCount)
                sql += TSqlBuilder.Instance.DBStatementSeperator + GetTotalCountSql(qc);

            return sql;
        }

        private DataSet DoSplitPageQuery(QueryCondition qc, bool retrieveTotalCount)
        {
            string sql = GetQuerySql(qc, retrieveTotalCount);

            using (DbContext context = DbContext.GetContext(string.IsNullOrEmpty(this.ConnectionName) ? ConnectionDefine.DBConnectionName : this.ConnectionName))
            {
                DataSet ds = null;
                int serverVersion = Convert.ToInt32(context.Connection.ServerVersion.Split('.')[0]);

                Database db = DatabaseFactory.Create(context);
                //根据SQL Server版本选择分页语句的写法
                if (serverVersion > 8)
                    ds = db.ExecuteDataSet(CommandType.Text, sql, "RESULT", "RESULT_COUNT");
                else
                    ds = db.ExecuteDataSet("CommonSplitPageQuery",
                        qc.SelectFields,
                        qc.FromClause,
                        qc.WhereClause,
                        qc.OrderByClause,
                        qc.PrimaryKey,
                        qc.RowIndex / qc.PageSize + 1,
                        qc.PageSize,
                        1);

                return ds;
            }
        }

        private DataSet DoNoSplitPageQuery(QueryCondition qc)
        {
            string sql = GetNoSplitPageSql(qc);

            using (DbContext context = DbContext.GetContext(string.IsNullOrEmpty(this.ConnectionName) ? ConnectionDefine.DBConnectionName : this.ConnectionName))
            {
                Database db = DatabaseFactory.Create(context);

                DataSet ds = db.ExecuteDataSet(CommandType.Text, sql, "RESULT");

                DataTable table = new DataTable("RESULT_COUNT");

                table.Columns.Add("TOTAL_COUNT", typeof(int));
                table.Rows.Add(ds.Tables[0].Rows.Count);

                ds.Tables.Add(table);

                return ds;
            }
        }

        private static string GetNoSplitPageSql(QueryCondition qc)
        {
            string sql = string.Format("SELECT {0} FROM {1} WHERE 1 = 1 {2} {3} ORDER BY {4}",
                        qc.SelectFields,
                        qc.FromClause,
                        qc.WhereClause.IsNotEmpty() ? " AND " + qc.WhereClause : string.Empty,
                        qc.GroupBy.IsNotEmpty() ? "GROUP BY " + qc.GroupBy : string.Empty,
                        qc.OrderByClause);

            return sql;
        }

        private static string GetNoSplitPageSqlWithoutOrderBy(QueryCondition qc)
        {
            string sql = string.Format("SELECT {0} FROM {1} WHERE 1 = 1 {2} {3}",
                        qc.SelectFields,
                        qc.FromClause,
                        qc.WhereClause.IsNotEmpty() ? " AND " + qc.WhereClause : string.Empty,
                        qc.GroupBy.IsNotEmpty() ? "GROUP BY " + qc.GroupBy : string.Empty);

            return sql;
        }

        private static void DefaultDataRowToObject<T, TCollection>(TCollection dataCollection, DataRow row)
            where T : new()
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            T data = new T();

            ORMappingItemCollection mapping = ORMapping.GetMappingInfo<T>();
            ORMapping.DataRowToObject(row, mapping, data);

            dataCollection.Add(data);
        }
    }
}
