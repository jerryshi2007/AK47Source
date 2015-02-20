using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 带Base Response的返回结果
	/// {"base_resp":{"ret":0,"err_msg":"ok"}}
	/// </summary>
	[Serializable]
	public abstract class WeChatRetInfoWithBaseResponse
	{
		private WeChatBaseResponse _BaseResponse = null;

		public WeChatBaseResponse BaseResponse
		{
			get
			{
				if (this._BaseResponse == null)
					this._BaseResponse = new WeChatBaseResponse();

				return this._BaseResponse;
			}
			set
			{
				this._BaseResponse = value;
			}
		}

		public void CheckResult()
		{
			if (this.BaseResponse != null)
			{
				if (this.BaseResponse.Ret != 0)
					throw new ApplicationException(this.BaseResponse.ToString());
			}
		}
	}
}
