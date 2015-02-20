using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Library.Data.Builder;

namespace MCS.Web.Apps.WeChat.Adapters
{
	public class MemberAdapter : UpdatableAndLoadableAdapterBase<Member, MemberCollection>
	{
		public static readonly MemberAdapter Instance = new MemberAdapter();

		private MemberAdapter()
		{
		}

		public MemberCollection LoadAll()
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("1", 1);

			return base.LoadByBuilder(builder);
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.WeChatInfoDBConnectionName;
		}
	}
}
