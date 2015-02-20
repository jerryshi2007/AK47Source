using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示权限（功能）
	/// </summary>
	[Serializable]
	public class SCPermission : SCBase, ISCMemberObject, ISCApplicationMember
	{
		/// <summary>
		/// 初始化<see cref="SCPermission"/>的新实例
		/// </summary>
		public SCPermission() :
			base(StandardObjectSchemaType.Permissions.ToString())
		{
		}

		public SCPermission(string schemaType)
			: base(schemaType)
		{
		}

		[NonSerialized]
		private SCRoleCollection _AllRoles = null;

		/// <summary>
		/// 获取一个<see cref="SCRoleCollection"/>，表示所有的角色
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection AllRoles
		{
			get
			{
				if (this._AllRoles == null && this.ID.IsNotEmpty())
				{
					this._AllRoles = new SCRoleCollection();

					this.AllParents.ForEach(c => this._AllRoles.Add((SCRole)(c)));
				}

				return this._AllRoles;
			}
		}

		[NonSerialized]
		private SCRoleCollection _CurrentRoles = null;

		/// <summary>
		/// 获取一个<see cref="SCRoleCollection"/>，表示当前的角色。
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection CurrentRoles
		{
			get
			{
				if (this._CurrentRoles == null && this.ID.IsNotEmpty())
				{
					this._CurrentRoles = new SCRoleCollection();

					this.CurrentParents.ForEach(c => this._CurrentRoles.Add((SCRole)(c)));
				}

				return this._CurrentRoles;
			}
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllRoles = null;
			this._CurrentRoles = null;
			this._CurrentApplication = null;
		}

		#region ISCMemberObject Members

		public SCObjectContainerRelationCollection GetCurrentMemberOfRelations()
		{
			return SCMemberRelationAdapter.Instance.LoadByMemberID(this.ID);
		}

		#endregion

		#region ISCApplicationMember Members

		[NonSerialized]
		private SCApplication _CurrentApplication = null;

		[ScriptIgnore]
		public SCApplication CurrentApplication
		{
			get
			{
				if (this._CurrentApplication == null)
				{
					SCSimpleRelationBase mr = GetCurrentMemberOfRelations().FirstOrDefault();

					if (mr != null)
						this._CurrentApplication = SchemaObjectAdapter.Instance.Load(mr.ContainerID) as SCApplication;
				}

				return this._CurrentApplication;
			}
		}

		#endregion
	}

	/// <summary>
	/// 表示权限(功能)的集合
	/// </summary>
	[Serializable]
	public class SCPermissionCollection : SchemaObjectEditableKeyedCollectionBase<SCPermission, SCPermissionCollection>
	{
		protected override SCPermissionCollection CreateFilterResultCollection()
		{
			return new SCPermissionCollection();
		}

		protected override string GetKeyForItem(SCPermission item)
		{
			return item.ID;
		}
	}
}
