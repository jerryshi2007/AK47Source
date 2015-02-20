using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.DataSources
{
    public class GroupMembersQueryAdapter : ObjectDataSourceQueryAdapterBase<Member, MemberCollection>
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
            qc.SelectFields = @"g.Name,m.*";
            qc.FromClause = " Biz.ConditionalGroups g INNER JOIN Biz.GroupsAndMembers gm ON g.GroupID=gm.GroupID INNER JOIN Biz.Members m ON gm.MemberID=m.MemberID ";
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
                qc.OrderByClause = "m.MemberID";
            else
                qc.OrderByClause = string.Format("{0}, m.MemberID", qc.OrderByClause);

            return qc.OrderByClause;
        }

        protected override void OnDataRowToObject(MemberCollection dataCollection, System.Data.DataRow row)
        {
            Member data = new Member();
            ORMappingItemCollection mapping = GetMappingInfo();
            ORMapping.DataRowToObject(row, mapping, data);
            data.GroupName = row["Name"].ToString();
            dataCollection.Add(data);
        }
    }
}