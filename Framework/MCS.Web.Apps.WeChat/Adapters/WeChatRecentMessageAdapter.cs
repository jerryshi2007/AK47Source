using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Adapters
{
    public class WeChatRecentMessageAdapter : UpdatableAndLoadableAdapterBase<WeChatRecentMessage, WeChatRecentMessageCollection>
    {
        public static readonly WeChatRecentMessageAdapter Instance = new WeChatRecentMessageAdapter();

        private WeChatRecentMessageAdapter()
        {
        }

        protected override string GetConnectionName()
        {
			return ConnectionDefine.WeChatInfoDBConnectionName;
        }

        public WeChatRecentMessage Load(string accountID, long messageID)
        {
            return this.Load(p =>
                {
                    p.AppendItem("AccountID", accountID);
                    p.AppendItem("MessageID", messageID);
                }).FirstOrDefault();
        }
    }
}
