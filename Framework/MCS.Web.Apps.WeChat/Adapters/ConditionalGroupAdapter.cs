using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Adapters
{
	public class ConditionalGroupAdapter : UpdatableAndLoadableAdapterBase<ConditionalGroup, ConditionalGroupCollection>
	{
		public static readonly ConditionalGroupAdapter Instance = new ConditionalGroupAdapter();

		private ConditionalGroupAdapter()
		{
		}

		public ConditionalGroup Load(string groupID)
		{
			groupID.CheckStringIsNullOrEmpty("groupID");

			return base.LoadByInBuilder(builder =>
				{
					builder.DataField = "GroupID";
					builder.AppendItem(groupID);
				}).FirstOrDefault();
		}

        protected override string GetConnectionName()
        {
            return ConnectionDefine.WeChatInfoDBConnectionName;
        }

        public ConditionalGroupCollection LoadAll()
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("1", 1);

            return base.LoadByBuilder(builder);
        }
    }
}
