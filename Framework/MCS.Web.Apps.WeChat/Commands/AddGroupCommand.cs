using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class AddGroupCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public AddGroupCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatModifyGroupRetInfo groupInfo = WeChatHelper.AddGroup(argument, WeChatRequestContext.Current.LoginInfo);

			Console.WriteLine(groupInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "addGroup {name}";
			}
		}
	}
}
