using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public static class OutputHelper
	{
		public static void Output(this WeChatGroupCollection groups)
		{
			foreach (WeChatGroup group in groups)
			{
				Console.WriteLine("Group ID: {0}, Name: {1}, Member Count: {2}",
					group.GroupID, group.Name, group.Count);
			}
		}

		public static void Output(this WeChatFriendCollection friends)
		{
			foreach (WeChatFriend friend in friends)
			{
				Console.WriteLine("Friend ID: {0}, Name: {1}, Group ID: {2}",
					friend.FakeID, friend.NickName, friend.GroupID);
			}
		}

		public static void Output(this WeChatRecentMessageCollection messages)
		{
			foreach (WeChatRecentMessage message in messages)
			{
				Console.WriteLine(message.ToString());
			}
		}

		public static void Output(this WeChatAppMessageCollection appMessages)
		{
			foreach (WeChatAppMessage appMessage in appMessages)
			{
				Console.WriteLine(appMessage.ToString());
			}
		}
	}
}
