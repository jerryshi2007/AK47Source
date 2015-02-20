using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	public class WeChatSendMessageRetInfo
	{
		public int ret
		{
			get;
			set;
		}

		public string msg
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("ret: {0}, msg: {1}", this.ret, this.msg);
		}
	}
}
