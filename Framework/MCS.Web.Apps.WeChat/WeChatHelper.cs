using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.Apps.WeChat.Executors;

namespace MCS.Web.Apps.WeChat
{
	public class WeChatHelper
	{
		public static WeChatLoginInfo ExecLogin(string name, string password)//登陆微信公众平台函数
		{
			WeChatLoginExecutor executor = new WeChatLoginExecutor(name, password);

			executor.Execute();

			return executor.LoginInfo;
		}

		public static void FillLoginInExtraInfo(WeChatLoginInfo loginInfo)
		{
			loginInfo.NullCheck("loginInfo");

			WeChatGetLoginInExtraInfoExecutor executor = new WeChatGetLoginInExtraInfoExecutor(loginInfo);

			executor.Execute();
		}

		public static WeChatUploadFileRetInfo UploadFile(string filePath, WeChatLoginInfo loginInfo)
		{
			loginInfo.NullCheck("loginInfo");

			WeChatUploadFileExecutor executor = new WeChatUploadFileExecutor(filePath, loginInfo);

			executor.Execute();

			return executor.UploadedFileInfo;
		}

		public static WeChatAppMessage UpdateAppMessage(WeChatAppMessage message, WeChatLoginInfo loginInfo)
		{
			WeChatUpdateAppMessageExecutor executor = new WeChatUpdateAppMessageExecutor(message, loginInfo);

			executor.Execute();

			return executor.ResponseMessage;
		}

		public static WeChatAppMessageCollection GetAppMessages(int pageIndex, int pageSize, WeChatAppMessageType messageType, WeChatLoginInfo loginInfo)
		{
			WeChatGetAppMessagesExecutor executor = new WeChatGetAppMessagesExecutor(pageIndex, pageSize, messageType, loginInfo);

			executor.Execute();

			return executor.AppMessages;
		}

		public static WeChatAppMessageCollection GetAppMessages(WeChatAppMessageType messageType, WeChatLoginInfo loginInfo)
		{
			return GetAppMessages(0, 20, messageType, loginInfo);
		}

		/// <summary>
		/// 读取所有的组信息
		/// </summary>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatGroupCollection GetAllGroups(WeChatLoginInfo loginInfo)
		{
			WeChatGetAllGroupsExecutor executor = new WeChatGetAllGroupsExecutor(loginInfo);

			executor.Execute();

			return executor.AllGroups;
		}

		/// <summary>
		/// 得到微信账号在某个组内的粉丝
		/// </summary>
		/// <param name="groupID">所属组的ID，如果小于0，则表示所有的用户</param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatFriendCollection GetGroupMembers(int groupID, int pageIndex, int pageSize, WeChatLoginInfo loginInfo)
		{
			WeChatGetGroupMembersExecutor executor = new WeChatGetGroupMembersExecutor(groupID, pageIndex, pageSize, loginInfo);

			executor.Execute();

			return executor.Friends;
		}

		/// <summary>
		/// 得到微信账号在某个组内的粉丝（头20条记录）
		/// </summary>
		/// <param name="groupID"></param>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatFriendCollection GetGroupMembers(int groupID, WeChatLoginInfo loginInfo)
		{
			return GetGroupMembers(groupID, 0, 20, loginInfo);
		}

		/// <summary>
		/// 得到微信账号的所有粉丝。
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatFriendCollection GetAllMembers(int pageIndex, int pageSize, WeChatLoginInfo loginInfo)
		{
			return GetGroupMembers(-1, pageIndex, pageSize, loginInfo);
		}

		public static void MassSendTextMessage(int groupID, string message, WeChatLoginInfo loginInfo)
		{
			WeChatMassSendTextMessageExecutor executor = new WeChatMassSendTextMessageExecutor(groupID, message, loginInfo);

			executor.Execute();
		}

		public static void MassSendImageMessage(int groupID, string fileID, WeChatLoginInfo loginInfo)
		{
			WeChatMassSendImageMessageExecutor executor = new WeChatMassSendImageMessageExecutor(groupID, fileID, loginInfo);

			executor.Execute();
		}

		public static void MassSendAppMessage(int groupID, string appMsgID, WeChatLoginInfo loginInfo)
		{
			WeChatMassSendAppMessageExecutor executor = new WeChatMassSendAppMessageExecutor(groupID, appMsgID, loginInfo);

			executor.Execute();
		}

		/// <summary>
		/// 发送文本消息给某个人
		/// </summary>
		/// <param name="fakeID"></param>
		/// <param name="content"></param>
		/// <param name="loginInfo"></param>
		public static void SingleSendTextMessage(string fakeID, string content, WeChatLoginInfo loginInfo)
		{
			WeChatSingleSendTextMessageExecutor executor = new WeChatSingleSendTextMessageExecutor(fakeID, content, loginInfo);

			executor.Execute();
		}

		/// <summary>
		/// 发送图片消息给某个人
		/// </summary>
		/// <param name="fakeID"></param>
		/// <param name="fileID"></param>
		/// <param name="loginInfo"></param>
		public static void SingleSendImageMessage(string fakeID, string fileID, WeChatLoginInfo loginInfo)
		{
			WeChatSingleSendImageMessageExecutor executor = new WeChatSingleSendImageMessageExecutor(fakeID, fileID, loginInfo);

			executor.Execute();
		}

		/// <summary>
		/// 发送图文消息给某个人
		/// </summary>
		/// <param name="fakeID"></param>
		/// <param name="appMsgID"></param>
		/// <param name="loginInfo"></param>
		public static void SingleSendAppMessage(string fakeID, string appMsgID, WeChatLoginInfo loginInfo)
		{
			WeChatSingleSendAppMessageExecutor executor = new WeChatSingleSendAppMessageExecutor(fakeID, appMsgID, loginInfo);

			executor.Execute();
		}

		/// <summary>
		/// 获取该账号下最近的20条消息
		/// </summary>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatRecentMessageCollection GetRecentMessages(WeChatLoginInfo loginInfo)
		{
			return GetRecentMessages(0, 20, loginInfo);
		}

		/// <summary>
		/// 获取该账号下最近的消息
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatRecentMessageCollection GetRecentMessages(int pageIndex, int pageSize, WeChatLoginInfo loginInfo)
		{
			WeChatGetRecentMessagesExecutor executor = new WeChatGetRecentMessagesExecutor(pageIndex, pageSize, loginInfo);

			executor.Execute();

			return executor.Messages;
		}

		/// <summary>
		/// 获取某个粉丝的最近的消息
		/// </summary>
		/// <param name="fakeID"></param>
		/// <param name="loginInfo"></param>
		/// <returns></returns>
		public static WeChatRecentMessageCollection GetFriendMessages(string fakeID, WeChatLoginInfo loginInfo)
		{
			WeChatGetFriendMessagesExecutor executor = new WeChatGetFriendMessagesExecutor(fakeID, loginInfo);

			executor.Execute();

			return executor.Messages;
		}

		public static WeChatModifyGroupRetInfo AddGroup(string name, WeChatLoginInfo loginInfo)
		{
			WeChatAddGroupExecutor executor = new WeChatAddGroupExecutor(name, loginInfo);

			executor.Execute();

			return executor.GroupInfo;
		}

		public static WeChatModifyGroupRetInfo RenameGroup(int groupID, string name, WeChatLoginInfo loginInfo)
		{
			WeChatRenameGroupExecutor executor = new WeChatRenameGroupExecutor(groupID, name, loginInfo);

			executor.Execute();

			return executor.GroupInfo;
		}

		public static WeChatModifyGroupRetInfo DeleteGroup(int groupID, WeChatLoginInfo loginInfo)
		{
			WeChatDeleteGroupExecutor executor = new WeChatDeleteGroupExecutor(groupID, loginInfo);

			executor.Execute();

			return executor.GroupInfo;
		}

		public static void ChangeFriendsGroup(int groupID, string[] fakeIDs, WeChatLoginInfo loginInfo)
		{
			WeChatChangeFriendsGroupExecutor executor = new WeChatChangeFriendsGroupExecutor(groupID, fakeIDs, loginInfo);

			executor.Execute();
		}
	}
}
