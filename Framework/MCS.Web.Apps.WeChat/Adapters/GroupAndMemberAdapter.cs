using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Library.Data.Builder;

namespace MCS.Web.Apps.WeChat.Adapters
{
    public class GroupAndMemberAdapter : UpdatableAndLoadableAdapterBase<GroupAndMember, GroupAndMemberCollection>
    {
        public static readonly GroupAndMemberAdapter Instance = new GroupAndMemberAdapter();

        private GroupAndMemberAdapter()
        {
        }

        public GroupAndMemberCollection LoadByGroupID(string groupID)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("GroupID", groupID);

            return base.LoadByBuilder(builder);
        }

        public void DeleteByGroupID(string groupID)
        {
            base.Delete(p => p.AppendItem("GroupID", groupID));
        }

        protected override string GetConnectionName()
        {
            return ConnectionDefine.WeChatInfoDBConnectionName;
        }
    }
}
