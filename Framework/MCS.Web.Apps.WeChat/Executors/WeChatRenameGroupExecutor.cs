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
	public class WeChatRenameGroupExecutor : WeChatModifyGroupExecutorBase
	{
		public WeChatRenameGroupExecutor(int groupID, string name, WeChatLoginInfo loginInfo)
			: base("rename", loginInfo)
		{
			(groupID >= 0).FalseThrow("组号必须大于等于0");
			name.CheckStringIsNullOrEmpty("name");

			this.GroupID = groupID;
			this.Name = name;
		}

		public int GroupID
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.Name.CheckStringIsNullOrEmpty("Name");

			postParams["id"] = this.GroupID.ToString();
			postParams["name"] = this.Name;
		}
	}
}
