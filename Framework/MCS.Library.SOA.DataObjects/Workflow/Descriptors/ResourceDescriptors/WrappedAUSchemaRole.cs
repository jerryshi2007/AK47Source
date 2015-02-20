using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Security.AUClient;
using MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker;
using System.Web.Script.Serialization;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 包装后的管理单元的角色
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WrappedAUSchemaRole
	{
		public const string AUSchemaRolesSchemaType = "AUSchemaRoles";
		public static string AdministrativeUnitParameterName = "AdministrativeUnit";

		private static readonly string[] AUSchemaSchemaType = new string[] { "AUSchemas" };
		private static readonly string[] AUSchemaType = new string[] { "AdminUnits" };
		private static readonly string[] UserSchemaType = new string[] { "Users" };

		private string _SchemaRoleID = null;

		#region Sync Objects
		[NonSerialized]
		private readonly object _ClientAUSyncObject = new object();

		[NonSerialized]
		private readonly object _ClientRoleSyncObject = new object();

		[NonSerialized]
		private readonly object _AUSchemaSyncObject = new object();

		[NonSerialized]
		private readonly object _RoleMembersSyncObject = new object();
		#endregion

		/// <summary>
		/// 根据上下文获得的管理单元对象
		/// </summary>
		//[NonSerialized]
		//private ClientSchemaObjectBase _ClientAUObject = null;

		/// <summary>
		/// 根据上下文获得的管理单元的角色对象
		/// </summary>
		[NonSerialized]
		private ClientAURole _ClientRoleObject = null;

		[NonSerialized]
		private WrappedAURoleMembers _RoleMembers = null;

		[NonSerialized]
		private string _LastestAUCodeName = null;

		[NonSerialized]
		private ClientGenericObject _ClientSchemaRoleObject = null;

		[NonSerialized]
		private ClientSchemaObjectBase _ClientAUSchemaObject = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		public WrappedAUSchemaRole()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="schemaRoleID">管理架构的角色ID</param>
		public WrappedAUSchemaRole(string schemaRoleID)
		{
			schemaRoleID.NullCheck("schemaRoleID");

			this._SchemaRoleID = schemaRoleID;
		}

		public static WrappedAUSchemaRole FromCodeName(string roleFullCodeName)
		{
			return FromCodeName(roleFullCodeName, false);
		}

		/// <summary>
		/// 根据代码名称，获取角色
		/// </summary>
		/// <param name="roleFullCodeName"></param>
		/// <param name="throwIfFail">表示在查询不到数据时，是否应抛出异常</param>
		/// <returns></returns>
		public static WrappedAUSchemaRole FromCodeName(string roleFullCodeName, bool throwIfFail)
		{
			roleFullCodeName.CheckStringIsNullOrEmpty("roleFullCodeName");
			WrappedAUSchemaRole role = null;

			string[] fullCodeNames = roleFullCodeName.Split(':');

			if (fullCodeNames.Length > 1)
			{
				string auSchemaCode = fullCodeNames[0];
				string auRoleCode = fullCodeNames[1];

				role = FromCodeName(auSchemaCode, auRoleCode, throwIfFail);
			}
			else if (throwIfFail)
			{
				throw new FormatException("Full code name is not correct.");
			}

			return role;
		}

		public static WrappedAUSchemaRole FromCodeName(string auSchemaCodeName, string auRoleCodeName, bool throwIfFail)
		{
			WrappedAUSchemaRole role = null;

			var schema = AUCenterQueryService.Instance.GetObjectsByCodeNames(new string[] { auSchemaCodeName }, AUSchemaSchemaType, true).FirstOrDefault();
			if (schema == null)
			{
				if (throwIfFail)
					throw new ObjectNotFoundException("Could not find schema via code \"" + auSchemaCodeName + "\"");
			}
			else
			{
				var schemaRole = AUCenterQueryService.Instance.GetAUSchemaRoles(schema.ID, new string[] { auRoleCodeName }, true).FirstOrDefault();
				if (schemaRole == null)
				{
					if (throwIfFail)
						throw new ObjectNotFoundException("Could not find schema role via code \"" + auRoleCodeName + "\"");
				}
				else
				{
					role = new WrappedAUSchemaRole();

					role._SchemaRoleID = schemaRole.ID;
					role._ClientAUSchemaObject = schema;
					role._ClientSchemaRoleObject = schemaRole;
					role.Name = schemaRole.Name;
					role.Description = schemaRole.DisplayName;
				}
			}

			return role;
		}

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// 管理单元角色或者管理架构角色
		/// </summary>
		public string SchemaRoleID
		{
			get
			{
				return this._SchemaRoleID;
			}
			private set
			{
				this._SchemaRoleID = value;
			}
		}

		/// <summary>
		/// 角色对象
		/// </summary>
		[ScriptIgnore]
		public ClientGenericObject ClientSchemaRoleObject
		{
			get
			{
				if (this._ClientSchemaRoleObject == null)
				{
					lock (this._ClientRoleSyncObject)
					{
						if (this._ClientSchemaRoleObject == null)
						{
							ClientSchemaObjectBase[] roles = AUCenterQueryService.Instance.GetObjectsByIDs(new string[] { this.SchemaRoleID }, new string[] { AUSchemaRolesSchemaType }, true);

							this._ClientSchemaRoleObject = (ClientGenericObject)roles.FirstOrDefault();
						}
					}
				}

				return this._ClientSchemaRoleObject;
			}
		}

		/// <summary>
		/// 角色对应Schema，
		/// </summary>
		[ScriptIgnore]
		public ClientSchemaObjectBase ClientAUSchemaObject
		{
			get
			{
				if (this._ClientAUSchemaObject == null)
				{
					lock (this._AUSchemaSyncObject)
					{
						if (this._ClientAUSchemaObject == null)
						{
							ClientSchemaObjectBase[] containers = AUCenterQueryService.Instance.GetContainers(this.SchemaRoleID, AUSchemaSchemaType, true);

							this._ClientAUSchemaObject = containers.FirstOrDefault();
						}
					}
				}

				return this._ClientAUSchemaObject;
			}
		}

		/// <summary>
		/// 得到某个Schema角色在管理单元下的角色ID
		/// </summary>
		/// <param name="auCodeName"></param>
		/// <returns></returns>
		public ClientAURole GetAURoleObject(string auCodeName)
		{
			ClientAURole result = null;

			if (auCodeName.IsNotEmpty())
			{
				lock (this._ClientAUSyncObject)
				{
					if (this._LastestAUCodeName != auCodeName)
					{
						ClientSchemaObjectBase au = AUCenterQueryService.Instance.GetObjectsByCodeNames(new string[] { auCodeName }, WrappedAUSchemaRole.AUSchemaType, true).FirstOrDefault();

						if (au != null)
							this._ClientRoleObject = AUCenterQueryService.Instance.GetAURoleBySchemaRoleID(au.ID, this.SchemaRoleID, true);
						else
							this._ClientRoleObject = null;

						result = this._ClientRoleObject;
						this._LastestAUCodeName = auCodeName;
					}
					else
						result = this._ClientRoleObject;
				}
			}

			return result;
		}

		/// <summary>
		/// 获取角色中的直接用户
		/// </summary>
		/// <param name="auCodeName"></param>
		/// <returns></returns>
		public IEnumerable<IUser> GetDirectUsers(string auCodeName)
		{
			IEnumerable<IUser> result;

			if (auCodeName.IsNotEmpty())
			{
				InnerRefreshMembers(auCodeName);
				result = this._RoleMembers.Members;
			}
			else
			{
				result = new IUser[0];
			}

			return result;
		}

		private void InnerRefreshMembers(string auCodeName)
		{
			lock (this._RoleMembersSyncObject)
			{
				if (this._RoleMembers != null)
				{
					if (this._RoleMembers.AUCodeName != auCodeName)
						this._RoleMembers = null;
				}

				if (this._RoleMembers == null)
				{
					ClientAURole role = GetAURoleObject(auCodeName);

					if (role != null)
					{
						ClientSchemaMember[] members = AUCenterQueryService.Instance.GetMemberships(role.ID, UserSchemaType, true);

						List<string> userIDs = new List<string>();

						members.ForEach(m => userIDs.Add(m.ID));

						IEnumerable<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userIDs.ToArray());

						this._RoleMembers = new WrappedAURoleMembers(auCodeName, users);
					}
					else
					{
						this._RoleMembers = new WrappedAURoleMembers(auCodeName, new IUser[0]);
					}
				}
			}
		}

		/// <summary>
		/// 得到所有的用户，包括矩阵中的和直接包含的
		/// </summary>
		/// <param name="auCodeName"></param>
		/// <returns></returns>
		public IEnumerable<IUser> GetAllUsers(string auCodeName)
		{
			OguDataCollection<IUser> result = new OguDataCollection<IUser>();

			result.CopyFrom(GetDirectUsers(auCodeName));
			result.CopyFrom(GetObjectsFromMatrix(auCodeName));

			return result;
		}

		public void FillUsers(IWfProcess process, OguDataCollection<IUser> users)
		{
			this.DoCurrentRoleAction(process, (role, auCodeName) =>
					users.CopyFrom(this.GetAllUsers(auCodeName)));
		}

		private static void FillAndDistinctUsers(IEnumerable<IUser> source, OguDataCollection<IUser> target)
		{
			foreach (IUser user in source)
			{
				if (target.Exists(u => string.Compare(u.FullPath, user.FullPath, true) == 0) == false)
					target.Add(user);
			}
		}

		/// <summary>
		/// 得到矩阵中的符合条件的人员
		/// </summary>
		/// <param name="auCodeName"></param>
		/// <returns></returns>
		public IEnumerable<IUser> GetObjectsFromMatrix(string auCodeName)
		{
			List<IUser> result = new List<IUser>();

			SOARole role = GetSOARole(auCodeName);

			if (role != null)
			{
				IEnumerable<IOguObject> objectsInMatrix = role.GetObjectsFromMatrix();

				foreach (IOguObject obj in objectsInMatrix)
				{
					if (obj is IUser)
						result.Add((IUser)obj);
				}
			}
			else
				result = new List<IUser>();

			return result;
		}

		/// <summary>
		/// 根据管理单元的ID得到SOARole。
		/// </summary>
		/// <param name="auCodeName"></param>
		/// <returns></returns>
		public SOARole GetSOARole(string auCodeName)
		{
			SOARole role = null;

			if (this.ClientSchemaRoleObject != null)
			{
				string definitionID = this.ClientSchemaRoleObject.ID;
				string roleID = definitionID;

				ClientAURole auRole = GetAURoleObject(auCodeName);

				if (auRole != null)
					roleID = auRole.ID;

				SOARolePropertyDefinitionCollection definition = SOARolePropertyDefinitionAdapter.Instance.GetByRoleID(definitionID);

				role = new SOARole(definition);
				role.ID = roleID;

				//如果矩阵的行数为零
				if (role.Rows.Count == 0)
				{
					role = new SOARole(definition);
					role.ID = definitionID;
				}
			}

			return role;
		}

		public void DoCurrentRoleAction(IWfProcess process, Action<SOARole, string> action)
		{
			if (action != null)
			{
				string auCodeName = GetCurrentAdministrativeUnitCodeName(process);

				SOARole role = this.GetSOARole(auCodeName);

				if (role != null)
					SOARoleContext.DoAction(role, process, (context) =>
						action(role, auCodeName)
				);
			}
		}

		internal static string GetCurrentAdministrativeUnitCodeName(IWfProcess process)
		{
			WfApplicationRuntimeParameters runtimeParameters = null;

			if (WfApplicationParametersContext.Current != null)
				runtimeParameters = WfApplicationParametersContext.Current.ApplicationRuntimeParameters;

			if (runtimeParameters == null && process != null)
				runtimeParameters = process.ApplicationRuntimeParameters;

			return runtimeParameters.GetValueRecursively(AdministrativeUnitParameterName, string.Empty);
		}
	}
}
