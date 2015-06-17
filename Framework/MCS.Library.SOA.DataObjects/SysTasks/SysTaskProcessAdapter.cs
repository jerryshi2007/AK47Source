using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
    public class SysTaskProcessAdapter : UpdatableAndLoadableAdapterBase<SysTaskProcess, SysTaskProcessCollection>
    {
        public readonly static SysTaskProcessAdapter Instance = new SysTaskProcessAdapter();

        private SysTaskProcessAdapter()
        {
        }

        public SysTaskProcess Load(string processID)
        {
            processID.NullCheck("processID");

            SysTaskProcessCollection processes = this.LoadByInBuilder(builder =>
            {
                builder.DataField = "ID";
                builder.AppendItem(processID);
            });

            (processes.Count > 0).FalseThrow("不能找到ID为{0}的任务流程", processID);

            return processes[0];
        }

        public SysTaskProcess LoadByActivityID(string activityID)
        {
            activityID.NullCheck("activityID");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("A.ID", activityID);
            builder.AppendTenantCode("A.TENANT_CODE");

            string sql = string.Format("SELECT P.* FROM WF.SYS_TASK_PROCESS P INNER JOIN WF.SYS_TASK_ACTIVITY A ON P.ID = A.PROCESS_ID AND P.TENANT_CODE = A.TENANT_CODE WHERE {0}",
                builder.ToSqlString(TSqlBuilder.Instance));

            SysTaskProcessCollection processes = this.QueryData(sql);

            (processes.Count > 0).FalseThrow("不能找到活动ID{0}对应的任务流程", activityID);

            this.AfterLoad(processes);

            return processes[0];
        }

        public SysTaskProcessCollection LoadByOwnerActivityID(string ownerActivityID)
        {
            ownerActivityID.NullCheck("ownerActivityID");

            return this.LoadByInBuilder(inBuilder =>
                        {
                            inBuilder.DataField = "OWNER_ACTIVITY_ID";
                            inBuilder.AppendItem(ownerActivityID);
                        },
                        orderBuilder =>
                        {
                            orderBuilder.AppendItem("SEQUENCE", FieldSortDirection.Ascending);
                        });
        }

        protected override void AfterLoad(SysTaskProcessCollection data)
        {
            base.AfterLoad(data);

            data.ForEach(p => p.Loaded = true);
        }

        protected override int InnerUpdate(SysTaskProcess data, Dictionary<string, object> context)
        {
            int count = base.InnerUpdate(data, context);

            if (count == 0 && data.UpdateTag > 0)
                throw new ApplicationException(string.Format("任务流程{0}的状态已经改变，不能更新", data.ID));

            return count;
        }

        protected override string GetUpdateSql(SysTaskProcess data, ORMappingItemCollection mappings, Dictionary<string, object> context)
        {
            UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, mappings, "UpdateTag");
            uBuilder.AppendItem("UPDATE_TAG", "UPDATE_TAG + 1", "=", true);

            WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data, mappings);
            wBuilder.AppendItem("UPDATE_TAG", data.UpdateTag);

            return string.Format("UPDATE {0} SET {1} WHERE {2}",
                mappings.TableName,
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        protected override string GetConnectionName()
        {
            return WorkflowSettings.GetConfig().ConnectionName;
        }
    }
}
