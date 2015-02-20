using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Web.Apps.WeChat.Adapters
{
    public class WeChatFriendAdapter : WeChatObjectAdapterBase<WeChatFriend, WeChatFriendCollection>
    {
        public static readonly WeChatFriendAdapter Instance = new WeChatFriendAdapter();

        private WeChatFriendAdapter()
        {
        }

        public WeChatFriend Load(string fakeID)
        {
            fakeID.CheckStringIsNullOrEmpty("fakeID");

            return Load(builder =>
                {
                    builder.AppendItem("FakeID", fakeID);
                }).FirstOrDefault();
        }

        protected override string GetUpdateSql(WeChatFriend data, ORMappingItemCollection mappings, Dictionary<string, object> context)
        {
            UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, mappings, "OpenID");

            WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data, mappings);

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                mappings.TableName,
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wBuilder.ToSqlString(TSqlBuilder.Instance));

            return sql;
        }
       
        public IList<string> GetFakeIDsByLocalGroupID(string groupID)
        {
            List<string> result = new List<string>();

            WhereSqlClauseBuilder whereSqlClause = new WhereSqlClauseBuilder();
            whereSqlClause.AppendItem("gm.GroupID", groupID);

            string sqlStr = @"SELECT wf.FakeID FROM [Biz].[GroupsAndMembers] gm
	INNER JOIN [Biz].[MembersAndOpenID] mo
	on gm.MemberID=mo.MemberID
	INNER JOIN [WeChat].[Friends] wf
	on mo.OpenID=wf.OpenID
	Where " + whereSqlClause.ToSqlString(TSqlBuilder.Instance);

            var ds = DbHelper.RunSqlReturnDS(sqlStr, this.GetConnectionName());

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(row["FakeID"].ToString());
            }

            return result;
        }

        public void CalculateOpenIDFromMessages()
        {
            DbHelper.RunSql("WeChat.CalculateOpenIDFromMessages", this.GetConnectionName());
        }
    }
}
