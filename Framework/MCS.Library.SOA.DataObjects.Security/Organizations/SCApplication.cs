using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class SCApplication : SCBase, ISCAclContainer
	{
		public SCApplication() :
			base(StandardObjectSchemaType.Applications.ToString())
		{
		}

		public SCApplication(string schemaType)
			: base(schemaType)
		{
		}

		[NonSerialized]
		private SCObjectMemberRelationCollection _AllRolesRelations = null;

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection AllRolesRelations
		{
			get
			{
				if (this._AllRolesRelations == null && this.ID.IsNotEmpty())
					this._AllRolesRelations = SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID, StandardObjectSchemaType.Roles.ToString());

				return this._AllRolesRelations;
			}
		}

		[NonSerialized]
		private SCRoleCollection _AllRoles = null;

		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection AllRoles
		{
			get
			{
				if (this._AllRoles == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection roles = SchemaObjectAdapter.Instance.Load(AllRolesRelations.ToMemberIDsBuilder());

					this._AllRoles = new SCRoleCollection();
					roles.ForEach(r => this._AllRoles.Add((SCRole)r));
				}

				return this._AllRoles;
			}
		}

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection CurrentRolesRelations
		{
			get
			{
				return (SCObjectMemberRelationCollection)AllRolesRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SCRoleCollection _CurrentRoles = null;

		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection CurrentRoles
		{
			get
			{
				if (this._CurrentRoles == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection roles = SchemaObjectAdapter.Instance.Load(CurrentRolesRelations.ToMemberIDsBuilder());

					this._CurrentRoles = new SCRoleCollection();

					foreach (SCRole role in roles)
					{
						if (role.Status == SchemaObjectStatus.Normal)
							this._CurrentRoles.Add(role);
					}
				}

				return this._CurrentRoles;
			}
		}

		[NonSerialized]
		private SCObjectMemberRelationCollection _AllPermissionsRelations = null;

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection AllPermissionsRelations
		{
			get
			{
				if (this._AllPermissionsRelations == null && this.ID.IsNotEmpty())
					this._AllPermissionsRelations = SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID, StandardObjectSchemaType.Permissions.ToString());

				return this._AllPermissionsRelations;
			}
		}

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection CurrentPermissionsRelations
		{
			get
			{
				return (SCObjectMemberRelationCollection)AllPermissionsRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SCPermissionCollection _AllPermissions = null;

		[ScriptIgnore]
		[NoMapping]
		public SCPermissionCollection AllPermissions
		{
			get
			{
				if (this._AllPermissions == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection permissions = SchemaObjectAdapter.Instance.Load(AllPermissionsRelations.ToMemberIDsBuilder());

					this._AllPermissions = new SCPermissionCollection();
					permissions.ForEach(r => this._AllPermissions.Add((SCPermission)r));
				}

				return this._AllPermissions;
			}
		}

		[NonSerialized]
		private SCPermissionCollection _CurrentPermissions = null;

		[ScriptIgnore]
		[NoMapping]
		public SCPermissionCollection CurrentPermissions
		{
			get
			{
				if (this._CurrentPermissions == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection permissions = SchemaObjectAdapter.Instance.Load(CurrentPermissionsRelations.ToMemberIDsBuilder());

					this._CurrentPermissions = new SCPermissionCollection();

					foreach (SCPermission permission in permissions)
					{
						if (permission.Status == SchemaObjectStatus.Normal)
							this._CurrentPermissions.Add(permission);
					}
				}

				return this._CurrentPermissions;
			}
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllPermissions = null;
			this._AllPermissionsRelations = null;
			this._AllRoles = null;
			this._AllRolesRelations = null;
			this._CurrentRoles = null;
			this._CurrentPermissions = null;
		}

		#region ISCAclContainer Members

		public SCAclMemberCollection GetAclMembers()
		{
			return SCAclAdapter.Instance.LoadByContainerID(this.ID, SchemaObjectStatus.Normal, DateTime.MinValue);
		}

		#endregion
	}
}
