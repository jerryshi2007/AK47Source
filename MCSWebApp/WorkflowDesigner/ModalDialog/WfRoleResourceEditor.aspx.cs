using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
namespace WorkflowDesigner.ModalDialog
{
	public partial class WfRoleResourceEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WfConverterHelper.RegisterConverters();

			if (IsPostBack == false)
			{
				try
				{
					ddlApps.BindData(GetAppCollection(), "CodeName", "Name");

					if (ddlApps.Items.Count > 0)
						ddlApps.Items[0].Selected = true;

					ddlRoles.BindData(GetRoles(ddlApps.SelectedValue), "CodeName", "Name");
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.relativeLink.Style["display"] = "inline";

			string template = "/MCSWebApp/PermissionCenter/lists/AppRoles.aspx?appCodeName={0}";

			UriConfigurationCollection urls = ResourceUriSettings.GetConfig().Paths;

			if (urls.ContainsKey("appRoles"))
				template = urls["appRoles"].Uri.ToString();

			this.relativeLink.HRef = string.Format(template, ddlApps.SelectedValue);

			base.OnPreRender(e);
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			try
			{
				IRole role = GetSelectedRole(ddlApps.SelectedValue, ddlRoles.SelectedValue);

				role.NullCheck("应用或角色的代码名称不能为空");

				WfRoleResourceDescriptor roleRes = new WfRoleResourceDescriptor(role);

				resultData.Value = JSONSerializerExecute.Serialize(roleRes);

				Page.ClientScript.RegisterStartupScript(this.GetType(), "returnRole",
					string.Format("window.returnValue = $get('resultData').value; top.close();"),
					true);
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		protected void ddlApps_SelectedIndexChanged(object sender, EventArgs e)
		{
			WfConverterHelper.RegisterConverters();

			ddlRoles.Enabled = false;
			string appCodeName = (sender as DropDownList).SelectedValue;

			RoleCollection roles = GetRoles(ddlApps.SelectedValue);

			ddlRoles.BindData(GetRoles(ddlApps.SelectedValue), "CodeName", "Name");

			ddlRoles.Enabled = true;
		}

		private static ApplicationCollection GetAppCollection()
		{
			return PermissionMechanismFactory.GetMechanism().GetAllApplications();
		}

		private static IApplication GetSelectedApplication(string appCodeName)
		{
			IApplication app = null;

			if (appCodeName.IsNotEmpty())
			{
				ApplicationCollection apps = GetAppCollection();

				app = apps[appCodeName];
			}

			return app;
		}

		private static IRole GetSelectedRole(string appCodeName, string roleCodeName)
		{
			IApplication app = GetSelectedApplication(appCodeName);

			IRole role = null;

			if (app != null)
			{
				role = app.Roles[roleCodeName];
			}

			return role;
		}

		private static RoleCollection GetRoles(string appCodeName)
		{
			ApplicationCollection apps = GetAppCollection();

			return apps[appCodeName].Roles;
		}
	}
}