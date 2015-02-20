using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Web.Apps.WeChat;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.Converters;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library;
using MCS.Web.WebControls;
using System.Threading;

namespace WeChatManage.ModalDialogs
{
    public partial class GroupMembers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WeChatConverterHelper.RegisterConverters();
            
            if (!IsPostBack)
            {
                BindGroups();
            }
        }

        private void BindGroups()
        {
            ddlGroups.DataSource = ConditionalGroupAdapter.Instance.LoadAll();
            ddlGroups.DataValueField = "GroupID";
            ddlGroups.DataTextField = "Name";
            ddlGroups.DataBind();

            ddlGroups.Items.Insert(0, new ListItem("全部", "-1"));
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

                if (ddlGroups.SelectedIndex == 0)
                {
                    var allGroups = ConditionalGroupAdapter.Instance.LoadAll();
                    ProcessProgress.Current.MaxStep = allGroups.Count;
                    ProcessProgress.Current.StatusText = "开始计算...";
                    ProcessProgress.Current.Response();
                    foreach (var group in allGroups)
                    {
                        var result = Common.GetCalculatedGroupMembers(group.Condition);
                        using (TransactionScope scope = TransactionScopeFactory.Create())
                        {
                            GroupAndMemberAdapter.Instance.DeleteByGroupID(group.GroupID);
                            foreach (var member in result)
                            {
                                GroupAndMember gm = new GroupAndMember();
                                gm.GroupID = group.GroupID;
                                gm.MemberID = member.MemberID;
                                GroupAndMemberAdapter.Instance.Update(gm);
                            }
                            scope.Complete();
                        }

                        ProcessProgress.Current.StatusText = "";
                        ProcessProgress.Current.Increment();
                        ProcessProgress.Current.Response();
                    }
                }
                else
                {
                    var group = ConditionalGroupAdapter.Instance.Load(p => p.AppendItem("GroupID", ddlGroups.SelectedValue)).FirstOrDefault();

                    var result = Common.GetCalculatedGroupMembers(group.Condition);

                    ProcessProgress.Current.MaxStep = result.Count;
                    ProcessProgress.Current.Response();

                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        GroupAndMemberAdapter.Instance.DeleteByGroupID(group.GroupID);
                        foreach (var member in result)
                        {
                            GroupAndMember gm = new GroupAndMember();
                            gm.GroupID = group.GroupID;
                            gm.MemberID = member.MemberID;
                            GroupAndMemberAdapter.Instance.Update(gm);

                            ProcessProgress.Current.Increment();
                            ProcessProgress.Current.Response();
                        }

                        scope.Complete();
                    }
                }

                ProcessProgress.Current.StatusText = "";
                ProcessProgress.Current.CurrentStep = 0;
                ProcessProgress.Current.Response();
            }
            catch (Exception ex)
            {
                WebUtility.ResponseShowClientErrorScriptBlock(ex);
            }
            finally
            {
                this.Response.Write(SubmitButton.GetResetAllParentButtonsScript(true));
                this.Response.End();
            }
        }

        protected void ddlGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindList();
        }

        private void BindList()
        {
            if (ddlGroups.SelectedIndex > 0)
            {
                WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
                builder.AppendItem("g.GroupID", ddlGroups.SelectedValue);
                gridDataSource.Condition = builder.ToSqlString(TSqlBuilder.Instance);
            }
            else
            {
                gridDataSource.Condition = "";
            }

            gridMain.PageIndex = 0;
            gridMain.DataBind();
        }

        protected void btnSynchronizeToWeChat_Click(object sender, EventArgs e)
        {
            SynchronizeToWeChat();
        }

        private void SynchronizeToWeChat()
        {
            try
            {
                ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

                var allGroups = ConditionalGroupAdapter.Instance.LoadAll();
                var accounts = AccountInfoAdapter.Instance.LoadAll();

                WeChatGroupCollection weChatGroups = WeChatGroupAdapter.Instance.LoadAll();

                ProcessProgress.Current.MaxStep = allGroups.Count * accounts.Count;
                ProcessProgress.Current.CurrentStep = 0;
                ProcessProgress.Current.StatusText = "开始同步...";
                ProcessProgress.Current.Response();

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
                            var newGroup = new WeChatGroup()
                                {
                                    AccountID = account.AccountID,
                                    GroupID = modifiedGroup.GroupId,
                                    Name = group.Name,
                                    Count = modifiedGroup.MemberCnt
                                };

                            WeChatGroupAdapter.Instance.Update(newGroup);
                            Thread.Sleep(1000);
                            groupID = modifiedGroup.GroupId;
                        }
                        else
                        {
                            groupID = matchedGroup.GroupID;
                        }

                        if (fakeIDs.Length > 0)
                        {
                            WeChatHelper.ChangeFriendsGroup(groupID, fakeIDs, WeChatRequestContext.Current.LoginInfo);
                        }

                        ProcessProgress.Current.Increment();
                        ProcessProgress.Current.StatusText = string.Format("帐号\"{0}\",同步组\"{1}\"完成", account.UserID, group.Name);
                        ProcessProgress.Current.Response();
                        Thread.Sleep(1000);
                    }
                }

                ProcessProgress.Current.StatusText = string.Format("同步完成");
                ProcessProgress.Current.Response();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                WebUtility.ResponseShowClientErrorScriptBlock(ex);
            }
            finally
            {
                this.Response.Write(SubmitButton.GetResetAllParentButtonsScript(true));
                this.Response.End();
            }
        }
    }
}