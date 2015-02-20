using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
	public class WeChatChangeFriendsRetInfo
	{
		public string ret
		{
			get;
			set;
		}

		public WeChatChangeFriendsRetInfoItem[] result
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WeChatChangeFriendsRetInfoItem
	{
		public string faleId
		{
			get;
			set;
		}

		public string ret
		{
			get;
			set;
		}
	}
}
