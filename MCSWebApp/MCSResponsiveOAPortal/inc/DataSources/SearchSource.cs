using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using System.Data;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCSResponsiveOAPortal.DataSources
{
    public class SearchSource : ObjectDataSourceQueryAdapterBase<AppCommonInfo, AppCommonInfoCollection>
    {
        protected override string GetConnectionName()
        {
            return ConnectionNameMappingSettings.GetConfig().GetConnectionName("FullTextSearch", string.Empty);
        }

        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            //qc.SelectFields = "[APPLICATION_NAME], [SUBJECT], [CONTENT], [URL], [CREATE_TIME], ACI.[RESOURCE_ID] AS ACI_RESOURCE_ID, CIM.[PROCESS_ID] AS RESOURCE_ID";
            //qc.FromClause = "WF.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK)  LEFT JOIN [WF].[COMMON_INFO_MAPPING] AS CIM ON  ACI.RESOURCE_ID = CIM.COMMON_INFO_ID ";
            qc.SelectFields = "[APPLICATION_NAME], [PROGRAM_NAME], [SUBJECT], [CONTENT], [URL], [CREATE_TIME], [RESOURCE_ID]";
            qc.FromClause = "WF.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK)";
            qc.OrderByClause = "CREATE_TIME DESC";

            base.OnBuildQueryCondition(qc);
        }
    }

    public class FullTextSearcher
    {
        public static readonly FullTextSearcher Instance = new FullTextSearcher();

        private FullTextSearcher()
        {
        }

        public DataSet SearchFullDataByQuery(string query)
        {
            Object result = null;

            int cacheKey = query.GetHashCode();

            if (!ObjectContextCache.Instance.TryGetValue(cacheKey, out result))
            {
                if (result == null)
                {
                    result = GetSearchData(query);
                    ObjectContextCache.Instance.Add(cacheKey, result);
                }
            }

            return (DataSet)result;
        }

        protected DataSet GetSearchData(string query)
        {
            //string sql = string.Format("SELECT ACI.[RESOURCE_ID], CIM.[PROCESS_ID],[URL] FROM WF.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK)  LEFT JOIN [WF].[COMMON_INFO_MAPPING] AS CIM (NOLOCK) ON  ACI.RESOURCE_ID = CIM.COMMON_INFO_ID WHERE  CONTAINS(ACI.*, {0})",query);

            //联接PROCESS_INSTANCES的方式
            string sql = string.Format("SELECT ACI.[RESOURCE_ID], CIM.[PROCESS_ID],ACI.[URL],PSI.[INSTANCE_ID],PSI.[OWNER_ACTIVITY_ID] FROM WF.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK)  LEFT JOIN [WF].[COMMON_INFO_MAPPING] AS CIM (NOLOCK) ON  ACI.RESOURCE_ID = CIM.COMMON_INFO_ID LEFT JOIN [MCS_WORKFLOW].[WF].[PROCESS_INSTANCES] AS PSI (NOLOCK) ON CIM.RESOURCE_ID = PSI.RESOURCE_ID WHERE  CONTAINS(ACI.*, {0})", query);

            return DbHelper.RunSqlReturnDS(sql, GetConnectionName());
        }

        protected string GetConnectionName()
        {
            return ConnectionNameMappingSettings.GetConfig().GetConnectionName("FullTextSearch", WorkflowSettings.GetConfig().ConnectionName);
        }

    }
}