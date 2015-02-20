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
	public class WeChatMassSendImageMessageExecutor : WeChatMassSendMessageExecutorBase
	{
		public WeChatMassSendImageMessageExecutor(int groupID, string fileID, WeChatLoginInfo loginInfo)
			: base(groupID, WeChatAppMessageType.Image, loginInfo)
		{
			fileID.IsNotEmpty().FalseThrow("待发送的图片ID不能为空");

			this.FileID = fileID;
		}

		public string FileID
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.FileID.CheckStringIsNullOrEmpty("FileID");

			postParams["fileid"] = this.FileID;
		}
	}
}
