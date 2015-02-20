using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfProcessDescriptorDimensionAdapter : UpdatableAndLoadableAdapterBase<WfProcessDescriptorDimension, WfProcessDescriptorDimensionCollection>
    {
        public static readonly WfProcessDescriptorDimensionAdapter Instance = new WfProcessDescriptorDimensionAdapter();

        private WfProcessDescriptorDimensionAdapter()
        {
        }

        public WfProcessDescriptorDimension Load(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            return LoadByInBuilder(builder =>
            {
                builder.DataField = "PROCESS_KEY";
                builder.AppendItem(processKey);
            }).FirstOrDefault();
        }

        public void DeleteByProcessKey(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            Delete(builder =>
            {
                builder.AppendItem("PROCESS_KEY", processKey);
                builder.AppendTenantCode();
            });
        }

        /// <summary>
        /// 查询用户相关的流程定义
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataTable QueryUserRelativeProcessDescriptors(string userID)
        {
            string sql = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                "MCS.Library.SOA.DataObjects.Workflow.DataObjects.UserRelativeProcessDescriptors.sql");

            sql = string.Format(sql, TSqlBuilder.Instance.CheckQuotationMark(userID, false));

            return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
        }

        protected override void BeforeInnerUpdate(WfProcessDescriptorDimension data, Dictionary<string, object> context)
        {
            data.UpdateTime = DateTime.MinValue;

            base.BeforeInnerUpdate(data, context);
        }

        protected override string GetInsertSql(WfProcessDescriptorDimension data, ORMappingItemCollection mappings, Dictionary<string, object> context)
        {
            InsertSqlClauseBuilder iBuilder = ORMapping.GetInsertSqlClauseBuilder(data, mappings);

            iBuilder.AppendTenantCode();

            return string.Format("INSERT INTO {0} {1}", mappings.TableName, iBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        protected override string GetUpdateSql(WfProcessDescriptorDimension data, ORMappingItemCollection mappings, Dictionary<string, object> context)
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
    }
}
