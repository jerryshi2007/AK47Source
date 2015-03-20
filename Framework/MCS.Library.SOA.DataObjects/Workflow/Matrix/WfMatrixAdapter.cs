using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfMatrixAdapter : UpdatableAndLoadableAdapterBase<WfMatrix, WfMatrixCollection>, IWfMatrixQuery
    {
        public static readonly WfMatrixAdapter Instance = new WfMatrixAdapter();

        internal static readonly string INSERT_MR_SQL_CLAUSE_PREFIX = "INSERT INTO [WF].[MATRIX_ROWS] ";
        internal static readonly string INSERT_MC_SQL_CLAUSE_PREFIX = "INSERT INTO [WF].[MATRIX_CELLS] ";
        internal static readonly string SELECT_SQL_CLAUSE = "SELECT R.[MATRIX_ID],R.[MATRIX_ROW_ID],R.[OPERATOR_TYPE],R.[OPERATOR],C.DIMENSION_KEY,C.STRING_VALUE FROM [WF].[MATRIX_ROWS] R JOIN [WF].[MATRIX_CELLS] C ON R.MATRIX_ID = C.MATRIX_ID AND R.MATRIX_ROW_ID = C.MATRIX_ROW_ID  WHERE ";
        internal static readonly string SELECT_MC_CLAUSE = "SELECT MATRIX_ROW_ID FROM [WF].[MATRIX_CELLS] WHERE ";
        internal static readonly string DELETE_SQL_CLAUSE = "DELETE [WF].[MATRIX_MAIN] WHERE ";
        internal static readonly string DELETE_MR_SQL_CLAUSE = "DELETE [WF].[MATRIX_ROWS] WHERE ";
        internal static readonly string DELETE_MC_SQL_CLAUSE = "DELETE [WF].[MATRIX_CELLS] WHERE ";
        internal static readonly string DELETE_MU_SQL_CLAUSE = "DELETE [WF].[MATRIX_ROWS_USERS] WHERE ";

        internal static readonly string DB_FIELD_MATRIX_ID = "MATRIX_ID";
        internal static readonly string DB_FIELD_MATRIX_ROW_ID = "MATRIX_ROW_ID";
        internal static readonly string DB_FIELD_DIMENSION_KEY = "DIMENSION_KEY";
        internal static readonly string DB_FIELD_STRING_VALUE = "STRING_VALUE";
        internal static readonly string DB_FIELD_OPERATOR = "OPERATOR";
        internal static readonly string DB_FIELD_OPERATOR_TYPE = "OPERATOR_TYPE";

        private WfMatrixAdapter()
        {
        }

        public WfMatrix Load(string matrixID)
        {
            matrixID.NullCheck("matrixID");

            WfMatrixCollection matrices = Load(builder => builder.AppendItem(DB_FIELD_MATRIX_ID, matrixID));

            (matrices.Count > 0).FalseThrow("不能找到{0}为{1}的矩阵记录", DB_FIELD_MATRIX_ID, matrixID);

            return matrices[0];
        }

        public WfMatrix GetByProcessKey(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WfMatrix result = WfMatrixProcessKeyCache.Instance.GetOrAddNewValue(processKey, (cache, key) =>
            {
                WfMatrix m = LoadByProcessKey(key, false);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, m, dependency);

                return m;
            });

            return result;
        }

        public WfMatrix LoadByProcessKey(string processKey, bool autoThrowException)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WfMatrixCollection matrices = Load(builder => builder.AppendItem("PROCESS_KEY", processKey));

            WfMatrix result = null;

            if (matrices.Count > 0)
                result = matrices[0];
            else
                autoThrowException.TrueThrow("不能找到PROCESS_KEY为{0}的矩阵记录", processKey);

            return result;
        }

        /// <summary>
        /// 查询用户相关的权限矩阵所属的流程信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataTable QueryUserRelativeProcessMatrices(string userID)
        {
            string sql = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                "MCS.Library.SOA.DataObjects.Workflow.DataObjects.UserRelativeProcessMatrices.sql");

            sql = string.Format(sql, TSqlBuilder.Instance.CheckUnicodeQuotationMark(userID));

            return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
        }

        protected override void BeforeInnerUpdate(WfMatrix data, Dictionary<string, object> context)
        {
            context["MatrixRowsUsers"] = data.Rows.GenerateRowsDirectUsers();

            SaveWfMatrixDefinition(data.Definition);
        }

        protected override void AfterInnerUpdate(WfMatrix data, Dictionary<string, object> context)
        {
            DeleteRelatedData(data.MatrixID);

            WfMatrixRowUsersCollection rowsUsers = (WfMatrixRowUsersCollection)context["MatrixRowsUsers"];

            string rowsUsersSql = BuildRowsUsersSql(rowsUsers);

            if (rowsUsersSql.IsNotEmpty())
                DbHelper.RunSql(rowsUsersSql, GetConnectionName());

            StringBuilder sqlClause = new StringBuilder();

            foreach (var row in data.Rows)
            {
                if (sqlClause.Length > 0)
                    sqlClause.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sqlClause.Append(BuildInsertRowSql(data.MatrixID, row));

                foreach (var cell in row.Cells)
                {
                    sqlClause.Append(BuildInsertCellSql(data.MatrixID, row, cell));
                }
            }

            if (sqlClause.Length > 0)
                DbHelper.RunSql(sqlClause.ToString(), GetConnectionName());

            CacheNotifyData notifyData = new CacheNotifyData(typeof(WfMatrixProcessKeyCache), data.ProcessKey, CacheNotifyType.Invalid);

            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        protected override void AfterLoad(WfMatrixCollection data)
        {
            foreach (var matrix in data)
            {
                matrix.Definition = WfMatrixDefinitionAdapter.Instance.Get(matrix.Definition.Key);
            }
        }

        internal void FillMatrixRows(string matrixID, WfMatrixRowCollection rows)
        {
            WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();
            whereBuilder.AppendItem("R." + DB_FIELD_MATRIX_ID, matrixID);

            var ds = DbHelper.RunSqlReturnDS(SELECT_SQL_CLAUSE + whereBuilder.ToSqlString(TSqlBuilder.Instance));

            rows.CopyFrom(ds.Tables[0].DefaultView);
        }

        protected override int InnerDelete(WfMatrix data, Dictionary<string, object> context)
        {
            return DeleteRelatedData(data.MatrixID);
        }

        protected override string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }

        public int Delete(string wfMatrixID)
        {
            WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();
            whereBuilder.AppendItem(DB_FIELD_MATRIX_ID, wfMatrixID);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(DELETE_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strBuilder.Append(DELETE_MR_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strBuilder.Append(DELETE_MC_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strBuilder.Append(DELETE_MU_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            return DbHelper.RunSqlWithTransaction(strBuilder.ToString());
        }

        public int DeleteByProcessKey(string processKey)
        {
            var matrix = LoadByProcessKey(processKey, false);

            if (matrix == null)
                return 0;

            return Delete(matrix.MatrixID);
        }

        /// <summary>
        /// 按照矩阵维度和维度值查询
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public WfMatrix Query(WfMatrixQueryParamCollection queryParams)
        {
            var matrixIdParam = queryParams.Find(p => p.QueryName == DB_FIELD_MATRIX_ID);

            if (matrixIdParam == null)
            {
                throw new ArgumentException("参数至少包括要查询的矩阵ID");
            }

            var matrixCollection = Load(p => p.AppendItem(matrixIdParam.QueryName, matrixIdParam.QueryValue));

            if (matrixCollection.Count == 0)
                return null;

            var result = matrixCollection.First();

            var selectClause = BuildSqlClause(queryParams);
            var ds = DbHelper.RunSqlReturnDS(selectClause.ToString());

            result.Loaded = false;
            result.Rows.CopyFrom(ds.Tables[0].DefaultView);
            result.Loaded = true;

            return result;
        }

        public override void ClearAll()
        {
            base.ClearAll();

            CacheNotifyData notifyData = new CacheNotifyData(typeof(WfMatrixProcessKeyCache), null, CacheNotifyType.Clear);

            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        #region private method
        private static string BuildInsertCellSql(string wfMatrixID, WfMatrixRow row, WfMatrixCell cell)
        {
            StringBuilder result = new StringBuilder();
            InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();

            insertBuilder.AppendItem(DB_FIELD_MATRIX_ID, wfMatrixID);
            insertBuilder.AppendItem(DB_FIELD_MATRIX_ROW_ID, row.RowNumber);
            insertBuilder.AppendItem(DB_FIELD_DIMENSION_KEY, cell.Definition.DimensionKey);
            insertBuilder.AppendItem(DB_FIELD_STRING_VALUE, cell.StringValue);

            result.Append(INSERT_MC_SQL_CLAUSE_PREFIX);
            result.Append(insertBuilder.ToSqlString(TSqlBuilder.Instance));
            result.Append(TSqlBuilder.Instance.DBStatementSeperator);

            return result.ToString();
        }

        private static string BuildInsertRowSql(string wfMatrixID, WfMatrixRow row)
        {
            StringBuilder result = new StringBuilder();
            InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();

            insertBuilder.AppendItem(DB_FIELD_MATRIX_ID, wfMatrixID);
            insertBuilder.AppendItem(DB_FIELD_MATRIX_ROW_ID, row.RowNumber);
            insertBuilder.AppendItem(DB_FIELD_OPERATOR_TYPE, (int)row.OperatorType);
            insertBuilder.AppendItem(DB_FIELD_OPERATOR, row.Operator);

            result.Append(INSERT_MR_SQL_CLAUSE_PREFIX);
            result.Append(insertBuilder.ToSqlString(TSqlBuilder.Instance));
            result.Append(TSqlBuilder.Instance.DBStatementSeperator);

            return result.ToString();
        }

        private static int DeleteRelatedData(string wfMatrixID)
        {
            WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();
            whereBuilder.AppendItem(DB_FIELD_MATRIX_ID, wfMatrixID);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(DELETE_MR_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strBuilder.Append(DELETE_MC_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            strBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strBuilder.Append(DELETE_MU_SQL_CLAUSE);
            strBuilder.Append(whereBuilder.ToSqlString(TSqlBuilder.Instance));

            return DbHelper.RunSql(strBuilder.ToString());
        }

        private static void SaveWfMatrixDefinition(WfMatrixDefinition def)
        {
            if (def == null) return;

            bool defIsExist = true;

            try
            {
                WfMatrixDefinitionAdapter.Instance.Load(def.Key);
            }
            catch (SystemSupportException)
            {
                defIsExist = false;
            }

            if (defIsExist == false)
            {
                WfMatrixDefinitionAdapter.Instance.Update(def);
            }
        }

        private static StringBuilder BuildSqlClause(WfMatrixQueryParamCollection queryParams)
        {
            var matrixIdParam = queryParams.Find(p => p.QueryName == DB_FIELD_MATRIX_ID);

            string inSqlClause = "";

            foreach (var para in queryParams)
            {
                if (para == matrixIdParam) continue;  //|| string.IsNullOrEmpty(para.QueryValue) 空值也作为条件 

                inSqlClause = BuilderSubSqlClause(para, matrixIdParam.QueryValue, inSqlClause);
            }

            StringBuilder result = new StringBuilder(SELECT_SQL_CLAUSE);
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("R." + matrixIdParam.QueryName, matrixIdParam.QueryValue);
            result.Append(builder.ToSqlString(TSqlBuilder.Instance));

            if (!string.IsNullOrEmpty(inSqlClause))
            {
                result.AppendFormat(" AND R.{0} IN ({1})", DB_FIELD_MATRIX_ROW_ID, inSqlClause);
            }
            return result;
        }

        private static string BuilderSubSqlClause(WfMatrixQueryParam para, string wfMatrixID, string inSql)
        {
            WhereSqlClauseBuilder matrixIdBuilder = new WhereSqlClauseBuilder();
            matrixIdBuilder.AppendItem(DB_FIELD_MATRIX_ID, wfMatrixID);

            WhereSqlClauseBuilder fieldBuilder = new WhereSqlClauseBuilder();
            fieldBuilder.AppendItem(DB_FIELD_DIMENSION_KEY, para.QueryName);
            fieldBuilder.AppendItem(DB_FIELD_STRING_VALUE, para.QueryValue);

            WhereSqlClauseBuilder fieldEmptyBuilder = new WhereSqlClauseBuilder();
            fieldEmptyBuilder.AppendItem(DB_FIELD_DIMENSION_KEY, para.QueryName);
            fieldEmptyBuilder.AppendItem(DB_FIELD_STRING_VALUE, "");

            ConnectiveSqlClauseCollection fieldClauseConnector = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
            fieldClauseConnector.Add(fieldBuilder);
            fieldClauseConnector.Add(fieldEmptyBuilder);

            ConnectiveSqlClauseCollection matrixIdConnector = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
            matrixIdConnector.Add(matrixIdBuilder);
            matrixIdConnector.Add(fieldClauseConnector);

            StringBuilder strBuilder = new StringBuilder(SELECT_MC_CLAUSE);

            strBuilder.Append(matrixIdConnector.ToSqlString(TSqlBuilder.Instance));

            if (!string.IsNullOrEmpty(inSql))
            {
                strBuilder.AppendFormat(" AND {0} IN ({1})", DB_FIELD_MATRIX_ROW_ID, inSql);
            }

            return strBuilder.ToString();
        }

        private static string BuildRowsUsersSql(WfMatrixRowUsersCollection rowsUsers)
        {
            StringBuilder strB = new StringBuilder();

            foreach (WfMatrixRowUsers rowUsers in rowsUsers)
            {
                foreach (IUser user in rowUsers.Users)
                {
                    InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

                    builder.AppendItem("MATRIX_ID", rowUsers.Row.Matrix.MatrixID);
                    builder.AppendItem("MATRIX_ROW_ID", rowUsers.Row.RowNumber);
                    builder.AppendItem("USER_ID", user.ID);
                    builder.AppendItem("USER_NAME", user.DisplayName);

                    if (strB.Length > 0)
                        strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    strB.AppendFormat("INSERT INTO WF.MATRIX_ROWS_USERS {0}", builder.ToSqlString(TSqlBuilder.Instance));
                }
            }

            return strB.ToString();
        }
        #endregion
    }

    internal class WfMatrixProcessKeyCache : CacheQueue<string, WfMatrix>
    {
        public static readonly WfMatrixProcessKeyCache Instance = CacheManager.GetInstance<WfMatrixProcessKeyCache>();
    }
}
