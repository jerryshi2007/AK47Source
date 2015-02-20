using System;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Web.Library;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AppRoleConditionMembers : Page, ITimeSceneDescriptor
	{
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;
		private string appId = null;

		[Serializable]
		internal class RoleAndAppData
		{
			public string RoleID { get; set; }

			public string AppID { get; set; }

			public string AppName { get; set; }

			public string AppCodeName { get; set; }

			public string RoleName { get; set; }

			public string RoleCodeName { get; set; }

			public string RoleDisplayName { get; set; }

			public string VisibleName
			{
				get
				{
					if (string.IsNullOrEmpty(RoleDisplayName))
						return RoleName;
					else
						return RoleDisplayName;
				}
			}
		}

		protected bool EditRoleMembersEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.RoleAndAppObject.AppID, "ModifyMembersInRoles"));
			}
		}

		string ITimeSceneDescriptor.NormalSceneName
		{
			get { return (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.appId, "ModifyMembersInRoles")) ? "Normal" : "ReadOnly"; }
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		private RoleAndAppData RoleAndAppObject
		{
			get { return (RoleAndAppData)this.ViewState["RoleAndAppObject"]; }

			set { this.ViewState["RoleAndAppObject"] = value; }
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Util.SuperVisiorMode == false)
			{
				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.appId });
			}

			base.OnPreRender(e);

			roleMatrixEntryControl.AppID = this.RoleAndAppObject.AppID;
			roleMatrixEntryControl.AppName = this.RoleAndAppObject.AppName;
			roleMatrixEntryControl.AppCodeName = this.RoleAndAppObject.AppCodeName;
			roleMatrixEntryControl.RoleID = this.RoleAndAppObject.RoleID;
			roleMatrixEntryControl.RoleName = this.RoleAndAppObject.RoleName;
			roleMatrixEntryControl.RoleCodeName = this.RoleAndAppObject.RoleCodeName;
			roleMatrixEntryControl.RoleDescription = this.RoleAndAppObject.RoleDisplayName;
			roleMatrixEntryControl.Enabled = this.EditRoleMembersEnabled == false;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string roleId = Request.QueryString["role"];

			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				var role = DbUtil.GetEffectiveObject<PC.SCRole>(roleId);
				var app = role.CurrentApplication;

				this.RoleAndAppObject = new RoleAndAppData() { AppID = app.ID, AppCodeName = app.CodeName, AppName = app.Name, RoleID = role.ID, RoleCodeName = role.CodeName, RoleDisplayName = role.DisplayName, RoleName = role.Name };

				this.binding1.Data = this.RoleAndAppObject; // 绑定数据

				this.gridMain.InitialData = new Services.ConditionSvc().GetRoleConditions(roleId);
				this.gridMain.DataBind();
			}

			this.calcProgress.Tag = roleId;
		}

		protected void ProcessCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			SCConditionCalculator calc = new SCConditionCalculator();

			calc.GenerateUserAndContainerSnapshot(new[] { (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(this.calcProgress.Tag) });

			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void Nop(object sender, EventArgs e)
		{
		}

		protected void SaveClick(object sender, EventArgs e)
		{
			SCConditionCollection conditions = this.gridMain.InitialData as SCConditionCollection;

			if (conditions != null && this.RoleAndAppObject != null)
			{
				var owner = PC.Adapters.SCConditionAdapter.Instance.Load(this.RoleAndAppObject.RoleID, "Default") ?? new SCConditionOwner() { OwnerID = this.RoleAndAppObject.RoleID, Type = "Default" };

				owner.Conditions.ReplaceItemsWith(conditions, owner.OwnerID, "Default");

				try
				{
					Util.EnsureOperationSafe();
					DbUtil.GetEffectiveObject(this.RoleAndAppObject.RoleID, null);

					PC.Executors.SCObjectOperations.InstanceWithPermissions.UpdateRoleConditions(owner);
					this.msg.Text = "条件已保存完成";
					this.gridMain.InitialData = new Services.ConditionSvc().GetRoleConditions(this.RoleAndAppObject.RoleID);
					this.gridMain.DataBind();

					if (this.chkAutoCalc.Checked)
					{
						this.postScript.Text = Util.SurroundScriptBlock("Sys.Application.add_init(function(){ document.getElementById('btnRecalc').click();});");
					}
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex.GetRealException());
					this.notice.AddErrorInfo(ex);
					this.msg.Text = "操作遇到错误，可能没有成功保存条件";
				}
			}
		}
	}
}