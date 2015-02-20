using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Security;
using MCS.Web.Apps.WeChat;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.Converters;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WeChatManage
{
    public partial class MassSendAppMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WeChatConverterHelper.RegisterConverters();

            if (!IsPostBack)
            {
                BindGroups();
                BindAccount();
            }
        }

        private void BindGroups()
        {
            ddlGroups.DataSource = ConditionalGroupAdapter.Instance.LoadAll();
            ddlGroups.DataValueField = "GroupID";
            ddlGroups.DataTextField = "Name";
            ddlGroups.DataBind();

            ddlGroups.Items.Insert(0, new ListItem("所有粉丝", "-1"));
        }

        private void BindAccount()
        {
            ddlAccount.DataSource = AccountInfoAdapter.Instance.LoadAll();
            ddlAccount.DataValueField = "AccountID";
            ddlAccount.DataTextField = "UserID";
            ddlAccount.DataBind();
            ddlAccount.Items.Insert(0, new ListItem("全部账号", "-1"));
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Send()
        {
            ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

            if (VerifyInput())
            {
                try
                {
                    if (ddlAccount.SelectedIndex == 0)
                    {
                        WeChatAppMessage appMessage = null;
                        ProcessProgress.Current.MaxStep = ddlAccount.Items.Count - 1;
                        ProcessProgress.Current.CurrentStep = 0;
                        ProcessProgress.Current.StatusText = "";
                        ProcessProgress.Current.Response();

                        foreach (ListItem accountItem in ddlAccount.Items)
                        {
                            try
                            {
                                if (accountItem.Value == "-1")
                                {
                                    continue;
                                }

                                appMessage = SendOne(accountItem.Value, ddlGroups.SelectedItem);
                                ProcessProgress.Current.StatusText = string.Format("对帐号 {0} 发送成功！", accountItem.Text);
                                ProcessProgress.Current.Response();
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex)
                            {
                                //WebUtility.ResponseShowClientErrorScriptBlock(ex);
                                ProcessProgress.Current.StatusText = string.Format("对帐号 {0} 发送失败，原因：{1}", accountItem.Text, ex.Message);
                                ProcessProgress.Current.Response();
                                Thread.Sleep(2000);
                            }
                            finally
                            {
                                ProcessProgress.Current.Increment();
                                ProcessProgress.Current.Response();
                            }
                        }

                        if (appMessage != null)
                        {
                            WeChatAppMessageAdapter.Instance.Update(appMessage);
                        }

                        ProcessProgress.Current.CurrentStep = 0;
                        ProcessProgress.Current.StatusText = "";
                        ProcessProgress.Current.Response();
                    }
                    else
                    {
                        ProcessProgress.Current.MaxStep = 1;
                        ProcessProgress.Current.CurrentStep = 0;
                        ProcessProgress.Current.StatusText = "";
                        ProcessProgress.Current.Response();

                        var appMessage = SendOne(ddlAccount.SelectedValue, ddlGroups.SelectedItem);
                        WeChatAppMessageAdapter.Instance.Update(appMessage);

                        ProcessProgress.Current.Increment();
                        ProcessProgress.Current.Response();
                    }

                    //ClientScript.RegisterStartupScript(this.GetType(), "sendComplete", "alert('发送完毕！');", true);
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
            else
            {
                this.Response.Write(SubmitButton.GetResetAllParentButtonsScript(true));
            }
        }

        private WeChatAppMessage SendOne(string accountID, ListItem item)
        {
            WeChatAppMessage result = null;

            if (item.Value == "-1")
            {
                var account = AccountInfoAdapter.Instance.Load(p => p.AppendItem("AccountID", accountID)).FirstOrDefault();

                var loginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                Thread.Sleep(1000);

                var uploadedImg = WeChatHelper.UploadFile(imgUploader.ImgProp.FilePath.Decrypt(), loginInfo); //上传图片
                Thread.Sleep(1000);

                WeChatAppMessage message = new WeChatAppMessage();
                message.Title = txtTitle.Text;
                message.Author = txtAuthor.Text;
                message.Digest = txtDigest.Value;
                message.FileID = uploadedImg.Content;
                message.ShowCover = chkShowInContent.Checked;
                message.Content = editor.InitialData;
                message.SourceUrl = txtSourceUrl.Text;

                result = WeChatHelper.UpdateAppMessage(message, loginInfo); //新增图文消息

                Thread.Sleep(1000);

                WeChatHelper.MassSendAppMessage(-1, result.AppMessageID.ToString(), loginInfo); //发送，一天一个帐号只能发送一次

                result.Title = message.Title;
                result.Author = message.Author;
                result.Digest = message.Digest;
                result.FileID = message.FileID;
                result.ShowCover = message.ShowCover;
                result.Content = message.Content;
                result.SourceUrl = message.SourceUrl;
            }
            else
            {
                var weChatGroup = WeChatGroupAdapter.Instance.Load(p =>
                    {
                        p.AppendItem("AccountID", accountID);
                        p.AppendItem("Name", item.Text);
                    }).FirstOrDefault();

                if (weChatGroup != null)
                {
                    var account = AccountInfoAdapter.Instance.Load(p => p.AppendItem("AccountID", accountID)).FirstOrDefault();

                    var loginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                    Thread.Sleep(1000);

                    var uploadedImg = WeChatHelper.UploadFile(imgUploader.ImgProp.FilePath.Decrypt(), loginInfo); //上传图片
                    Thread.Sleep(1000);

                    WeChatAppMessage message = new WeChatAppMessage();
                    message.Title = txtTitle.Text;
                    message.Author = txtAuthor.Text;
                    message.Digest = txtDigest.Value;
                    message.FileID = uploadedImg.Content;
                    message.ShowCover = chkShowInContent.Checked;
                    message.Content = editor.InitialData;
                    message.SourceUrl = txtSourceUrl.Text;

                    result = WeChatHelper.UpdateAppMessage(message, loginInfo); //新增图文消息

                    Thread.Sleep(1000);

                    WeChatHelper.MassSendAppMessage(weChatGroup.GroupID, result.AppMessageID.ToString(), loginInfo); //发送，一天一个帐号只能发送一次

                    result.Title = message.Title;
                    result.Author = message.Author;
                    result.Digest = message.Digest;
                    result.FileID = message.FileID;
                    result.ShowCover = message.ShowCover;
                    result.Content = message.Content;
                    result.SourceUrl = message.SourceUrl;
                }
            }

            return result;
        }

        private bool VerifyInput()
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                WebUtility.ResponseShowClientErrorScriptBlock("标题不能为空", "标题不能为空", "出错");
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('标题不能为空！');", true);
                return false;
            }

            if (string.IsNullOrEmpty(imgUploader.ImgProp.FilePath))
            {
                WebUtility.ResponseShowClientErrorScriptBlock("必须上传封面", "必须上传封面", "出错");
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('必须上传封面！');", true);
                return false;
            }

            if (string.IsNullOrEmpty(txtDigest.Value.Trim()))
            {
                WebUtility.ResponseShowClientErrorScriptBlock("摘要不能为空", "摘要不能为空", "出错");
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('摘要不能为空！');", true);
                return false;
            }

            if (string.IsNullOrEmpty(editor.InitialData.Trim()))
            {
                WebUtility.ResponseShowClientErrorScriptBlock("内容不能为空", "内容不能为空", "出错");
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('内容不能为空！');", true);
                return false;
            }

            return true;
        }
    }
}