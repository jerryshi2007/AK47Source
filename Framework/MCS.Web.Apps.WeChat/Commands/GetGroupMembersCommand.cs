using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class GetGroupMembersCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetGroupMembersCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			int groupID = -1;

			if (int.TryParse(argument, out groupID) == false)
				groupID = -1;

			WeChatFriendCollection friends = WeChatHelper.GetGroupMembers(groupID, 0, 30, WeChatRequestContext.Current.LoginInfo);

			friends.Output();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getGroupMembers {groupID}";
			}
		}
	}
}
