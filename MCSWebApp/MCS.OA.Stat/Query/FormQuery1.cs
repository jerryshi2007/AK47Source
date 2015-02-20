using System.Web;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.OA.Stat.Query
{
    public class FormQuery1
    {
        /// <summary>
        /// 表单分页查询
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public AppCommonInfoCollection GetFormQueryWithCount(int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount)
        {
            if (string.IsNullOrEmpty(where))
                where = "1 = 1";

            if (string.IsNullOrEmpty(orderBy))
                orderBy = "CREATE_TIME DESC";

            if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin") == false)
            {
                ConnectiveSqlClauseCollection cscc = WfAclAdapter.Instance.GetAclQueryConditionsByUser(string.Empty);

                string resourceIDList = string.Format("SELECT RESOURCE_ID FROM WF.ACL WHERE {0}", cscc.ToSqlString(TSqlBuilder.Instance));

                where = string.Format("{0} AND ACI.RESOURCE_ID IN ({1})", where, resourceIDList);
            }

            //LDM 加上ACI.PROGRAM_NAME as [PROGRAM_NAME1]，
            //兼容远洋地产的查询（表单工作程序里没有对 APPLICATIONFORM_INFO 表插入数据行）
            string strSelect = @" ACI.APPLICATION_NAME,ACI.[PROGRAM_NAME] as [PROGRAM_NAME_MCS],ACI.RESOURCE_ID,ACI.[SUBJECT],ACI.[EMERGENCY]," +
                               "  ACI.URL,ACI.CREATOR_ID, ACI.CREATOR_NAME,ACI.CREATE_TIME,ACI.DRAFT_DEPARTMENT_NAME";

            string strfrom = @" WF.APPLICATIONS_COMMON_INFO as ACI (nolock)";
           

            QueryCondition qc = new QueryCondition(
                       startRowIndex,
                       maximumRows,
                       strSelect,
                       strfrom,
                       orderBy,
                       where
                       );
            CommonAdapter commonAdapter = new CommonAdapter(ConnectionDefine.SearchConnectionName);
            DataSet ds = commonAdapter.SplitPageQuery(qc, totalCount < 0);

            AppCommonInfoCollection FormQueryEntitys = new AppCommonInfoCollection();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AppCommonInfo entity = new AppCommonInfo();

                ORMapping.DataRowToObject(row, entity);
                FormQueryEntitys.Add(entity);
            }

            if (ds.Tables.Count > 1)
                totalCount = (int)ds.Tables[1].Rows[0][0];

            HttpContext.Current.Items["UserFormQueryEntityCount"] = totalCount;

            //当页码超出索引的，返回最大页
            if (FormQueryEntitys.Count == 0 && totalCount > 0)
            {
                int newStartRowIndex = (totalCount - 1) / maximumRows * maximumRows;

                totalCount = -1;

                FormQueryEntitys = GetFormQueryWithCount(newStartRowIndex, maximumRows, where, orderBy, ref totalCount);
            }

            return FormQueryEntitys;
        }

        public int GetFormQueryCount(string where, ref int totalCount)
        {
            return (int)HttpContext.Current.Items["UserFormQueryEntityCount"];
        }
    }
}
