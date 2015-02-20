using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Web.Apps.WeChat.Commands
{
	public static class WeChatCommandHelper
	{
		/// <summary>
		/// 注册所有的命令
		/// </summary>
		public static void RegisterCommands()
		{
			CommandHelper.RegisterCommand(new HelpCommand("?"));
			CommandHelper.RegisterCommand(new GetAllGroupsCommand("getAllGroups"));
			CommandHelper.RegisterCommand(new GetGroupMembersCommand("getGroupMembers"));
			CommandHelper.RegisterCommand(new GetFriendMessagesCommand("getFriendMessages"));
			CommandHelper.RegisterCommand(new GetRecentMessagesCommand("getRecentMessages"));
			CommandHelper.RegisterCommand(new UploadFileCommand("uploadFile"));
			CommandHelper.RegisterCommand(new UpdateAppMessageCommand("updateMessage"));
			CommandHelper.RegisterCommand(new GetAppMessagesCommand("getAppMessages"));
			CommandHelper.RegisterCommand(new SingleSendTextMessageCommand("singleSendText"));
			CommandHelper.RegisterCommand(new SingleSendImageMessageCommand("singleSendImage"));
			CommandHelper.RegisterCommand(new SingleSendAppMessageCommand("singleSendAppMessage"));
			CommandHelper.RegisterCommand(new MassSendTextMessageCommand("massSendText"));
			CommandHelper.RegisterCommand(new MassSendAppMessageCommand("massSendAppMessage"));
			CommandHelper.RegisterCommand(new MassSendImageMessageCommand("massSendImage"));
			CommandHelper.RegisterCommand(new AddGroupCommand("addGroup"));
			CommandHelper.RegisterCommand(new RenameGroupCommand("renameGroup"));
			CommandHelper.RegisterCommand(new DeleteGroupCommand("deleteGroup"));
			CommandHelper.RegisterCommand(new ChangeFriendsGroupCommand("changeGroup"));
		}
	}
}
