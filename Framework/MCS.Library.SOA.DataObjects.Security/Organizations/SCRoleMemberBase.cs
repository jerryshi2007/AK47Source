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
	/// 角色成员的虚基类。如果您的对象可以作为角色成员，可以考虑从这里派生
	/// </summary>
	[Serializable]
	public abstract class SCRoleMemberBase : SCBase
	{
		/// <summary>
		/// 使用指定的模式类型字符串初始化<see cref="SCRoleMemberBase"/>成员。
		/// </summary>
		/// <param name="schemaType">模式类型的字符串</param>
		public SCRoleMemberBase(string schemaType) :
			base(schemaType)
		{
		}

		[NonSerialized]
		private SCObjectContainerRelationCollection _AllRolesRelations = null;

		/// <summary>
		/// 获取表示所有角色关系的集合
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection AllRolesRelations
		{
			get
			{
				if (this._AllRolesRelations == null && this.ID.IsNotEmpty())
					this._AllRolesRelations = SCMemberRelationAdapter.Instance.LoadByMemberID(this.ID, StandardObjectSchemaType.Roles.ToString());

				return this._AllRolesRelations;
			}
		}

		/// <summary>
		/// 获取当前的角色关系
		/// </summary>
		/// <value>一个<see cref="SCObjectContainerRelationCollection"/></value>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection CurrentRolesRelations
		{
			get
			{
				return (SCObjectContainerRelationCollection)AllRolesRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SCRoleCollection _AllRoles = null;

		/// <summary>
		/// 获取所有角色的集合
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection AllRoles
		{
			get
			{
				if (this._AllRoles == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection roles = SchemaObjectAdapter.Instance.Load(AllRolesRelations.ToContainerIDsBuilder());

					this._AllRoles = new SCRoleCollection();
					roles.ForEach(r => this._AllRoles.Add((SCRole)r));
				}

				return this._AllRoles;
			}
		}

		[NonSerialized]
		private SCRoleCollection _CurrentRoles = null;

		/// <summary>
		/// 获取当前角色的集合
		/// </summary>
		///<value>一个<see cref="SCRoleCollection"/>，表示用户当前的角色</value> 
		[ScriptIgnore]
		[NoMapping]
		public SCRoleCollection CurrentRoles
		{
			get
			{
				if (this._CurrentRoles == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection roles = SchemaObjectAdapter.Instance.Load(CurrentRolesRelations.ToContainerIDsBuilder());

					this._CurrentRoles = new SCRoleCollection();
					roles.ForEach(r => this._CurrentRoles.Add((SCRole)r));
					this._CurrentRoles = this._CurrentRoles.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return _CurrentRoles;
			}
		}

		/// <summary>
		/// 处理ID改变
		/// </summary>
		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllRoles = null;
			this._AllRolesRelations = null;
			this._CurrentRoles = null;
		}
	}
}
