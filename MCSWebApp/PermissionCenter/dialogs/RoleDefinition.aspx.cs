using System;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class RoleDefinition : Page, INormalSceneDescriptor
	{
		private SCContainerAndPermissionCollection containerPermissions = null;
		private string appId = null;

		private SCSimpleObject RoleObject
		{
			get { return (SCSimpleObject)this.ViewState["RoleObject"]; }
			set { this.ViewState["RoleObject"] = value; }
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				this.btnSave.Enabled &= Util.ContainsPermission(this.containerPermissions, this.appId, "EditRelationOfRolesAndPermissions");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
			{
				this.appId = SCMemberRelationAdapter.Instance.LoadByMemberID(this.RoleObject.ID).Find(m => m.ContainerSchemaType == "Applications" && m.Status == SchemaObjectStatus.Normal).ContainerID;

				this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.appId });
			}

			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string id = Request.QueryString["role"];

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.RoleObject = DbUtil.GetEffectiveObject<SCRole>(id).ToSimpleObject();

				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.binding1.Data = this.RoleObject;
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridMain.DataBind();
		}

		protected void SearchButtonClick(object sender, EventArgs e)
		{
			this.gridMain.PageIndex = 0;
			this.InnerRefreshList();
		}

		protected void SaveClick(object sender, EventArgs e)
		{
			try
			{
				Util.EnsureOperationSafe();
				var role = (SCRole)DbUtil.GetEffectiveObject(this.RoleObject, "角色不存在或已被删除");

				string strToAdd = this.hfAdded.Value;
				string strToRemove = this.hfRemoved.Value;

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;
				if (strToAdd.Length > 0)
				{
					foreach (string key in strToAdd.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries))
					{
						try
						{
							var permission = (SCPermission)adapter.Load(key);
							if (permission == null || permission.Status != SchemaObjectStatus.Normal)
								throw new InvalidOperationException("指定的权限无效");
							actor.JoinRoleAndPermission(role, permission);
						}
						catch (Exception ex)
						{
							this.notice.AddErrorInfo(ex);
						}
					}
				}

				if (strToRemove.Length > 0)
				{
					foreach (string key in strToRemove.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						try
						{
							var permission = (SCPermission)adapter.Load(key);
							if (permission == null || permission.Status != SchemaObjectStatus.Normal)
								throw new InvalidOperationException("指定的权限无效");
							actor.DisjoinRoleAndPermission(role, permission);
						}
						catch (Exception ex)
						{
							this.notice.AddErrorInfo(ex);
						}
					}
				}

				this.hfAdded.Value = string.Empty;
				this.hfRemoved.Value = string.Empty;
				if (this.notice.HasErrors == false)
				{
					this.notice.Text = "权限设置成功。";
					this.notice.RenderType = WebControls.NoticeType.Info;
					this.preScript.Text = Util.SurroundScriptBlock("window.close();");
				}
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
			}
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.dataSourceMain.LastQueryRowCount = -1;
			this.gridMain.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
			this.hfAdded.Value = this.hfRemoved.Value = string.Empty;
		}

		protected void dataSourceMain_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
		{
			this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
		}
	}
}