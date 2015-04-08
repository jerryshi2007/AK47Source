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
using System.Linq;
using System.Text;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
    public class SOARolePropertyDefinitionAdapter
    {
        public static readonly SOARolePropertyDefinitionAdapter Instance = new SOARolePropertyDefinitionAdapter();

        private SOARolePropertyDefinitionAdapter()
        {
        }

        public SOARolePropertyDefinitionCollection GetByRole(IRole role)
        {
            role.NullCheck("role");

            return GetByRoleID(role.ID);
        }

        public SOARolePropertyDefinitionCollection GetByRoleID(string roleID)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");

            SOARolePropertyDefinitionCollection result = SOARolePropertiesDefinitionCache.Instance.GetOrAddNewValue(roleID.ToRoleIDCacheKey(), (cache, key) =>
            {
                SOARolePropertyDefinitionCollection properties = LoadByRoleID(roleID);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, properties, dependency);

                return properties;
            });

            return result;
        }

        public SOARolePropertyDefinitionCollection LoadByRole(IRole role)
        {
            role.NullCheck("role");

            return LoadByRoleID(role.ID);
        }

        public SOARolePropertyDefinitionCollection LoadByRoleID(string roleID)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");

            string sql = string.Format("SELECT * FROM WF.ROLE_PROPERTIES_DEFINITIONS WHERE {0} ORDER BY SORT_ORDER",
                roleID.ToRoleIDCriteria());

            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
            {
                DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

                SOARolePropertyDefinitionCollection result = new SOARolePropertyDefinitionCollection();

                foreach (DataRow row in table.Rows)
                {
                    SOARolePropertyDefinition property = new SOARolePropertyDefinition();

                    ORMapping.DataRowToObject(row, property);

                    result.Add(property);
                }

                return result;
            }
        }

        /// <summary>
        /// 根据角色ID判断哪些角色有了扩展属性定义
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public Dictionary<string, bool> AreExist(IEnumerable<string> roleIDs)
        {
            roleIDs.NullCheck("roleIDs");

            Dictionary<string, bool> result = new Dictionary<string, bool>();

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
                WhereSqlClauseBuilder tenantBuilder = new WhereSqlClauseBuilder();

                tenantBuilder.AppendTenantCode();

                string tenantCriteria = string.Empty;

                if (tenantBuilder.IsEmpty == false)
                    tenantCriteria = " AND " + tenantBuilder.ToSqlString(TSqlBuilder.Instance);

                string sql = string.Format("SELECT ROLE_ID FROM WF.ROLE_PROPERTIES_DEFINITIONS WHERE ROLE_ID IN ({0}){1} GROUP BY ROLE_ID",
                    builder.ToSqlString(TSqlBuilder.Instance), tenantCriteria);

                DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

                foreach (DataRow row in table.Rows)
                    result[row["ROLE_ID"].ToString()] = true;
            }

            return result;
        }

        public void Update(IRole role, SOARolePropertyDefinitionCollection properties)
        {
            role.NullCheck("role");
            properties.NullCheck("properties");

            this.Update(role.ID, properties);
        }

        public void Update(string roleID, SOARolePropertyDefinitionCollection properties)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");
            properties.NullCheck("properties");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_DEFINITIONS WHERE {0}", roleID.ToRoleIDCriteria());

            foreach (SOARolePropertyDefinition property in properties)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                property.RoleID = roleID;

                strB.AppendFormat(ORMapping.GetInsertSql(property, TSqlBuilder.Instance));
            }

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                DbHelper.RunSql(strB.ToString(), GetConnectionName());

                scope.Complete();
            }

            string cacheKey = roleID.ToRoleIDCacheKey();

            CacheNotifyData notifyData1 = new CacheNotifyData(typeof(SOARolePropertiesDefinitionCache), cacheKey, CacheNotifyType.Invalid);
            CacheNotifyData notifyData2 = new CacheNotifyData(typeof(SOARolePropertiesCache), cacheKey, CacheNotifyType.Invalid);

            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData1, notifyData2);
            MmfCacheNotifier.Instance.SendNotify(notifyData1, notifyData2);
        }

        public void Delete(IRole role)
        {
            role.NullCheck("role");

            this.Delete(role.ID);
        }

        public void Delete(string roleID)
        {
            roleID.CheckStringIsNullOrEmpty("roleID");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_DEFINITIONS WHERE {0}", roleID.ToRoleIDCriteria());

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_ROWS WHERE {0}", roleID.ToRoleIDCriteria());

            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

            strB.AppendFormat("DELETE WF.ROLE_PROPERTIES_CELLS WHERE {0}", roleID.ToRoleIDCriteria());

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                DbHelper.RunSql(strB.ToString(), GetConnectionName());

                scope.Complete();
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void SetDirty()
        {
            SOARolePropertiesDefinitionCache.Instance.Clear();
        }

        private static string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
