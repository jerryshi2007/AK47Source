using System;
using System.Linq;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.Dialogs
{
    public partial class EditorPassword : System.Web.UI.Page
    {
        public bool SupervisiorMode
        {
            get
            {
                return Util.SuperVisiorMode;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // WebUtility.RegisterClientMessageScript();
            if (Page.IsPostBack == false)
            {
                this.txtLogOnName.Text = Server.HtmlEncode(MCS.Library.Principal.DeluxePrincipal.Current.Identity.Name);
                this.tb_OldPassword.Focus();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.isSuperVisior.Value = this.SupervisiorMode ? "1" : "0";
            this.currentUserName.Value = Util.CurrentUser.LogOnName;
            this.lblCurrentPass.InnerText = (this.txtLogOnName.Text != this.currentUserName.Value && this.SupervisiorMode) ? "管理员口令" : "当前口令";
        }

        protected void OK_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                TimePointContext context = TimePointContext.GetCurrentState();

                try
                {
                    TimePointContext.Current.SimulatedTime = DateTime.MinValue;
                    TimePointContext.Current.UseCurrentTime = true;

                    var user = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(StandardObjectSchemaType.Users.ToString(), this.txtLogOnName.Text, SchemaObjectStatus.Normal, DateTime.MinValue);

                    if (user != null)
                    {
                        if (user.CurrentParentRelations.Exists(p => p.Status == SchemaObjectStatus.Normal && p.ParentSchemaType == "Organizations" && p.Parent.Status == SchemaObjectStatus.Normal) == false)
                            throw new InvalidOperationException(string.Format("没有找到对应登录名 {0} 的用户的组织，必须先将用户加入组织。", this.txtLogOnName.Text));

                        var userId = user.ID;
                        bool valid = false;
                        if (this.SupervisiorMode)
                        {
                            if (this.txtLogOnName.Text == Util.CurrentUser.LogOnName)
                            {
                                // 管理员修改自己的
                                if (UserPasswordAdapter.Instance.CheckPassword(userId, UserPasswordAdapter.GetPasswordType(), this.tb_OldPassword.Value) == true)
                                {
                                    valid = true;
                                }
                                else
                                {
                                    this.passwordresult.InnerText = "原始密码不正确";
                                }
                            }
                            else
                            {
                                // 修改别人的，检查管理员的密码是否正确
                                if (UserPasswordAdapter.Instance.CheckPassword(Util.CurrentUser.ID, UserPasswordAdapter.GetPasswordType(), this.tb_OldPassword.Value) == true)
                                {
                                    valid = true;
                                }
                                else
                                {
                                    this.passwordresult.InnerText = "管理员密码错误";
                                }
                            }
                        }
                        else
                        {
                            if (UserPasswordAdapter.Instance.CheckPassword(userId, UserPasswordAdapter.GetPasswordType(), this.tb_OldPassword.Value) == true)
                            {
                                valid = true;
                            }
                            else
                            {
                                this.passwordresult.InnerText = "原始密码不正确";
                            }
                        }

                        if (valid)
                        {
                            UserPasswordAdapter.Instance.SetPassword(userId, UserPasswordAdapter.GetPasswordType(), this.tb_NewPassword.Value);

                            ScriptManager.RegisterClientScriptBlock(this.passwordUpdatePanel, this.GetType(), "master", "top.window.close();", true);
                        }
                    }
                    else
                    {
                        throw new ObjectNotFoundException(string.Format("未能找到对应登录名 {0} 的用户。", this.txtLogOnName.Text));
                    }
                }
                catch (Exception ex)
                {
                    this.passwordresult.InnerText = string.Format("无法验证用户,{0}", ex.Message);

                    // WebUtility.RegisterClientErrorMessage(ex);
                    // ScriptManager.RegisterStartupScript(this.passwordUpdatePanel, this.GetType(), "提示", "window.alert('原密码不正确')", true);
                }
                finally
                {
                    TimePointContext.RestoreCurrentState(context);
                }
            }
        }
    }
}