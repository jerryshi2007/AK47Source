using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 微信上传文件的返回结果
	/// {"base_resp":{"ret":0,"err_msg":"ok"},"location":"bizfile","type":"image","content":"200452361"}
	/// </summary>
	[Serializable]
	public class WeChatUploadFileRetInfo : WeChatRetInfoWithBaseResponse
	{
		public string Location
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public int Content
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("Ret: {0}\nErrMsg: {1}\nLocation: {2}\nType: {3}\nContent: {4}",
				this.BaseResponse.Ret, this.BaseResponse.ErrMsg, this.Location, this.Type, this.Content);
		}
	}
}
