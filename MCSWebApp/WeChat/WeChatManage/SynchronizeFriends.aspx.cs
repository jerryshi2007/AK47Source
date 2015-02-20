using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat;
using MCS.Web.Apps.WeChat.Converters;
using MCS.Web.WebControls;
using MCS.Web.Library;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Apps.WeChat.Adapters;

namespace WeChatManage
{
	public partial class SynchronizeFriends : System.Web.UI.Page
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
				List<WeChatFriend> allFriends = new List<WeChatFriend>();

				if (ddlAccount.SelectedIndex == 0)
				{
					var allAccounts = AccountInfoAdapter.Instance.LoadAll();

					foreach (var account in allAccounts)
					{
						WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                        ProcessProgress.Current.StatusText = string.Format("正在准备帐号\"{0}\"的数据...", account.UserID);
                        ProcessProgress.Current.Response();
					    Thread.Sleep(1000);

						var flag = true;
						var curIndex = 0;
						while (flag)
						{
							WeChatFriendCollection friends = WeChatHelper.GetAllMembers(curIndex, 100, WeChatRequestContext.Current.LoginInfo);
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

							ProcessProgress.Current.Response();
						}
					}
				}
				else
				{
					var account = AccountInfoAdapter.Instance.Load(p => p.AppendItem("AccountID", ddlAccount.SelectedValue)).FirstOrDefault();
					WeChatRequestContext.Current.LoginInfo = WeChatHelper.ExecLogin(account.UserID, account.Password);
                    ProcessProgress.Current.StatusText = string.Format("正在准备帐号\"{0}\"的数据...", account.UserID);
                    ProcessProgress.Current.Response();
                    Thread.Sleep(1000);

					var flag = true;
					var curIndex = 0;
					while (flag)
					{
						WeChatFriendCollection friends = WeChatHelper.GetAllMembers(curIndex, 100, WeChatRequestContext.Current.LoginInfo);
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

						ProcessProgress.Current.Response();
					}
				}

				ProcessProgress.Current.MaxStep = allFriends.Count;
                ProcessProgress.Current.StatusText = "开始同步...";
				ProcessProgress.Current.Response();

				foreach (var friend in allFriends)
				{
					WeChatFriendAdapter.Instance.Update(friend);
					ProcessProgress.Current.Increment();
					ProcessProgress.Current.Response();
				}

				ProcessProgress.Current.CurrentStep = 0;
                ProcessProgress.Current.StatusText = "";
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
	}
}