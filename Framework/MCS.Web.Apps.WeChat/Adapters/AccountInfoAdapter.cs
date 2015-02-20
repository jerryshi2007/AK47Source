using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Library.Data.Builder;

namespace MCS.Web.Apps.WeChat.Adapters
{
	public class AccountInfoAdapter : UpdatableAndLoadableAdapterBase<AccountInfo, AccountInfoCollection>
	{
		public static readonly AccountInfoAdapter Instance = new AccountInfoAdapter();

		private AccountInfoAdapter()
		{
		}

		public AccountInfoCollection LoadAll()
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
