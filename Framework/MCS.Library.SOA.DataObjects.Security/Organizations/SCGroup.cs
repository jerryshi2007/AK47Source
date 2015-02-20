using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class SCGroup : SCRoleMemberBase, ISCContainerObject, ISCUserContainerWithConditionObject
	{
		public SCGroup() :
			base(StandardObjectSchemaType.Groups.ToString())
		{
		}

		public SCGroup(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true)]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;

				this._AllMembersRelations = null;
			}
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

		private SCUserCollection _AllUsers = null;

		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection AllUsers
		{
			get
			{
				if (this._AllUsers == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(AllMembersRelations.ToMemberIDsBuilder());

					this._AllUsers = new SCUserCollection();
					users.ForEach(r => this._AllUsers.Add((SCUser)r));
				}

				return this._AllUsers;
			}
		}

		private SCUserCollection _CurrentUsers = null;

		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection CurrentUsers
		{
			get
			{
				if (this._CurrentUsers == null && this.ID.IsNotEmpty())
				{
					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(CurrentMembersRelations.ToMemberIDsBuilder());

					this._CurrentUsers = new SCUserCollection();

					foreach (SCUser user in users)
					{
						if (user.Status == SchemaObjectStatus.Normal)
							this._CurrentUsers.Add(user);
					}
				}

				return _CurrentUsers;
			}
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllMembersRelations = null;
			this._AllUsers = null;
			this._CurrentUsers = null;
		}

		public SCObjectMemberRelationCollection GetCurrentMembersRelations()
		{
			return this.CurrentMembersRelations;
		}

		public SchemaObjectCollection GetCurrentMembers()
		{
			return this.CurrentUsers.ToSchemaObjects();
		}

		#region ISCUserContainerObject Members

		/// <summary>
		/// 得到所有预定义的用户
		/// </summary>
		/// <returns></returns>
		public SchemaObjectCollection GetCurrentUsers()
		{
			return this.GetCurrentMembers().ToUsers(true);
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
	}

	[Serializable]
	public class SCGroupCollection : SchemaObjectEditableKeyedCollectionBase<SCGroup, SCGroupCollection>
	{
		protected override SCGroupCollection CreateFilterResultCollection()
		{
			return new SCGroupCollection();
		}

		protected override string GetKeyForItem(SCGroup item)
		{
			return item.ID;
		}
	}
}
