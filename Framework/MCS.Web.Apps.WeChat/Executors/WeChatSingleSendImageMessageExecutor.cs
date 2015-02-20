using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Executors
{
	/// <summary>
	/// 发送图片消息给某个人
	/// </summary>
	public class WeChatSingleSendImageMessageExecutor : WeChatSingleSendMessageExecutorBase
	{
		public WeChatSingleSendImageMessageExecutor(string fakeID, string fileID, WeChatLoginInfo loginInfo)
			: base(fakeID, WeChatAppMessageType.Image, loginInfo)
		{
			fileID.CheckStringIsNullOrEmpty("fileID");

			this.FileID = fileID;
		}

		/// <summary>
		/// 图片素材的ID
		/// </summary>
		public string FileID
		{
			get;
			private set;
		}

		protected override void PreparePostParams(HttpWebRequest request, NameValueCollection postParams)
		{
			this.FileID.CheckStringIsNullOrEmpty("FileID");

			postParams["fileid"] = this.FileID;
			postParams["file_id"] = this.FileID;
		}
	}
}
