using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class SingleSendImageMessageCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public SingleSendImageMessageCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			string[] parts = argument.Split(' ');

			if (parts.Length >= 2)
				WeChatHelper.SingleSendImageMessage(parts[0], parts[1], WeChatRequestContext.Current.LoginInfo);
			else
				throw new System.ApplicationException(string.Format("非法的参数{0}", argument));

			Console.WriteLine("Sent OK!");
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "singleSendImage {fakeID} {fileID}";
			}
		}
	}
}
