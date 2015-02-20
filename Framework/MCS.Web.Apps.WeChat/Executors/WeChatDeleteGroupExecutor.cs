using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatDeleteGroupExecutor : WeChatModifyGroupExecutorBase
	{
		public WeChatDeleteGroupExecutor(int groupID, WeChatLoginInfo loginInfo)
			: base("del", loginInfo)
		{
			(groupID >= 0).FalseThrow("组号必须大于等于0");

			this.GroupID = groupID;
		}

		public int GroupID
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			postParams["id"] = this.GroupID.ToString();
		}
	}
}
