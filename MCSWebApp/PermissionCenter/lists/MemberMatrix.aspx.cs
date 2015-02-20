using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.lists
{
	// [SceneUsage("~/App_Data/ListScene.xml")]
	public partial class MemberMatrix : System.Web.UI.Page
	{
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		protected PC.SCSimpleObject UserObject
		{
			get { return this.ViewState["UserObject"] as PC.SCSimpleObject; }
			set { this.ViewState["UserObject"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.UserObject = DbUtil.GetEffectiveObject<PC.SCUser>(this.Request.QueryString["id"]).ToSimpleObject();
				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.binding1.Data = this.UserObject;
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.dataSourceMain.LastQueryRowCount = -1;
			this.gridMain.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridMain.DataBind();
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
		}

		protected bool EditRoleMembersEnabled(string appID)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, appID, "ModifyMembersInRoles"));
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			System.Data.DataView view = e.ReturnValue as System.Data.DataView;
			if (view != null)
			{
				HashSet<string> parentIds = new HashSet<string>();
				HashSet<string> deleteLimitIds = new HashSet<string>();
				foreach (System.Data.DataRow row in view.Table.Rows)
				{
					string parentID = (string)row["AppID"];
					parentIds.Add(parentID);
				}

				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, parentIds);
			}
		}
	}
}