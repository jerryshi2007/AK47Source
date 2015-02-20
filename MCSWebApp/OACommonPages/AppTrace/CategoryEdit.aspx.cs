using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.OA.CommonPages.AppTrace
{
    public partial class CategoryEdit : System.Web.UI.Page
    {
        protected void OkClick(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                try
                {
                    if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin") == false)
                        throw new OperationDeniedException("只有管理员可以进行此操作");

                    RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin").FalseThrow("只有管理员可以进行此操作");

                    if (string.IsNullOrEmpty(this.ddPg.SelectedValue) == false)
                    {
                        var old = WfApplicationAuthAdapter.Instance.Load(this.ddApp.SelectedValue, this.ddPg.SelectedValue, this.chkAdjuster.Checked ? WfApplicationAuthType.FormAdmin : WfApplicationAuthType.FormViewer);
                        if (old == null)
                            old = new WfApplicationAuth() { ApplicationName = this.ddApp.SelectedValue, AuthType = this.chkAdjuster.Checked ? WfApplicationAuthType.FormAdmin : WfApplicationAuthType.FormViewer, ProgramName = this.ddPg.SelectedItem.Value };

                        old.RoleID = this.inputRole.Text;
                        old.RoleDescription = this.inputRoleName.Value;

                        WfApplicationAuthAdapter.Instance.Update(old);
                        WebUtility.CloseWindow();
                    }
                    else
                    {
                        var appName = this.ddApp.SelectedValue;
                        var authType = this.chkAdjuster.Checked ? WfApplicationAuthType.FormAdmin : WfApplicationAuthType.FormViewer;
                        var old = WfApplicationAuthAdapter.Instance.Load(appName, authType);
                        var appPrograms = WfApplicationAdapter.Instance.LoadProgramsByApplication(appName);

                        var roleID = this.inputRole.Text;
                        var roleDescription = this.inputRoleName.Value;

                        List<WfApplicationAuth> auths = new List<WfApplicationAuth>(appPrograms.Count);
                        foreach (var item in appPrograms)
                        {
                            WfApplicationAuth auth = old.Find(m => m.ApplicationName == appName && m.AuthType == authType && m.ProgramName == item.CodeName);
                            if (auth == null)
                            {
                                auths.Add(new WfApplicationAuth() { ApplicationName = appName, AuthType = authType, ProgramName = item.CodeName, RoleID = roleID, RoleDescription = roleDescription });
                            }
                            else if (auth.RoleID != roleID)
                            {
                                auth.RoleID = roleID;
                                auth.RoleDescription = roleDescription;
                                auths.Add(auth);
                            }
                        }

                        using (System.Transactions.TransactionScope scope = MCS.Library.Data.TransactionScopeFactory.Create())
                        {
                            foreach (var item in auths)
                            {
                                WfApplicationAuthAdapter.Instance.Update(item);
                            }

                            scope.Complete();
                        }

                        WebUtility.CloseWindow();
                    }
                }
                catch (Exception ex)
                {
                    WebUtility.ShowClientError(ex);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (string.IsNullOrEmpty(this.inputRoleName.Value) == false)
            {
                this.roleNameLabel.InnerText = this.inputRoleName.Value;
            }
        }
    }
}