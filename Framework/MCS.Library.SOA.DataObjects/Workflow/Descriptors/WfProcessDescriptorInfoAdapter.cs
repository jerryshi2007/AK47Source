using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using System.Data;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfProcessDescriptorInfoAdapter : UpdatableAdapterBase<WfProcessDescriptorInfo>
    {
        public static readonly WfProcessDescriptorInfoAdapter Instance = new WfProcessDescriptorInfoAdapter();

        private WfProcessDescriptorInfoAdapter()
        {
        }

        public bool ExistsProcessKey(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WhereSqlClauseBuilder builder = PrepareProcessKeyWhereBuilder(processKey);

            string sql = string.Format("SELECT COUNT(*) FROM WF.PROCESS_DESCRIPTORS WHERE {0}",
                builder.ToSqlString(TSqlBuilder.Instance));

            int count = (int)DbHelper.RunSqlReturnScalar(sql);

            return count > 0;
        }

        public void UpdateImportTime(string processKey, IUser user)
        {
            WhereSqlClauseBuilder wbuilder = PrepareProcessKeyWhereBuilder(processKey);
            UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

            uBuilder.AppendItem("IMPORT_TIME", "GETDATE()", "=", true);

            if (user != null)
            {
                uBuilder.AppendItem("IMPORT_USER_ID", user.ID);
                uBuilder.AppendItem("IMPORT_USER_NAME", user.DisplayName);
            }

            string sql = string.Format("UPDATE [WF].[PROCESS_DESCRIPTORS] SET {0} WHERE {1}",
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wbuilder.ToSqlString(TSqlBuilder.Instance));

            DbHelper.RunSql(sql, this.GetConnectionName());
        }

        [Obsolete("已经废弃，流程保存的时候会自动更新")]
        public void UpdateModifyDate(string processKey, DateTime datetime, IUser user)
        {
            WhereSqlClauseBuilder builder = PrepareProcessKeyWhereBuilder(processKey);

            string sql = string.Format("UPDATE [WF].[PROCESS_DESCRIPTORS] SET [MODIFY_TIME] = {0}, [MODIFIER_ID] = '{1}', [MODIFIER_NAME] = '{2}' WHERE {3}",
                TSqlBuilder.Instance.FormatDateTime(datetime),
                user.ID,
                user.Name,
                builder.ToSqlString(TSqlBuilder.Instance));

            DbHelper.RunSql(sql, this.GetConnectionName());
        }

        public WfProcessDescriptorInfo Load(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WhereSqlClauseBuilder builder = PrepareProcessKeyWhereBuilder(processKey);

            string sql = string.Format("SELECT * FROM WF.PROCESS_DESCRIPTORS WHERE {0}",
                builder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql).Tables[0];

            (table.Rows.Count > 0).FalseThrow("不能根据\"{0}\"找到对应的流程定义", processKey);

            WfProcessDescriptorInfo processDescInfo = new WfProcessDescriptorInfo();

            ORMapping.DataRowToObject<WfProcessDescriptorInfo>(table.Rows[0], processDescInfo);

            return processDescInfo;
        }

        public void Delete(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            WhereSqlClauseBuilder builder = PrepareProcessKeyWhereBuilder(processKey);

            string sql = string.Format("DELETE FROM WF.PROCESS_DESCRIPTORS WHERE {0}",
                    builder.ToSqlString(TSqlBuilder.Instance));

            DbHelper.RunSql(sql);
        }

        public WfProcessDescriptorInfoCollection LoadWfProcessDescriptionInfos(Action<WhereSqlClauseBuilder> action, bool ignoreProcessData)
        {
            action.NullCheck("action");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendTenantCode();
            action(builder);

            string sqlFieldStr = ignoreProcessData ? ORMapping.GetSelectFieldsNameSql<WfProcessDescriptorInfo>("DATA") : "*";

            string sql = string.Format("SELECT {0} FROM WF.PROCESS_DESCRIPTORS WHERE {1}",
                   sqlFieldStr, builder.ToSqlString(TSqlBuilder.Instance));

            WfProcessDescriptorInfoCollection result = new WfProcessDescriptorInfoCollection();

            DataTable table = DbHelper.RunSqlReturnDS(sql).Tables[0];

            foreach (DataRow row in table.Rows)
            {
                WfProcessDescriptorInfo wfProcessDescInfo = new WfProcessDescriptorInfo();
                ORMapping.DataRowToObject(row, wfProcessDescInfo);

                result.Add(wfProcessDescInfo);
            }

            return result;
        }

        protected override void BeforeInnerUpdate(WfProcessDescriptorInfo data, Dictionary<string, object> context)
        {
            if (DeluxePrincipal.IsAuthenticated)
            {
                data.Creator = DeluxeIdentity.CurrentUser;
                data.Modifier = DeluxeIdentity.CurrentUser;
            }

            base.BeforeInnerUpdate(data, context);
        }

        protected override string GetUpdateSql(WfProcessDescriptorInfo data, ORMappingItemCollection mappings, Dictionary<string, object> context)
        {
            UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, mappings);
            uBuilder.AppendTenantCode();

            WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data, mappings);
            wBuilder.AppendTenantCode();

            return string.Format("UPDATE {0} SET {1} WHERE {2}",
                mappings.TableName,
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        protected override string GetInsertSql(WfProcessDescriptorInfo data, ORMappingItemCollection mappings, Dictionary<string, object> context)
        {
            InsertSqlClauseBuilder iBuilder = ORMapping.GetInsertSqlClauseBuilder(data, mappings);

            iBuilder.AppendTenantCode();

            return string.Format("INSERT INTO {0} {1}", mappings.TableName, iBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        private static WhereSqlClauseBuilder PrepareProcessKeyWhereBuilder(string processKey)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("PROCESS_KEY", processKey);
            builder.AppendTenantCode();

            return builder;
        }

        protected override string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
