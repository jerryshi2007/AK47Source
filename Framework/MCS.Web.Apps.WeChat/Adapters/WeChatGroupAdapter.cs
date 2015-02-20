using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Web.Apps.WeChat.Adapters
{
	public class WeChatGroupAdapter : WeChatObjectAdapterBase<WeChatGroup, WeChatGroupCollection>
	{
		public static readonly WeChatGroupAdapter Instance = new WeChatGroupAdapter();

		private WeChatGroupAdapter()
		{
		}

		public WeChatGroup Load(string accountID, int groupID)
		{
			accountID.CheckStringIsNullOrEmpty("accountID");

			return Load(builder =>
				{
					builder.AppendItem("AccountID", accountID);
					builder.AppendItem("GroupID", groupID);
				}).FirstOrDefault();
		}

        public WeChatGroupCollection LoadAll()
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("1", 1);

            return base.LoadByBuilder(builder);
        }
    }
}
