using System;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Web.Library;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class GroupDynamicMembers : Page, ITimeSceneDescriptor
	{
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		string ITimeSceneDescriptor.NormalSceneName
		{
			get { return this.CanEditMembers ? "Normal" : "ReadOnly"; }
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		protected bool CanEditMembers
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.GroupParentId, "EditMembersOfGroups"));
			}
		}

		private string GroupParentId
		{
			get
			{
				return (string)this.ViewState["GroupParentID"];
			}

			set
			{
				this.ViewState["GroupParentID"] = value;
			}
		}

		private PC.SCSimpleObject GroupObject
		{
			get
			{
				return (PC.SCSimpleObject)this.ViewState["SCGroup"];
			}

			set
			{
				this.ViewState["SCGroup"] = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.GroupParentId });
			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string groupId = Request.QueryString["id"];

			Util.InitSecurityContext(this.notice);

			this.calcProgress.Tag = groupId;

			if (Page.IsPostBack == false)
			{
				this.GroupObject = DbUtil.GetEffectiveObject(groupId, null).ToSimpleObject();
				this.GroupParentId = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.GroupObject.ID).Find(m => m.Status == SchemaObjectStatus.Normal && m.ParentSchemaType == "Organizations").ParentID;

				this.binding1.Data = this.GroupObject;

				this.gridMain.InitialData = new Services.ConditionSvc().GetGroupConditions(groupId);
				this.gridMain.DataBind();
			}
		}

		protected void ProcessCaculating(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			SCConditionCalculator calc = new SCConditionCalculator();

			calc.GenerateUserAndContainerSnapshot(new[] { (PC.SCGroup)SchemaObjectAdapter.Instance.Load(this.calcProgress.Tag) });

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void Nop(object sender, EventArgs e)
		{
		}

		protected void SaveClick(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();
			this.DoSave();
		}

		private void DoSave()
		{
			SCConditionCollection conditions = this.gridMain.InitialData as SCConditionCollection;
			if (conditions != null)
			{
				var owner = PC.Adapters.SCConditionAdapter.Instance.Load(this.GroupObject.ID, "Default") ?? new SCConditionOwner() { OwnerID = this.GroupObject.ID, Type = "Default" };

				var ownerConditions = owner.Conditions;

				owner.OwnerID = this.GroupObject.ID;

				owner.Conditions.ReplaceItemsWith(conditions, this.GroupObject.ID, "Default");

				try
				{
					Util.EnsureOperationSafe();
					DbUtil.GetEffectiveObject(this.GroupObject);

					PC.Executors.SCObjectOperations.InstanceWithPermissions.UpdateGroupConditions(owner);
					this.msg.Text = "条件已保存完成";
					this.gridMain.InitialData = new Services.ConditionSvc().GetGroupConditions(this.GroupObject.ID);
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