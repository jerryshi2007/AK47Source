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
	public class WeChatAddGroupExecutor : WeChatModifyGroupExecutorBase
	{
		public WeChatAddGroupExecutor(string name, WeChatLoginInfo loginInfo)
			: base("add", loginInfo)
		{
			name.CheckStringIsNullOrEmpty("name");

			this.Name = name;
		}

		public string Name
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.Name.CheckStringIsNullOrEmpty("Name");

			postParams["name"] = this.Name;
		}
	}
}
