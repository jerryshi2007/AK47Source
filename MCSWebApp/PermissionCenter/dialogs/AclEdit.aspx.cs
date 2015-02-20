using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Dialogs
{
	[SceneUsage("~/App_Data/AclEditor.xml")]
	public partial class AclEdit : System.Web.UI.Page, ITimeSceneDescriptor
	{
		private SCContainerAndPermissionCollection containerPermissions = null;
		private SCParentsRelationObjectCollection allParents;

		protected SCSimpleObject Object
		{
			get
			{
				return this.ViewState["SCObject"] as SCSimpleObject;
			}

			set
			{
				this.ViewState["SCObject"] = value;
			}
		}

		protected bool AllowAclInheritance
		{
			get
			{
				object o = this.ViewState["AllowAclInheritance"];
				if (o is bool)
				{
					return (bool)o;
				}

				return false;
			}

			set
			{
                this.ViewState["AllowAclInheritance"] = value;
			}
		}

		string ITimeSceneDescriptor.NormalSceneName
		{
			get
			{
				bool canEditPermission = this.allParents != null && this.allParents.Any(m => m.Status == SchemaObjectStatus.Normal && Util.ContainsPermission(this.containerPermissions, m.ParentID, "EditPermissionsOfChildren"));

				return (Util.SuperVisiorMode || canEditPermission) ? (this.AllowAclInheritance ? "Normal" : "NormalNoChild") : (this.AllowAclInheritance ? "ReadOnly" : "ReadOnlyNoChild");
			}
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return this.AllowAclInheritance ? "ReadOnly" : "ReadOnlyNoChild"; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				var obj = DbUtil.GetEffectiveObject(Request.QueryString["id"], null);
				if (obj is IAllowAclInheritance)
				{
					this.AllowAclInheritance = true;
					this.chkInherit.Visible = true;

					this.chkInherit.Checked = obj.Properties.GetValue("AllowAclInheritance", false);
				}
				else
				{
					this.AllowAclInheritance = false;
					this.chkInherit.Visible = false;
				}

				this.Object = obj.ToSimpleObject();
			}

			Page.Response.CacheControl = "no-cache";
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.spObjName.InnerText = string.Format("{0}({1})", this.Object.Name, this.Object.VisibleName);

			this.RenderInitJson();

			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
			{
				this.allParents = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.Object.ID);
				var parentSet = new HashSet<string>();
				foreach (var item in this.allParents)
				{
					if (item.Status == SchemaObjectStatus.Normal)
					{
						parentSet.Add(item.ParentID);
					}
				}

				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, parentSet);
			}

			base.OnPreRender(e);
		}

		protected void HandleSaveClick(object sender, EventArgs e)
		{
			SCAclMemberCollection aclMembers = JSONSerializerExecute.Deserialize<SCAclMemberCollection>(this.postData.Value);
			try
			{
				var obj = DbUtil.GetEffectiveObject(this.Object);
				InnerDoSave(aclMembers, obj, this.chkInherit.Checked);
				this.extScript.Text = Util.SurroundScriptBlock("window.close();");
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				WebUtility.ShowClientError(ex);
			}
		}

		protected void DoOverridePostData(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			// 先保存当前数据
			try
			{
				ProcessProgress pg = ProcessProgress.Current;
				pg.CurrentStep = pg.MaxStep = 1;
				pg.MinStep = 0;
				var aclMembers = JSONSerializerExecute.Deserialize<SCAclMemberCollection>(e.Steps[0]);
				bool all = e.Steps[1].Equals("all");
				bool inheritRight = e.Steps[2].Equals("inherit");
				var obj = DbUtil.GetEffectiveObject(e.ClientExtraPostedData, null);

				InnerDoSave(aclMembers, obj, inheritRight);

				PC.Executors.SCObjectOperations.InstanceWithPermissions.ReplaceAclRecursively((ISCAclContainer)obj, all);

				pg.Output.WriteLine("完毕");
			}
			catch (Exception ex)
			{
				ProcessProgress.Current.Output.WriteLine(ex.ToString());
				ProcessProgress.Current.StatusText = "错误";
				ProcessProgress.Current.Response();
			}

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		private static SCAclPermissionItemCollection GetPermissionDefinitions(string schemaType)
		{
			var element = ObjectSchemaSettings.GetConfig().Schemas[schemaType];
			var permissionDefine = new SCAclPermissionItemCollection(element.PermissionSet);
			return permissionDefine;
		}

		private static void InnerDoSave(SCAclMemberCollection aclMembers, SchemaObjectBase obj, bool inheritRights)
		{
			HashSet<string> roleIds = new HashSet<string>();
			aclMembers.ForEach(m => roleIds.Add(m.MemberID));

			var roles = DbUtil.LoadObjects(roleIds.ToArray());

			var pmDefs = GetPermissionDefinitions(obj.SchemaType);

			var container = new SCAclContainer(obj);

			foreach (var acl in aclMembers)
			{
				Debug.Assert(acl.ContainerID == obj.ID, "ACL的容器ID必须与对象的ID一致");
				container.Members.Add(acl.ContainerPermission, roles.Find(m => m.ID == acl.MemberID));
			}

			if (obj is IAllowAclInheritance)
			{
				if (obj.Properties.GetValue("AllowAclInheritance", false) != inheritRights)
				{
					obj.Properties.SetValue("AllowAclInheritance", inheritRights);

					// TODO:换更合适的方式
					PC.Executors.SCObjectOperations.Instance.DoOperation(SCObjectOperationMode.Update, obj, null);
				}
			}

			SCAclMemberCollection originalMembers = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(obj.ID, DateTime.MinValue);

			if (container.Members.MergeChangedItems(originalMembers))
				PC.Executors.SCObjectOperations.InstanceWithPermissions.UpdateObjectAcl(container);
		}

		private void RenderInitJson()
		{
			// 查找当前对象的权限定义
			var permissionDefine = GetPermissionDefinitions(this.Object.SchemaType);

			// 查找当前对象的权限
			var aclMembers = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(this.Object.ID, DateTime.MinValue);
			for (int i = 0; i < aclMembers.Count; )
			{
				if (aclMembers[i].Status == SchemaObjectStatus.Normal)
					i++;
				else
					aclMembers.RemoveAt(i); // 剔除删除的
			}

			Debug.Assert((from acl in aclMembers where acl.Status == SchemaObjectStatus.Normal select acl).Count() == aclMembers.Count, "应该一致");

			// 查出角色
			var roles = RoleDisplayItemAdapter.Instance.LoadByRoleIds((from r in aclMembers select r.MemberID).Distinct().ToArray());

			this.postData.Value = JSONSerializerExecute.Serialize(new ContextData() { ContainerID = this.Object.ID, AclMembers = aclMembers, Permissions = permissionDefine, Roles = new RoleDisplayItemCollection(roles) });
		}

		[Serializable]
		internal class ContextData
		{
			public string ContainerID { get; set; }

			public SCAclMemberCollection AclMembers { get; set; }

			public SCAclPermissionItemCollection Permissions { get; set; }

			public RoleDisplayItemCollection Roles { get; set; }
		}
	}
}