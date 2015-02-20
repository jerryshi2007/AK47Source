using System;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示用户
	/// </summary>
	[Serializable]
	public class SCUser : SCRoleMemberBase, ISCMemberObject, ISCContainerObject
	{
		/// <summary>
		/// 初始化<see cref="SCUser"/>的新实例
		/// </summary>
		public SCUser() :
			base(StandardObjectSchemaType.Users.ToString())
		{
		}

		public SCUser(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 所有者ID
		/// </summary>
		[NoMapping]
		public string OwnerID
		{
			get
			{
				return this.Properties.GetValue("OwnerID", string.Empty);
			}
			set
			{
				this.Properties.SetValue("OwnerID", value);
			}
		}

		/// <summary>
		/// 所有者名称
		/// </summary>
		[NoMapping]
		public string OwnerName
		{
			get
			{
				return this.Properties.GetValue("OwnerName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("OwnerName", value);
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

		/// <summary>
		/// 获取一个<see cref="SCObjectMemberRelationCollection"/>，表示当前用户的成员
		/// </summary>
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
		private SchemaObjectCollection _CurrentMembers = null;

		/// <summary>
		/// 获取用户的当前成员，一般是秘书关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectCollection CurrentMembers
		{
			get
			{
				if (this._CurrentMembers == null && this.ID.IsNotEmpty())
				{
					this._CurrentMembers = SchemaObjectAdapter.Instance.Load(CurrentMembersRelations.ToMemberIDsBuilder());
				}

				return _CurrentMembers;
			}
		}

		[NonSerialized]
		private SCObjectContainerRelationCollection _AllMemberOfRelations = null;

		/// <summary>
		/// 获取用户的所有成员关系的集合
		/// </summary>
		/// <value> 一个<see cref="SCObjectContainerRelationCollection"/></value>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection AllMemberOfRelations
		{
			get
			{
				if (this._AllMemberOfRelations == null && this.ID.IsNotEmpty())
					this._AllMemberOfRelations = SCMemberRelationAdapter.Instance.LoadByMemberID(this.ID);

				return this._AllMemberOfRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCObjectContainerRelationCollection"/>，表示当前用户的成员关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection CurrentMemberOfRelations
		{
			get
			{
				return (SCObjectContainerRelationCollection)AllMemberOfRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SCGroupCollection _AllGroups = null;

		/// <summary>
		/// 获取用户的所有群组的集合
		/// </summary>
		/// <value><see cref="SCGroupCollection"/>对象</value> 
		[ScriptIgnore]
		[NoMapping]
		public SCGroupCollection AllGroups
		{
			get
			{
				if (this._AllGroups == null && this.ID.IsNotEmpty())
				{
					SCObjectContainerRelationCollection fr = (SCObjectContainerRelationCollection)AllMemberOfRelations.FilterByContainerSchemaType(StandardObjectSchemaType.Groups.ToString());

					SchemaObjectCollection groups = SchemaObjectAdapter.Instance.Load(fr.ToContainerIDsBuilder());

					this._AllGroups = new SCGroupCollection();
					groups.ForEach(g => this._AllGroups.Add((SCGroup)g));
				}

				return this._AllGroups;
			}
		}

		[NonSerialized]
		private SCGroupCollection _CurrentGroups = null;

		/// <summary>
		/// 获取用户当前的群组的集合
		/// </summary>
		/// <see cref="SCGroupCollection"/>，表示用户的群组
		[ScriptIgnore]
		[NoMapping]
		public SCGroupCollection CurrentGroups
		{
			get
			{
				if (this._CurrentGroups == null && this.ID.IsNotEmpty())
				{
					SCObjectContainerRelationCollection fr = (SCObjectContainerRelationCollection)CurrentMemberOfRelations.FilterByContainerSchemaType(StandardObjectSchemaType.Groups.ToString());

					SchemaObjectCollection groups = SchemaObjectAdapter.Instance.Load(fr.ToContainerIDsBuilder());

					this._CurrentGroups = new SCGroupCollection();

					foreach (SCGroup group in groups)
					{
						if (group.Status == SchemaObjectStatus.Normal)
							this._CurrentGroups.Add((SCGroup)group);
					}
				}

				return this._CurrentGroups;
			}
		}

		[NonSerialized]
		private SCUserCollection _AllSecretaries = null;

		/// <summary>
		/// 获取用户的所有秘书的集合
		/// </summary>
		/// <value><see cref="SCUserCollection"/>对象</value> 
		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection AllSecretaries
		{
			get
			{
				if (this._AllSecretaries == null && this.ID.IsNotEmpty())
				{
					SCObjectMemberRelationCollection fr = (SCObjectMemberRelationCollection)AllMembersRelations.FilterBySchemaType(StandardObjectSchemaType.SecretaryRelations.ToString());

					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(fr.ToMemberIDsBuilder());

					this._AllSecretaries = new SCUserCollection();
					users.ForEach(u => this._AllSecretaries.Add((SCUser)u));
				}

				return this._AllSecretaries;
			}
		}

		[NonSerialized]
		private SCUserCollection _CurrentSecretaries = null;

		/// <summary>
		/// 获取当前人的秘书
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection CurrentSecretaries
		{
			get
			{
				if (this._CurrentSecretaries == null && this.ID.IsNotEmpty())
				{
					SCObjectMemberRelationCollection fr = (SCObjectMemberRelationCollection)CurrentMembersRelations.FilterBySchemaType(StandardObjectSchemaType.SecretaryRelations.ToString());

					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(fr.ToMemberIDsBuilder());

					this._CurrentSecretaries = new SCUserCollection();

					foreach (SCUser user in users)
					{
						if (user.Status == SchemaObjectStatus.Normal)
							this._CurrentSecretaries.Add((SCUser)user);
					}
				}

				return this._CurrentSecretaries;
			}
		}

		[NonSerialized]
		private SCUserCollection _AllSecretariesOf = null;

		/// <summary>
		/// 获取用户的所担当的秘书的集合
		/// </summary>
		/// <value><see cref="SCUserCollection"/>对象</value> 
		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection AllSecretariesOf
		{
			get
			{
				if (this._AllSecretariesOf == null && this.ID.IsNotEmpty())
				{
					SCObjectContainerRelationCollection fr = (SCObjectContainerRelationCollection)AllMemberOfRelations.FilterBySchemaType(StandardObjectSchemaType.SecretaryRelations.ToString());

					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(fr.ToContainerIDsBuilder());

					this._AllSecretariesOf = new SCUserCollection();
					users.ForEach(u => this._AllSecretariesOf.Add((SCUser)u));
				}

				return this._AllSecretariesOf;
			}
		}

		[NonSerialized]
		private SCUserCollection _CurrentSecretariesOf = null;

		/// <summary>
		/// 获取当前人的秘书
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCUserCollection CurrentSecretariesOf
		{
			get
			{
				if (this._CurrentSecretariesOf == null && this.ID.IsNotEmpty())
				{
					SCObjectContainerRelationCollection fr = (SCObjectContainerRelationCollection)CurrentMemberOfRelations.FilterBySchemaType(StandardObjectSchemaType.SecretaryRelations.ToString());

					SchemaObjectCollection users = SchemaObjectAdapter.Instance.Load(fr.ToContainerIDsBuilder());

					this._CurrentSecretariesOf = new SCUserCollection();

					foreach (SCUser user in users)
					{
						if (user.Status == SchemaObjectStatus.Normal)
							this._CurrentSecretariesOf.Add((SCUser)user);
					}
				}

				return this._CurrentSecretariesOf;
			}
		}

		/// <summary>
		/// 获取或设置用户的名
		/// </summary>
		[NoMapping]
		public string FirstName
		{
			get
			{
				return this.Properties.GetValue("FirstName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("FirstName", value);
			}
		}

		/// <summary>
		/// 获取或设置用户的姓
		/// </summary>
		[NoMapping]
		public string LastName
		{
			get
			{
				return this.Properties.GetValue("LastName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("LastName", value);
			}
		}

		/// <summary>
		/// 处理ID改变
		/// </summary>
		protected override void OnIDChanged()
		{
			base.OnIDChanged();
			this._AllMemberOfRelations = null;
			this._AllGroups = null;
			this._CurrentGroups = null;
			this._AllSecretaries = null;
			this._CurrentSecretaries = null;
			this._AllSecretariesOf = null;
			this._CurrentSecretariesOf = null;
			this._AllMembersRelations = null;
			this._CurrentMembers = null;
		}

		SCObjectContainerRelationCollection ISCMemberObject.GetCurrentMemberOfRelations()
		{
			return this.CurrentMemberOfRelations;
		}

		#region ISCContainerObject Members

		public SCObjectMemberRelationCollection GetCurrentMembersRelations()
		{
			return this.CurrentMembersRelations;
		}

		public SchemaObjectCollection GetCurrentMembers()
		{
			return this.CurrentMembers;
		}

		#endregion
	}

	/// <summary>
	///  表示<see cref="SCUser"/>的集合
	/// </summary>
	[Serializable]
	public class SCUserCollection : SchemaObjectEditableKeyedCollectionBase<SCUser, SCUserCollection>
	{
		public SchemaObjectCollection ToSchemaObjects()
		{
			SchemaObjectCollection result = new SchemaObjectCollection();

			this.ForEach(u => result.Add(u));

			return result;
		}

		/// <summary>
		/// 创建过滤器结果集合
		/// </summary>
		/// <returns></returns>
		protected override SCUserCollection CreateFilterResultCollection()
		{
			return new SCUserCollection();
		}

		/// <summary>
		/// 获取集合中指定的<see cref="SCUser"/>的键。
		/// </summary>
		/// <param name="item">获取其键的<see cref="SCUser"/></param>
		/// <returns>表示键的字符串</returns>
		protected override string GetKeyForItem(SCUser item)
		{
			return item.ID;
		}
	}
}
