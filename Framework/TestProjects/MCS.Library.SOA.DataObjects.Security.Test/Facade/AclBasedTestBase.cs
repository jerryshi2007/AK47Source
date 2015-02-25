using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	public abstract class AclBasedTestBase
	{
		protected static readonly string DefaultAppID = "8DDCCC74-0AE4-482B-ACEC-72A2731E9ADD";
		protected static readonly string DefaultRoleID = "FFA76F5D-AC90-4976-B94D-EA413F583EAB";
		public AclBasedTestBase()
		{
			this.InitPrincipal();
		}

		/// <summary>
		/// 获取或设置一个值，表示是否测试特殊状态（管理员角色无人时）
		/// </summary>
		public bool Special { get; set; }

		public static readonly IUser Wangli = OguObjectGenerator1.CastUser(OguObjectGenerator1.Wangli);

		protected virtual void InitPrincipal()
		{
			this.SetCurrentPrincipal(Wangli);
		}

		/// <summary>
		/// 设置负责人，并且清除缓存
		/// </summary>
		/// <param name="user"></param>
		protected void SetCurrentPrincipal(IUser user)
		{
			this.Principal = new DeluxePrincipal(new DeluxeIdentity(user));
		}

		protected void SetCurrentPrincipal(PC.SCUser user)
		{
			this.Principal = new DeluxePrincipal(new DeluxeIdentity(OguObjectGenerator1.CastUser(user)));
		}

		protected OGUPermission.IUser GetUserByCodeName(string codeName)
		{
			return OguObjectGenerator1.CastUser(this.GetSCUserByCodeName(codeName));
		}

		protected PC.SCUser GetSCUserByCodeName(string codeName)
		{
			return (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Users" }, new string[] { codeName }, true, false, DateTime.MinValue).First();
		}

		protected PC.SCOrganization GetOrganizationByCodeName(string codeName)
		{
			return (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Organizations" }, new string[] { codeName }, true, false, DateTime.MinValue).First();
		}

		protected PC.SCGroup GetGroupByCodeName(string codeName)
		{
			return (PC.SCGroup)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Groups" }, new string[] { codeName }, true, false, DateTime.MinValue).First();
		}

		protected IEnumerable<PC.SCUser> GetSCUsersByCodeNames(params string[] codeNames)
		{
			var result = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Users" }, codeNames, true, false, DateTime.MinValue);

			foreach (PC.SCUser user in result)
			{
				yield return user;
			}
		}

		protected PC.SCApplication CreateDefaultApp()
		{
			Facade.AddApplication(new PC.SCApplication()
			{
				ID = DefaultAppID,
				Name = "测试应用" + DefaultAppID,
				CodeName = DefaultAppID,
				DisplayName = "测试应用(默认测试)"
			});

			Sleep(200);

			return (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(DefaultAppID);
		}

		protected PC.SCRole CreateDefaultRole()
		{
			var app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(DefaultAppID);

			if (app == null || app.Status != SchemaObjectStatus.Normal)
			{
				app = this.CreateDefaultApp();
			}

			Debug.Assert(app.Status == SchemaObjectStatus.Normal);

			var role = new PC.SCRole()
			{
				ID = DefaultRoleID,
				Name = "测试角色" + DefaultRoleID,
				DisplayName = "测试角色(默认测试)",
				CodeName = DefaultRoleID
			};

			Facade.AddRole(role, app);

			return role;

		}

		protected PC.SCPermission CreatePermission(PC.SCApplication app, PC.SCPermission fun)
		{
			var fun0 = (PC.SCPermission)PC.Adapters.SchemaObjectAdapter.Instance.Load(fun.ID);
			if (fun0 == null || fun0.Status != SchemaObjectStatus.Normal)
			{
				Facade.AddPermission(fun, app);
			}
			else
			{
				fun = fun0;
			}

			return fun;
		}

		protected PC.SCRole CreateDefaultRoleWithMembers(IEnumerable<PC.SCUser> users)
		{
			var role = this.CreateDefaultRole();

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(role.ID);
			foreach (var item in members)
			{
				if (item.Status == SchemaObjectStatus.Normal)
				{
					Facade.RemoveMemberFromRole((PC.SCBase)item.Member, role);
				}
			}

			foreach (var user in users)
			{
				Facade.AddMemberToRole(user, role);
			}

			return role;
		}

		protected PC.SCRole CreateRoleWithMembers(PC.SCApplication app, PC.SCRole role, IEnumerable<PC.SCUser> users)
		{
			var role0 = (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(role.ID);
			if (role0 == null || role0.Status != SchemaObjectStatus.Normal)
			{
				Facade.AddRole(role, app);
			}
			else
			{
				role = role0;
			}

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(role.ID);
			foreach (var item in members)
			{
				Facade.RemoveMemberFromRole((PC.SCBase)item.Member, role);
			}

			if (users != null)
			{
				foreach (var user in users)
				{
					Facade.AddMemberToRole(user, role);
				}
			}


			return role;
		}


		protected void CreateRole(PC.SCApplication app, PC.SCRole role)
		{
			this.CreateRoleWithMembers(app, role, null);
		}


		protected virtual System.Security.Principal.IPrincipal Principal
		{
			get { return System.Threading.Thread.CurrentPrincipal; }

			set { System.Threading.Thread.CurrentPrincipal = value; }
		}

		internal static PC.Executors.ISCObjectOperations FacadeWithAcl
		{
			get
			{
				return PC.Executors.SCObjectOperations.InstanceWithPermissions;
			}
		}

		internal static PC.Executors.ISCObjectOperations Facade
		{
			get
			{
				return PC.Executors.SCObjectOperations.Instance;
			}
		}

		/// <summary>
		/// 根据配置文件，创建管理角色，并将wangli5注册为超级管理员
		/// </summary>
		protected virtual void InitAdmins()
		{
			this.InitAdminsAndUser(true);
			OguObjectGenerator1.GenerateOnly();
		}

		protected void InitAdminsAndUser(bool withWangli5)
		{
			PC.Adapters.SchemaObjectAdapter.Instance.ClearAllData();

			InnerInit(withWangli5);
		}

		private void InnerInit(bool withWangli5)
		{
			var adminRole = ObjectSchemaSettings.GetConfig().GetAdminRole();
			if (adminRole != null)
			{
				string[] parts = adminRole.FullCodeName.Split(':');
				string adminRoleID;
				if (parts.Length != 2)
					throw new FormatException("配置文件中的管理角色路径格式错误。");

				try
				{
					adminRoleID = adminRole.ID; // 有可能抛异常

					var roleMembers = PC.Adapters.UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(adminRoleID);

					var wangli = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(Wangli.ID);

					if (wangli == null)
					{
						var liucheng = this.CreateOU(new PC.SCOrganization()
						{
							ID = "f53e880d-b191-4788-8477-b0ddaa6d3a57",
							Name = "流程管理部",
							DisplayName = "流程管理部",
							CodeName = "f53e880d-b191-4788-8477-b0ddaa6d3a57"
						},
						this.CreateOU(new PC.SCOrganization()
						{
							ID = "04865298-aba1-4129-b041-f42f38f3547f",
							Name = "集团总部",
							DisplayName = "集团总部",
							CodeName = "04865298-aba1-4129-b041-f42f38f3547f"
						},
						this.CreateOU(new PC.SCOrganization()
						{
							ID = "efb29cac-5321-495b-844b-ed239a844ada",
							Name = "远洋地产",
							DisplayName = "远洋地产",
							CodeName = "efb29cac-5321-495b-844b-ed239a844ada"
						},
						this.CreateOU(new PC.SCOrganization()
						{
							ID = "85af29c7-9410-8d7e-4e49-924598e4e7d5",
							Name = "机构人员",
							DisplayName = "机构人员",
							CodeName = "85af29c7-9410-8d7e-4e49-924598e4e7d5"
						}, PC.SCOrganization.GetRoot()))));

						Facade.AddUser(OguObjectGenerator1.Wangli, liucheng);

						wangli = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(Wangli.ID);

						PC.Adapters.UserPasswordAdapter.Instance.SetPassword(wangli.ID, PC.Adapters.UserPasswordAdapter.GetPasswordType(), PC.Adapters.UserPasswordAdapter.GetDefaultPassword());
					}

					if (withWangli5)
					{
						if (roleMembers.Count == 0 || roleMembers.ContainsKey(wangli.ID) == false || roleMembers[Wangli.ID].Status != SchemaObjectStatus.Normal)
						{
							PC.SCRole r = (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(adminRoleID);
							if (r == null || r.Status != SchemaObjectStatus.Normal)
								throw new InvalidOperationException("管理角色已删除");

							Facade.AddMemberToRole(wangli, r);
						}
					}
					else
					{
						// 移除Wangli5
						if (roleMembers.ContainsKey(wangli.ID))
						{
							PC.SCRole r = (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(adminRoleID);
							Facade.RemoveMemberFromRole(wangli, r);
						}
					}
				}
				catch(SystemSupportException)
				{
					var apps = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Applications" }, new string[] { parts[0] }, true, false, DateTime.MinValue);
					if (apps.Count == 0)
					{
						Facade.AddApplication(new PC.SCApplication()
						{
							ID = UuidHelper.NewUuidString(),
							Name = "权限中心",
							CodeName = parts[0],
							DisplayName = "权限中心",
						});
						apps = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Applications" }, new string[] { parts[0] }, true, false, DateTime.MinValue);
					}

					if (apps.Count != 1)
						throw new System.IO.InvalidDataException(string.Format("不可以出现{0}个管理应用，只能有1个。", apps.Count));

					var mainApp = (PC.SCApplication)apps[0];

					var manageRoles = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(new string[] { "Roles" }, new string[] { parts[1] }, true, false, DateTime.MinValue);

					PC.SCRole mainRole = null;

					if (manageRoles.Count > 0)
					{
						var appMembersRelations = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(mainApp.ID);

						mainRole = (from a in appMembersRelations join PC.SCRole r in manageRoles on a.ID equals r.ID where a.SchemaType == "Roles" && a.Status == SchemaObjectStatus.Normal && r.Status == SchemaObjectStatus.Normal && ((PC.SCRole)r).CodeName == parts[1] select r).FirstOrDefault();
					}

					if (mainRole == null)
					{
						Facade.AddRole(new PC.SCRole()
						{
							ID = UuidHelper.NewUuidString(),
							Name = "权限中心总管理员",
							DisplayName = "权限中心总管理员",
							CodeName = parts[1]
						}, mainApp);
					}

					Thread.Sleep(200);
					this.InnerInit(withWangli5);
				}

				this.SetCurrentPrincipal(Wangli);
			}
			else
			{
				throw new InvalidOperationException("本单元测试要求配置文件中正确配置了管理角色");
			}
		}

		private PC.SCOrganization CreateOU(PC.SCOrganization ou, PC.SCOrganization parent)
		{
			var ouSelf = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.Load(ou.ID);
			if (ouSelf == null)
			{
				Facade.AddOrganization(ou, parent);
				ouSelf = ou;
			}

			return ouSelf;
		}

		protected static void ReGenOUData()
		{
            new MCS.Library.PermissionBridge.BridgedPermissionMechanism();

			OguObjectGenerator1.Generate();
		}

		protected static void Sleep(int milliSeconds)
		{
			Thread.Sleep(milliSeconds);
		}

		protected void SetContainerMemberAndPermissions(PC.SchemaObjectBase container, PC.SCRole member, string[] permissions)
		{
			PC.Permissions.SCAclContainer cc = new PC.Permissions.SCAclContainer(container);
			if (permissions != null)
			{
				foreach (string permission in permissions)
				{
					cc.Members.AddNotExistsItem(new PC.Permissions.SCAclItem(permission, member));
				}
			}

			var old = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(container.ID, DateTime.MinValue);
			if (old != null)
			{
				cc.Members.MergeChangedItems(old);
			}

			PC.Adapters.SCAclAdapter.Instance.Update(cc);
		}

		internal T NewObject<T>(string title) where T : PC.SCBase, new()
		{
			string uuid = UuidHelper.NewUuidString();

			return new T()
			{
				ID = uuid,
				Name = title + uuid,
				DisplayName = title + uuid,
				CodeName = uuid
			};
		}

		protected void RecalculateRoleUsers()
		{
			PC.Conditions.SCConditionCalculator calc = new PC.Conditions.SCConditionCalculator();
			calc.GenerateAllUserAndContainerSnapshot();
		}
	}
}
