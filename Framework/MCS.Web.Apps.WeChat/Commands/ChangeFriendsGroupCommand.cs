using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class ChangeFriendsGroupCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public ChangeFriendsGroupCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			string[] parts = argument.Split(' ');

			if (parts.Length >= 2)
			{
				string[] fakeIDs = parts[1].Split(',', ';');

				WeChatHelper.ChangeFriendsGroup(int.Parse(parts[0]), fakeIDs, WeChatRequestContext.Current.LoginInfo);
			}
			else
				throw new System.ApplicationException(string.Format("非法的参数{0}", argument));
			
			Console.WriteLine("Changed !");
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "changeGroup {groupID} {fakeID1,fakeID2...fakeIDn}";
			}
		}
	}
}
