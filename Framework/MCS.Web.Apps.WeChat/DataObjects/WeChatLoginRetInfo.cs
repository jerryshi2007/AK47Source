using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// {"base_resp":{"ret":0,"err_msg":"ok"},"redirect_url":"\/cgi-bin\/home?t=home\/index&lang=zh_CN&token=2118804718"}
	/// </summary>
	[Serializable]
	public class WeChatLoginRetInfo : WeChatRetInfoWithBaseResponse
	{
		public string RedirectUrl
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("Ret: {0}\nErrMsg: {1}\nReturnUrl: {2}",
				this.BaseResponse.Ret, this.BaseResponse.ErrMsg, this.RedirectUrl);
		}
	}
}
