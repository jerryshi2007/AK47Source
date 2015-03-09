using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
    public class SOARolePropertiesAdapter
    {
        public static readonly SOARolePropertiesAdapter Instance = new SOARolePropertiesAdapter();

        private SOARolePropertiesAdapter()
        {
        }

        public SOARolePropertyRowCollection GetByRole(IRole role)
        {
            role.NullCheck("role");

            SOARolePropertyRowCollection result = SOARolePropertiesCache.Instance.GetOrAddNewValue(role.ID, (cache, key) =>
            {
                SOARolePropertyRowCollection properties = LoadByRole(role);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, properties, dependency);

                return properties;
            });

            return result;
        }

        public SOARolePropertyRowCollection GetByRoleID(string roleID)
        {
            roleID.NullCheck("roleID");

            SOARolePropertyRowCollection result = SOARolePropertiesCache.Instance.GetOrAddNewValue(roleID, (cache, key) =>
            {
                SOARolePropertyRowCollection properties = LoadByRoleID(roleID, null);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, properties, dependency);

                return properties;
            });

            return result;
        }

        public SOARolePropertyRowCollection GetByRole(IRole role, SOARolePropertyDefinitionCollection definition)
        {
            role.NullCheck("role");

            SOARolePropertyRowCollection result = SOARolePropertiesCache.Instance.GetOrAddNewValue(role.ID, (cache, key) =>
            {
                SOARolePropertyRowCollection properties = LoadByRole(role, definition);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, properties, dependency);

                return properties;
            });

            return result;
        }


        public SOARolePropertyRowCollection LoadByRole(IRole role)
        {
            role.NullCheck("role");

            return LoadByRoleID(role.ID, role);
        }

        public SOARolePropertyRowCollection LoadByRole(IRole role, SOARolePropertyDefinitionCollection definition)
        {
            role.NullCheck("role");

            return LoadByRoleID(role.ID, role, definition);
        }

        /// <summary>
        /// 用户或角色直接所属的角色矩阵ID
        /// </summary>
        /// <param name="operatorIDs">用户或角色所属的角色矩阵ID集合</param>
        /// <returns></returns>
        public List<string> OperatorBelongToRoleIDsDirectly(params string[] operatorIDs)
        {
            operatorIDs.NullCheck("userIDs");

            List<string> result = new List<string>();

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("OPERATOR_ID");

            inBuilder.AppendItem(operatorIDs);

            if (inBuilder.IsEmpty == false)
            {
                string sql = string.Format("SELECT ROLE_ID FROM WF.ROLE_PROPERTIES_USER_CONTAINERS WHERE {0}", inBuilder.ToSqlString(TSqlBuilder.Instance));

                using (DbContext context = DbContext.GetContext(GetConnectionName()))
                {
                    Database db = DatabaseFactory.Create(context);

                    DbCommand cmd = db.GetSqlStringCommand(sql);

                    using (var dr = db.ExecuteReader(cmd))
                    {
                        while (dr.Read())
                            result.Add(dr.GetString(0));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 根据RoleID加载行信息
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="role"></param>
        /// <param name="definition">列定义</param>
        /// <returns></returns>
        public SOARolePropertyRowCollection LoadByRoleID(string roleID, IRole role, SOARolePropertyDefinitionCollection definition)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");
            definition.NullCheck("definition");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("SELECT * FROM WF.ROLE_PROPERTIES_ROWS WHERE ROLE_ID = {0} ORDER BY ROW_NUMBER",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("SELECT * FROM WF.ROLE_PROPERTIES_CELLS WHERE ROLE_ID = {0} ORDER BY PROPERTIES_ROW_ID",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection(role);

            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
            {
                DataSet ds = DbHelper.RunSqlReturnDS(strB.ToString(), GetConnectionName());
                Dictionary<int, SOARolePropertyValueCollection> propertyValues = SOARolePropertyValueCollection.LoadAndGroup(ds.Tables[1].DefaultView, definition);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SOARolePropertyRow property = new SOARolePropertyRow(role);

                    ORMapping.DataRowToObject(row, property);

                    SOARolePropertyValueCollection values = null;

                    if (propertyValues.TryGetValue(property.RowNumber, out values))
                        property.Values.CopyFrom(values);

                    result.Add(property);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据roleID加载角色矩阵。role参数可以为空
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public SOARolePropertyRowCollection LoadByRoleID(string roleID, IRole role)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");

            if (role == null)
                role = new SOARole() { ID = roleID };

            SOARolePropertyDefinitionCollection definition = SOARolePropertyDefinitionAdapter.Instance.GetByRoleID(roleID);

            return LoadByRoleID(roleID, role, definition);
        }

        /// <summary>
        /// 根据角色ID判断哪些角色有了扩展属性
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public Dictionary<string, bool> AreExist(IEnumerable<string> roleIDs)
        {
            roleIDs.NullCheck("roleIDs");

            Dictionary<string, bool> result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            InSqlClauseBuilder builder = new InSqlClauseBuilder();

            foreach (string roleID in roleIDs)
            {
                if (result.ContainsKey(roleID) == false)
                {
                    result.Add(roleID, false);
                    builder.AppendItem(roleID);
                }
            }

            if (result.Count > 0)
            {
                string sql = string.Format("SELECT ROLE_ID FROM WF.ROLE_PROPERTIES_ROWS WHERE ROLE_ID IN ({0}) GROUP BY ROLE_ID",
                    builder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

                foreach (DataRow row in table.Rows)
                    result[row["ROLE_ID"].ToString()] = true;
            }

            return result;
        }

        public void Update(SOARole role)
        {
            role.NullCheck("role");

            Update(role.ID, role.Rows);
        }

        public void Update(string roleID, SOARolePropertyRowCollection rows)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");
            rows.NullCheck("rows");

            StringBuilder strB = new StringBuilder(1024);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_ROWS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_CELLS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_USER_CONTAINERS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            PrepareRowsSql(roleID, rows, strB);

            SOARolePropertyRowUsersCollection rowsUsers = rows.GenerateRowsUsersDirectly();
            SOARolePropertyRowRolesCollection rowsRoles = rows.GenerateRowsRolesDirectly();

            PrepareUserContainers(roleID, rowsUsers, strB);
            PrepareRoleContainers(roleID, rowsRoles, strB);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                DbHelper.RunSql(strB.ToString(), GetConnectionName());
                scope.Complete();
            }

            CacheNotifyData notifyData = new CacheNotifyData(typeof(SOARolePropertiesCache), roleID, CacheNotifyType.Invalid);
            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        public void Delete(SOARole role)
        {
            role.NullCheck("role");

            this.Delete(role.ID);
        }

        public void Delete(string roleID)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");

            StringBuilder strB = new StringBuilder(1024);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_ROWS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_CELLS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_USER_CONTAINERS WHERE ROLE_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(roleID, true));

            DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());

            CacheNotifyData notifyData = new CacheNotifyData(typeof(SOARolePropertiesCache), roleID, CacheNotifyType.Invalid);
            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }


        private void PrepareUserContainers(string roleID, SOARolePropertyRowUsersCollection rowsUsers, StringBuilder strB)
        {
            foreach (SOARolePropertyRowUsers rowUsers in rowsUsers)
            {
                foreach (IUser user in rowUsers.Users)
                {
                    InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    builder.AppendItem("ROLE_ID", roleID);
                    builder.AppendItem("ROW_NUMBER", rowUsers.Row.RowNumber);
                    builder.AppendItem("OPERATOR_TYPE", (int)rowUsers.Row.OperatorType);
                    builder.AppendItem("OPERATOR_ID", user.ID);
                    builder.AppendItem("OPERATOR_NAME", user.DisplayName.IsNotEmpty() ? user.DisplayName : user.Name);

                    builder.AppendTenantCode();

                    string sql = string.Format("INSERT INTO WF.ROLE_PROPERTIES_USER_CONTAINERS {0}", builder.ToSqlString(TSqlBuilder.Instance));

                    strB.Append(sql);
                }
            }
        }

        private void PrepareRoleContainers(string roleID, SOARolePropertyRowRolesCollection rowsRoles, StringBuilder strB)
        {
            foreach (SOARolePropertyRowRoles rowRoles in rowsRoles)
            {
                foreach (IRole role in rowRoles.Roles)
                {
                    InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    builder.AppendItem("ROLE_ID", roleID);
                    builder.AppendItem("ROW_NUMBER", rowRoles.Row.RowNumber);
                    builder.AppendItem("OPERATOR_TYPE", (int)rowRoles.Row.OperatorType);
                    builder.AppendItem("OPERATOR_ID", role.ID);
                    builder.AppendItem("OPERATOR_NAME", role.FullCodeName);

                    builder.AppendTenantCode();

                    string sql = string.Format("INSERT INTO WF.ROLE_PROPERTIES_USER_CONTAINERS {0}", builder.ToSqlString(TSqlBuilder.Instance));

                    strB.Append(sql);
                }
            }
        }

        private static void PrepareRowsSql(string roleID, SOARolePropertyRowCollection rows, StringBuilder strB)
        {
            foreach (SOARolePropertyRow row in rows)
            {
                strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(PrepareRowSql(roleID, row));

                foreach (SOARolePropertyValue propValue in row.Values)
                {
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                    strB.Append(PrepareValueSql(roleID, row, propValue));
                }
            }
        }

        private static string PrepareRowSql(string roleID, SOARolePropertyRow row)
        {
            InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(row);

            builder.AppendItem("ROLE_ID", roleID);

            return "INSERT INTO WF.ROLE_PROPERTIES_ROWS" + builder.ToSqlString(TSqlBuilder.Instance);
        }

        private static string PrepareValueSql(string roleID, SOARolePropertyRow row, SOARolePropertyValue propValue)
        {
            InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(propValue);

            builder.AppendItem("ROLE_ID", roleID);
            builder.AppendItem("PROPERTIES_ROW_ID", row.RowNumber);
            builder.AppendItem("PROPERTY_NAME", propValue.Column.Name);

            return "INSERT INTO WF.ROLE_PROPERTIES_CELLS" + builder.ToSqlString(TSqlBuilder.Instance);
        }

        private static string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
