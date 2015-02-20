using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

using System.Data;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Passport;

namespace WorkflowDesigner.ModalDialog
{
	public partial class ProcessKeyEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				processKey.Value = Request["prockey"];
                DropAppBind();
			}
		}

		protected void confirmButton_TextChanged(object sender, EventArgs e)
		{
			try
			{
				processKey.Value.IsNotEmpty().FalseThrow("流程模板的Key不能为空");
				WfProcessDescriptorManager.ExsitsProcessKey(processKey.Value).TrueThrow("Key为{0}的流程模板定义已经存在", processKey.Value);
                string dropAppName = this.dropApp.Text;
                if (dropAppName == "其它")
                {
                    dropAppName = string.Empty;
                }
                string returnValueJson = string.Format("{{\"Key\":\"{0}\",\"AppName\":\"{1}\"}}", processKey.Value, dropAppName);
				Response.Write(string.Format("<script type='text/javascript'>top.returnValue = '{0}'; top.close();</script>",
                    WebUtility.CheckScriptString(returnValueJson)));

				Response.End();
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

        protected void DropAppBind() {

            this.dropApp.DataSource = GetApplicationsByCurrentUser();
            this.dropApp.DataValueField = "Name";
            this.dropApp.DataTextField = "Name";
			this.dropApp.SelectedValue = "其它";
            this.DataBind();
        }

        private List<AppListItem> GetApplicationsByCurrentUser()
        {
            List<AppListItem> applications = new List<AppListItem>();    

            if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
            {
                var defaultApp = WfProcessDescriptionCategoryAdapter.Instance.Load(p => p.AppendItem("ID", "", "<>"));
                foreach (var item in defaultApp)
                {
                    AppListItem obj = new AppListItem(item.Name);
                    if (!applications.Exists(a => a.Name == obj.Name))
                        applications.Add(obj);
                }
            }

            IRole[] roles = RolesDefineConfig.GetConfig().GetRolesInstances("DesignerRoleMatrix");
            foreach (var role in roles)
            {
                AppendApplicationName(role, applications);
            }

            AppListItem other = new AppListItem("其它");
            applications.Add(other);
            return applications;
        }

        private static void AppendApplicationName(IRole role, List<AppListItem> appList)
        {
            SOARolePropertyRowCollection allRows = SOARolePropertiesAdapter.Instance.GetByRole(role);
            SOARolePropertyRowCollection rows = allRows;

            if (!RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
                rows = allRows.Query(r => r.Operator == DeluxeIdentity.Current.User.LogOnName);

            foreach (var row in rows)
            {
                string appName = row.Values.GetValue("ApplicationName", string.Empty);
                AppListItem obj = new AppListItem(appName);
                if (!appList.Exists(a => a.Name == obj.Name))
                    appList.Add(obj);
            }
        }
	}
}