using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;

namespace WeChatManage.Services
{
    /// <summary>
    /// Summary description for SynchronizeService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SynchronizeService : System.Web.Services.WebService
    {

        [WebMethod]
        public void SynchronizeFromWeChat()
        {
            SynchronizeGroups();
            SynchronizeFriends();
            SynchronizeRecentMessages();
        }

        /// <summary>
        /// 同步组和组内成员数据
        /// </summary>
        [WebMethod]
        public void SynchronizeToWeChat()
        {
            //首先要关联OpenID和FakeID，通过存储过程CalculateOpenIDFromMessages实现。
            WeChatFriendAdapter.Instance.CalculateOpenIDFromMessages();

            SynchronizeGroupsAndMembersToWeChat();
        }

        private void SynchronizeGroupsAndMembersToWeChat()
        {
            var allGroups = ConditionalGroupAdapter.Instance.LoadAll();
            var accounts = AccountInfoAdapter.Instance.LoadAll();

            WeChatGroupCollection weChatGroups = WeChatGroupAdapter.Instance.LoadAll();
            foreach (var account in accounts)
            {
                WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                Thread.Sleep(1000);

                foreach (var group in allGroups)
                {
                    var fakeIDs = WeChatFriendAdapter.Instance.GetFakeIDsByLocalGroupID(group.GroupID).ToArray(); //找到本组里成员的fakeID
                    
                    var matchedGroup = weChatGroups.Find(p => p.Name == group.Name && p.AccountID == WeChatRequestContext.Current.LoginInfo.AccountID);

                    int groupID = 0;
                    if (matchedGroup == null)
                    {
                        var modifiedGroup = WeChatHelper.AddGroup(group.Name, WeChatRequestContext.Current.LoginInfo);
                        Thread.Sleep(1000);

                        var newGroup = new WeChatGroup()
                        {
                            AccountID = account.AccountID,
                            GroupID = modifiedGroup.GroupId,
                            Name = group.Name,
                            Count = modifiedGroup.MemberCnt
                        };

                        WeChatGroupAdapter.Instance.Update(newGroup);

                        groupID = modifiedGroup.GroupId;
                    }
                    else
                    {
                        groupID = matchedGroup.GroupID;
                    }

                    if (fakeIDs.Length > 0)
                    {
                        WeChatHelper.ChangeFriendsGroup(groupID, fakeIDs, WeChatRequestContext.Current.LoginInfo);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void SynchronizeGroups()
        {
            List<WeChatGroup> allGroups = new List<WeChatGroup>();

            var allAccounts = AccountInfoAdapter.Instance.LoadAll();

            foreach (var account in allAccounts)
            {
                WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                Thread.Sleep(1000);

                WeChatGroupCollection groups = WeChatHelper.GetAllGroups(WeChatRequestContext.Current.LoginInfo);
                Thread.Sleep(1000);

                foreach (var group in groups)
                {
                    group.AccountID = account.AccountID;
                    allGroups.Add(group);
                }
            }

            foreach (var group in allGroups)
            {
                WeChatGroupAdapter.Instance.Update(group);
            }
        }

        private void SynchronizeFriends()
        {
            List<WeChatFriend> allFriends = new List<WeChatFriend>();

            var allAccounts = AccountInfoAdapter.Instance.LoadAll();

            foreach (var account in allAccounts)
            {
                WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                Thread.Sleep(1000);

                var flag = true;
                var curIndex = 0;
                while (flag)
                {
                    WeChatFriendCollection friends = WeChatHelper.GetAllMembers(curIndex, 200, WeChatRequestContext.Current.LoginInfo);
                    Thread.Sleep(1000);

                    foreach (var friend in friends)
                    {
                        friend.AccountID = account.AccountID;
                        allFriends.Add(friend);
                    }

                    if (friends.Count == 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        curIndex++;
                    }
                }
            }

            foreach (var friend in allFriends)
            {
                WeChatFriendAdapter.Instance.Update(friend);
            }
        }

        private void SynchronizeRecentMessages()
        {
            List<WeChatRecentMessage> allMessages = new List<WeChatRecentMessage>();

            var allAccounts = AccountInfoAdapter.Instance.LoadAll();

            foreach (var account in allAccounts)
            {
                WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                Thread.Sleep(1000);

                var flag = true;
                var curIndex = 0;
                while (flag)
                {
                    WeChatRecentMessageCollection messages = WeChatHelper.GetRecentMessages(curIndex, 200, WeChatRequestContext.Current.LoginInfo);
                    Thread.Sleep(1000);

                    foreach (var message in messages)
                    {
                        message.AccountID = account.AccountID;
                        allMessages.Add(message);
                    }

                    if (messages.Count == 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        curIndex++;
                    }
                }
            }

            foreach (var message in allMessages)
            {
                WeChatRecentMessageAdapter.Instance.Update(message);
            }
        }
    }
}
