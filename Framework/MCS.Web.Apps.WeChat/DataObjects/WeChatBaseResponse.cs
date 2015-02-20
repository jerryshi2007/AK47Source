using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
	public class WeChatBaseResponse
	{
		public int Ret
		{
			get;
			set;
		}

		public string ErrMsg
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("Ret: {0}, ErrMsg: {1}", this.Ret, this.ErrMsg);
		}
	}
}
