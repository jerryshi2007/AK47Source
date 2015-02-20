using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class GetAppMessagesCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetAppMessagesCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatAppMessageType messageType = (WeChatAppMessageType)Enum.Parse(typeof(WeChatAppMessageType), argument, true);

			WeChatAppMessageCollection appMessages = WeChatHelper.GetAppMessages(messageType, WeChatRequestContext.Current.LoginInfo);

			appMessages.Output();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getAppMessages {type}";
			}
		}
	}
}
