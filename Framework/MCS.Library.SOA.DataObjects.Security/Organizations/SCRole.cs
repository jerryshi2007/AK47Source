using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class SCRole : SCBase, ISCContainerObject, ISCRelationContainer, ISCUserContainerWithConditionObject, ISCAclMember, ISCMemberObject, ISCApplicationMember
	{
		public SCRole() :
			base(StandardObjectSchemaType.Roles.ToString())
		{
		}

		public SCRole(string schemaType)
			: base(schemaType)
		{
		}

		[NonSerialized]
		private SCObjectMemberRelationCollection _AllMembersRelations = null;

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection AllMembersRelations
		{
			get
			{
				if (this._AllMembersRelations == null && this.ID.IsNotEmpty())
					this._AllMembersRelations = SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID);

				return this._AllMembersRelations;
			}
		}

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection CurrentMembersRelations
		{
			get
			{
				return (SCObjectMemberRelationCollection)AllMembersRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _AllMembers = null;

		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectCollection AllMembers
		{
			get
			{
				if (this._AllMembers == null && this.ID.IsNotEmpty())
					this._AllMembers = SchemaObjectAdapter.Instance.Load(AllMembersRelations.ToMemberIDsBuilder());

				return this._AllMembers;
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentMembers = null;

		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectCollection CurrentMembers
		{
			get
			{
				if (this._CurrentMembers == null && this.ID.IsNotEmpty())
				{
					this._CurrentMembers = SchemaObjectAdapter.Instance.Load(CurrentMembersRelations.ToMemberIDsBuilder());

					this._CurrentMembers = this._CurrentMembers.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return this._CurrentMembers;
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
					this._AllPermissions = new SCPermissionCollection();

					this.AllChildren.ForEach(c => this._AllPermissions.Add((SCPermission)(c)));
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
					this._CurrentPermissions = new SCPermissionCollection();

					this.CurrentChildren.ForEach(c => this._CurrentPermissions.Add((SCPermission)(c)));
				}

				return this._CurrentPermissions;
			}
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllMembersRelations = null;
			this._AllPermissions = null;
			this._CurrentPermissions = null;
			this._CurrentMembers = null;

			this._AllChildrenRelations = null;
			this._AllChildren = null;
			this._CurrentlChildren = null;
			this._CurrentApplication = null;
		}

		public SCObjectMemberRelationCollection GetCurrentMembersRelations()
		{
			return this.CurrentMembersRelations;
		}

		public SchemaObjectCollection GetCurrentMembers()
		{
			return this.CurrentMembers;
		}

		#region ISCRelationContainer Members

		[NonSerialized]
		private SCChildrenRelationObjectCollection _AllChildrenRelations = null;

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示所有子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection AllChildrenRelations
		{
			get
			{
				if (this._AllChildrenRelations == null && this.ID.IsNotEmpty())
				{
					this._AllChildrenRelations = SchemaRelationObjectAdapter.Instance.LoadByParentID(this.ID);
				}

				return _AllChildrenRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示当前子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection CurrentChildrenRelations
		{
			get
			{
				return (SCChildrenRelationObjectCollection)AllChildrenRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _AllChildren = null;

		/// <summary>
		/// 获取一个<see cref="SchemaObjectCollection"/>，表示所有的子级
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection AllChildren
		{
			get
			{
				if (this._AllChildren == null && this.ID.IsNotEmpty())
					this._AllChildren = SchemaObjectAdapter.Instance.Load(AllChildrenRelations.ToChildrenIDsBuilder());

				return this._AllChildren;
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentlChildren = null;

		/// <summary>
		/// 获取一个表示当前的子级的<see cref="SchemaObjectCollection"/>
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection CurrentChildren
		{
			get
			{
				if (this._CurrentlChildren == null && this.ID.IsNotEmpty())
				{
					this._CurrentlChildren = SchemaObjectAdapter.Instance.Load(CurrentChildrenRelations.ToChildrenIDsBuilder());
					this._CurrentlChildren = this._CurrentlChildren.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return this._CurrentlChildren;
			}
		}
		#endregion

		int ISCRelationContainer.GetCurrentChildrenCount()
		{
			return SchemaRelationObjectAdapter.Instance.GetChildrenCount(this.ID, null, DateTime.MinValue);
		}

		int ISCRelationContainer.GetCurrentMaxInnerSort()
		{
			return SchemaRelationObjectAdapter.Instance.GetMaxInnerSort(this.ID, null, DateTime.MinValue);
		}

		#region ISCUserContainerObject Members

		/// <summary>
		/// 得到所有预定义的用户
		/// </summary>
		/// <returns></returns>
		public SchemaObjectCollection GetCurrentUsers()
		{
			SchemaObjectCollection result = this.CurrentMembers.ToUsers(true);

			if (this.Properties.GetValue("includeMatrixUsers", false))
				EnumMatrixUsers(result);

			return result;
		}

		#endregion

		#region ISCUserContainerWithConditionObject Members

		/// <summary>
		/// 得到存放在计算结果表里的计算结果
		/// </summary>
		/// <returns></returns>
		public SchemaObjectCollection GetCalculatedUsers()
		{
			return ConditionCalculateResultAdapter.Instance.LoadCurrentUsers(this.ID);
		}

		/// <summary>
		/// 得到所有预定义的和计算的用户
		/// </summary>
		/// <returns></returns>
		public SchemaObjectCollection GetAllCurrentAndCalculatedUsers()
		{
			SchemaObjectCollection result = GetCurrentUsers();

			result.Merge(GetCalculatedUsers());

			return result;
		}

		#endregion

		#region ISCAclMember Members

		public Permissions.SCAclContainerCollection GetAclContainers()
		{
			return SCAclAdapter.Instance.LoadByMemberID(this.ID, SchemaObjectStatus.Normal, DateTime.MinValue);
		}

		#endregion

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

		#region Private
		/// <summary>
		/// 枚举矩阵中的人员
		/// </summary>
		/// <returns></returns>
        private void EnumMatrixUsers(SchemaObjectCollection result)
		{
			if (this.CurrentApplication != null)
			{
				SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRoleID(this.ID, null);

				SCRoleEnumMatrixUsersContext context = new SCRoleEnumMatrixUsersContext();

				context.CachedApplication.Add(this.CurrentApplication.CodeName, this.CurrentApplication);

				string fullCodeName = this.GetFullCodeName();

				context.CalculatedRolesCodeNames.Add(fullCodeName, fullCodeName);

				InternalEnumMatrixUsers(this, context);

				FillMatrixUsers(context, result);
			}
		}

		private static void FillMatrixUsers(SCRoleEnumMatrixUsersContext context, SchemaObjectCollection result)
		{
			SchemaObjectCollection users = SchemaObjectAdapter.Instance.LoadByCodeName(builder => { }, DateTime.MinValue, context.UsersCodeNames.Keys.ToArray());

			result.Merge(users);
		}

		private static void InternalEnumMatrixUsers(SCRole role, SCRoleEnumMatrixUsersContext context)
		{
			SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRoleID(role.ID, null);

			IEnumerator<SOARolePropertyRow> enumerator = rows.GetEnumerator();

			role.InternalEnumMatrixRows(context, enumerator);
		}

		private void InternalEnumMatrixRows(SCRoleEnumMatrixUsersContext context, IEnumerator<SOARolePropertyRow> enumerator)
		{
			if (enumerator.MoveNext())
			{
				SOARolePropertyRow row = enumerator.Current;

				switch (row.OperatorType)
				{
					case SOARoleOperatorType.User:
						if (context.UsersCodeNames.ContainsKey(row.Operator) == false)
							context.UsersCodeNames.Add(row.Operator, row.Operator);
						break;
					case SOARoleOperatorType.Role:
						if (row.Operator.IndexOf(":") >= 0)
							EnumInternalRoleMatrixUsers(context, row.Operator);
						break;
				}

				InternalEnumMatrixRows(context, enumerator);
			}
		}

		private void EnumInternalRoleMatrixUsers(SCRoleEnumMatrixUsersContext context, string roleFullCodeName)
		{
			//防止嵌套后的死循环
			if (context.CalculatedRolesCodeNames.ContainsKey(roleFullCodeName) == false)
			{
				SCRole role = LoadRoleByFullCodeName(context, roleFullCodeName);

				context.CalculatedRolesCodeNames.Add(roleFullCodeName, roleFullCodeName);

				if (role != null)
					InternalEnumMatrixUsers(role, context);
			}
		}

		private SCRole LoadRoleByFullCodeName(SCRoleEnumMatrixUsersContext context, string roleFullCodeName)
		{
			string[] nameParts = roleFullCodeName.Split(':');

			string appCodeName = nameParts[0].Trim(' ');
			string roleCodeName = nameParts[1].Trim(' ');

			SCApplication app = null;

			if (context.CachedApplication.TryGetValue(appCodeName, out app) == false)
			{
				app = SchemaObjectAdapter.Instance.LoadByCodeName(this.CurrentApplication.SchemaType, appCodeName, SchemaObjectStatus.Normal, DateTime.MinValue) as SCApplication;

				context.CachedApplication.Add(appCodeName, app);
			}

			SCRole role = null;

			if (app != null)
                role = SchemaObjectAdapter.Instance.LoadByCodeName(this.SchemaType, roleCodeName, SchemaObjectStatus.Normal, DateTime.MinValue) as SCRole;

			return role;
		}

		private string GetFullCodeName()
		{
			string appCodeName = string.Empty;

			if (this.CurrentApplication != null)
				appCodeName = this.CurrentApplication.CodeName;

			return appCodeName + ":" + this.CodeName;
		}
		#endregion Private
	}

	[Serializable]
	public class SCRoleCollection : SchemaObjectEditableKeyedCollectionBase<SCRole, SCRoleCollection>
	{
		protected override SCRoleCollection CreateFilterResultCollection()
		{
			return new SCRoleCollection();
		}

		protected override string GetKeyForItem(SCRole item)
		{
			return item.ID;
		}
	}
}
