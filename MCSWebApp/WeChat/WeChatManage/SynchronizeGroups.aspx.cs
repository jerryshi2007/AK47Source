using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.Converters;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WeChatManage
{
    public partial class SynchronizeGroups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WeChatConverterHelper.RegisterConverters();

            if (!IsPostBack)
            {
                BindAccount();
            }
        }

        private void BindAccount()
        {
            ddlAccount.DataSource = AccountInfoAdapter.Instance.LoadAll();
            ddlAccount.DataValueField = "AccountID";
            ddlAccount.DataTextField = "UserID";
            ddlAccount.DataBind();
            ddlAccount.Items.Insert(0, new ListItem("全部", "-1"));
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            Synchronize();
        }

        private void Synchronize()
        {
            try
            {
                ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);
                List<WeChatGroup> allGroups = new List<WeChatGroup>();

                if (ddlAccount.SelectedIndex == 0)
                {
                    var allAccounts = AccountInfoAdapter.Instance.LoadAll();

                    foreach (var account in allAccounts)
                    {
                        ProcessProgress.Current.StatusText = string.Format("正在准备帐号\"{0}\"的数据...", account.UserID);
                        ProcessProgress.Current.Response();

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
                }
                else
                {
                    var account = AccountInfoAdapter.Instance.Load(p => p.AppendItem("AccountID", ddlAccount.SelectedValue)).FirstOrDefault();
                    WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                    Thread.Sleep(1000);

                    WeChatGroupCollection groups = WeChatHelper.GetAllGroups(WeChatRequestContext.Current.LoginInfo);

                    foreach (var group in groups)
                    {
                        group.AccountID = account.AccountID;
                        allGroups.Add(group);
                    }

                    ProcessProgress.Current.Response();
                }

                ProcessProgress.Current.MaxStep = allGroups.Count;
                ProcessProgress.Current.StatusText = "开始同步...";
                ProcessProgress.Current.Response();

                foreach (var group in allGroups)
                {
                    WeChatGroupAdapter.Instance.Update(group);
                    ProcessProgress.Current.Increment();
                    ProcessProgress.Current.Response();
                }

                ProcessProgress.Current.StatusText = "";
                ProcessProgress.Current.CurrentStep = 0;
                ProcessProgress.Current.Response();
            }
            catch (System.Exception ex)
            {
                WebUtility.ResponseShowClientErrorScriptBlock(ex);
            }
            finally
            {
                this.Response.Write(MCS.Web.WebControls.SubmitButton.GetResetAllParentButtonsScript(true));
                this.Response.End();
            }


        }
    }
}