using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Permissions
{
	/// <summary>
	/// 权限中心操作的访问控制列表
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.Acl")]
	public class SCAclItem : IVersionDataObject, ISCStatusObject
	{
		private SchemaObjectStatus _Status = SchemaObjectStatus.Normal;

		public SCAclItem()
		{
		}

		public SCAclItem(string containerPermission, SchemaObjectBase member)
		{
			member.NullCheck("member");
			containerPermission.CheckStringIsNullOrEmpty("containerPermission");

			this.MemberID = member.ID;
			this.MemberSchemaType = member.SchemaType;
			this.ContainerPermission = containerPermission;
		}

		[ORFieldMapping("ContainerID", PrimaryKey = true)]
		public string ContainerID
		{
			get;
			set;
		}

		[ORFieldMapping("ContainerPermission", PrimaryKey = true)]
		public string ContainerPermission
		{
			get;
			set;
		}

		[ORFieldMapping("MemberID", PrimaryKey = true)]
		public string MemberID
		{
			get;
			set;
		}

		[ORFieldMapping("VersionStartTime")]
		public DateTime VersionStartTime
		{
			get;
			set;
		}

		[ORFieldMapping("VersionEndTime")]
		public DateTime VersionEndTime
		{
			get;
			set;
		}

		[ORFieldMapping("Status")]
		public SchemaObjectStatus Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}

		[ORFieldMapping("SortID")]
		public int SortID
		{
			get;
			set;
		}

		[ORFieldMapping("ContainerSchemaType")]
		public string ContainerSchemaType
		{
			get;
			set;
		}

		[ORFieldMapping("MemberSchemaType")]
		public string MemberSchemaType
		{
			get;
			set;
		}

		#region IVersionDataObject Members
		[NoMapping]
		string IVersionDataObject.ID
		{
			get
			{
				return this.ContainerID + "-" + this.MemberID;
			}
		}

		#endregion
	}

	/// <summary>
	/// Acl中容器和成员的集合类的基类
	/// </summary>
	[Serializable]
	public abstract class SCAclContainerOrMemberCollectionBase : SerializableEditableKeyedDataObjectCollectionBase<string, SCAclItem>
	{
		protected SCAclContainerOrMemberCollectionBase()
		{
		}

		protected SCAclContainerOrMemberCollectionBase(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}

	/// <summary>
	/// Acl成员集合。这个集合通常挂在容器上
	/// </summary>
	[Serializable]
	public class SCAclMemberCollection : SCAclContainerOrMemberCollectionBase
	{
		public SCAclMemberCollection()
		{
		}

		/// <summary>
		/// 合并需要变更的AclMember集合。假设当前集合是新集合，提供原始的集合，比较出哪些是需要变更的。
		/// </summary>
		/// <param name="original"></param>
		/// <returns>是否有变化产生</returns>
		public bool MergeChangedItems(SCAclMemberCollection original)
		{
			original.NullCheck("original");

			bool result = false;

			foreach (SCAclItem item in this)
			{
				//确认新集合每一项在老集合中是否存在
				if (original.ContainsKey(item.ContainerPermission, item.MemberID) == false)
					result = true;
			}

			foreach (SCAclItem item in original)
			{
				//确认老集合每一项在新集合中是否存在，如果不存在，则添加删除的记录
				if (this.ContainsKey(item.ContainerPermission, item.MemberID) == false)
				{
					item.Status = SchemaObjectStatus.Deleted;
					this.Add(item);
					result = true;
				}
			}

			return result;
		}

		public void Add(string containerPermission, SchemaObjectBase member)
		{
			this.Add(new SCAclItem(containerPermission, member));
		}

		protected override string GetKeyForItem(SCAclItem item)
		{
			return item.ContainerPermission + "|" + item.MemberID;
		}

		public SCAclItem this[string permission, string memberID]
		{
			get { return this[permission + "|" + memberID]; }
		}

		private new SCAclItem this[string key]
		{
			get { return base[key]; }
		}

		public bool ContainsKey(string permission, string memberID)
		{
			return base.ContainsKey(permission + "|" + memberID);
		}

		public void ContainsKey(string permission, string memberID, Action<SCAclItem> action)
		{
			base.ContainsKey(permission + "|" + memberID, action);
		}

		private new bool ContainsKey(string key)
		{
			return base.ContainsKey(key);
		}

		private new void ContainsKey(string key, Action<SCAclItem> action)
		{
			base.ContainsKey(key, action);
		}
	}

	/// <summary>
	/// Acl容器集合。这个集合通常挂在成员上
	/// </summary>
	[Serializable]
	public class SCAclContainerCollection : SCAclContainerOrMemberCollectionBase
	{
		public SCAclContainerCollection()
		{
		}

		protected override string GetKeyForItem(SCAclItem item)
		{
			return item.ContainerID + "|" + item.MemberID + "|" + item.ContainerPermission;
		}
	}
}
