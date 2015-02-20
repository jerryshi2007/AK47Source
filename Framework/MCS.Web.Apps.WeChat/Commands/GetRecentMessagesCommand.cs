using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class GetRecentMessagesCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetRecentMessagesCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatRecentMessageCollection result = WeChatHelper.GetRecentMessages(WeChatRequestContext.Current.LoginInfo);

			result.Output();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getRecentMessages";
			}
		}
	}
}
