using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// AppCommonInfo的数据访问类
    /// </summary>
    public class AppCommonInfoAdapter : UpdatableAndLoadableAdapterBase<AppCommonInfo, AppCommonInfoCollection>
    {
        public static readonly AppCommonInfoAdapter Instance = new AppCommonInfoAdapter();

        private AppCommonInfoAdapter()
        {
        }

        public AppCommonInfo Load(string resourceID)
        {
            return Load(resourceID, true);
        }

        public AppCommonInfo Load(string resourceID, bool throwError)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            AppCommonInfoCollection acis = Load(builder => builder.AppendItem("RESOURCE_ID", resourceID));

            if (throwError)
                (acis.Count > 0).FalseThrow("不能找到RESOURCE_ID为'{0}'的APPLICAITON_COMMON_INFO的记录", resourceID);

            return acis.FirstOrDefault();
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public bool Exists(string resourceID)
        {
            return this.Exists(builder => builder.AppendItem("RESOURCE_ID", resourceID));
            //resourceID.CheckStringIsNullOrEmpty("resourceID");

            //string sql = string.Format("SELECT RESOURCE_ID FROM WF.APPLICATIONS_COMMON_INFO WHERE RESOURCE_ID = {0}",
            //         TSqlBuilder.Instance.CheckQuotationMark(resourceID, true));


            //return DbHelper.RunSqlReturnScalar(sql, GetConnectionName()) != null;
        }

        /// <summary>
        /// 是否存在且没有归档
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public bool ExistsAndNotArchived(string resourceID)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("RESOURCE_ID", resourceID);
            builder.AppendTenantCode(typeof(AppCommonInfo));

            string sql = string.Format("SELECT ARCHIVE_STATUS FROM WF.APPLICATIONS_COMMON_INFO WHERE {0}",
                     builder.ToSqlString(TSqlBuilder.Instance));

            object result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());

            return result != null && result.ToString() != "1";
        }

        public void Update(AppCommonInfo aci, params string[] ignoredUpdateProperties)
        {
            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                AppCommonInfoSettings.GetConfig().Operations.ForEach(op => op.Update(aci, ignoredUpdateProperties));

                scope.Complete();
            }
        }

        public void UpdateProcessStatus(IEnumerable<IWfProcess> processes)
        {
            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                AppCommonInfoSettings.GetConfig().Operations.ForEach(op => op.UpdateProcessStatus(processes));

                scope.Complete();
            }
        }

        protected override string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
        }
    }
}
