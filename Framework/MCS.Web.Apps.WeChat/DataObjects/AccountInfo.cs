using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
	[ORTableMapping("Config.AccountInfo")]
	public class AccountInfo
	{
		[ORFieldMapping("AccountID", PrimaryKey = true)]
		public string AccountID
		{
			get;
			set;
		}

		[ORFieldMapping("UserID")]
		public string UserID
		{
			get;
			set;
		}

		[ORFieldMapping("Password")]
		public string Password
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("Account ID: {0}, User ID: {1}", this.AccountID, this.UserID);
		}
	}

	[Serializable]
	public class AccountInfoCollection : EditableDataObjectCollectionBase<AccountInfo>
	{
	}
}
