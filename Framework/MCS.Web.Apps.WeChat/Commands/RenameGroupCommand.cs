using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class RenameGroupCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public RenameGroupCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			string[] parts = argument.Split(' ');

			if (parts.Length >= 2)
			{
				WeChatModifyGroupRetInfo groupInfo = WeChatHelper.RenameGroup(int.Parse(parts[0]), parts[1], WeChatRequestContext.Current.LoginInfo);

				Console.WriteLine(groupInfo);
			}
			else
				throw new System.ApplicationException(string.Format("非法的参数{0}", argument));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "renameGroup {groupID} {name}";
			}
		}
	}
}
