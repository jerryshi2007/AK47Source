using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.DataSources
{
    public class ConditionalGroupQueryAdapter : ObjectDataSourceQueryAdapterBase<ConditionalGroup, ConditionalGroupCollection>
    {

        protected override string GetConnectionName()
        {
            return ConnectionDefine.WeChatInfoDBConnectionName;
        }

        /// <summary>
        /// 指定查询条件
        /// </summary>
        /// <param name="qc"></param>
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.SelectFields = @"GroupID,Name,Description";
            qc.FromClause = ORMapping.GetMappingInfo<ConditionalGroup>().TableName;
            qc.OrderByClause = GetOrderByString(qc);
            base.OnBuildQueryCondition(qc);
        }

        /// <summary>
        /// 获取排序字串
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        protected string GetOrderByString(QueryCondition qc)
        {
            //排序
            if (string.IsNullOrEmpty(qc.OrderByClause))
                qc.OrderByClause = "GroupID";
            else
                qc.OrderByClause = string.Format("{0}, GroupID", qc.OrderByClause);

            return qc.OrderByClause;
        }
    }
}