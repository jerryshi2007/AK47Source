using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Expression;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library;

namespace WeChatManage.ModalDialogs
{
    public partial class EditConditionGroup : System.Web.UI.Page
    {
        private string _GroupID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            _GroupID = WebUtility.GetRequestQueryValue("groupID", "");

            if (!IsPostBack)
            {
                if (_GroupID.IsNotEmpty())
                {
                    BindGroup();
                }
            }
        }

        private void BindGroup()
        {
            var group = ConditionalGroupAdapter.Instance.Load(p => p.AppendItem("GroupID", _GroupID)).FirstOrDefault();
            txtGroupName.Text = group.Name;
            txtDescript.Text = group.Description;
            txtCondition.Value = group.Condition;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var groupName = txtGroupName.Text;
            var description = txtDescript.Text;
            var condition = txtCondition.Value;

            if (groupName.Trim() == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errorAlert", "alert('名称不能为空!');", true);
            }
            else
            {
                if (condition.Trim() == "")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "errorAlert", "alert('条件不能为空!');", true);
                }
                else
                {
                    try
                    {
                        var result = Common.GetCalculatedGroupMembers(condition);

                        using (TransactionScope scope = TransactionScopeFactory.Create())
                        {
                            ConditionalGroup group = null;

                            if (_GroupID.IsNotEmpty())
                            {
                                group = ConditionalGroupAdapter.Instance.Load(p => p.AppendItem("GroupID", _GroupID)).FirstOrDefault();
                            }
                            else
                            {
                                group = new ConditionalGroup();
                                group.GroupID = UuidHelper.NewUuidString();
                            }

                            group.Name = groupName;
                            group.Description = description;
                            group.Condition = condition;
                            group.CalculateTime = DateTime.Now;

                            ConditionalGroupAdapter.Instance.Update(group);
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

                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('保存成功'); window.returnValue = true; window.close();", true);

                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "errorAlert", "alert('发生错误，原因：" + ex.Message.Replace("\'", "") + "');", true);
                    }
                }
            }
        }

     
    }
}