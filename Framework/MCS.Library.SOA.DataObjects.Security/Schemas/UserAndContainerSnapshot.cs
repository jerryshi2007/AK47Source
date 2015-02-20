using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示用户和容器的快照
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.UserAndContainerSnapshot")]
	public class UserAndContainerSnapshot
	{
		/// <summary>
		/// 初始化<see cref="UserAndContainerSnapshot"/>的新实例。
		/// </summary>
		public UserAndContainerSnapshot()
		{
		}

		/// <summary>
		/// 使用指定的<see cref="SCMemberRelation"/>初始化<see cref="UserAndContainerSnapshot"/>的新实例。
		/// </summary>
		/// <param name="mr"></param>
		public UserAndContainerSnapshot(SCMemberRelation mr)
		{
			this.UserID = mr.ID;
			this.UserSchemaType = mr.MemberSchemaType;

			this.ContainerID = mr.ContainerID;
			this.ContainerSchemaType = mr.ContainerSchemaType;
		}

		/// <summary>
		/// 获取或设置表示用户ID的字符串
		/// </summary>
		[ORFieldMapping("UserID", PrimaryKey = true)]
		public string UserID { get; set; }

		/// <summary>
		/// 获取或设置表示容器ID的字符串
		/// </summary>
		[ORFieldMapping("ContainerID", PrimaryKey = true)]
		public string ContainerID { get; set; }

		/// <summary>
		/// 获取或设置一个<see cref="DateTime"/>值，表示版本的开始时间。
		/// </summary>
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public DateTime VersionStartTime { get; set; }

		/// <summary>
		/// 获取或设置一个<see cref="DateTime"/>值，表示版本的结束时间。
		/// </summary>
		[ORFieldMapping("VersionEndTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public DateTime VersionEndTime { get; set; }

		/// <summary>
		/// 获取或设置用户模式类型的字符串
		/// </summary>
		[ORFieldMapping("UserSchemaType")]
		public string UserSchemaType { get; set; }

		/// <summary>
		/// 获取或设置表示容器模式类型的字符串
		/// </summary>
		[ORFieldMapping("ContainerSchemaType")]
		public string ContainerSchemaType { get; set; }

		private SchemaObjectStatus _Status = SchemaObjectStatus.Normal;

		/// <summary>
		/// 表示模式对象的状态
		/// </summary>
		[ORFieldMapping("Status")]
		public virtual SchemaObjectStatus Status
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
	}

	[Serializable]
	public class UserAndContainerSnapshotCollection : EditableDataObjectCollectionBase<UserAndContainerSnapshot>
	{
	}

	/// <summary>
	/// 同一个Container下的用户和容器信息快照，ContainerID是相同的，用户ID为Key
	/// </summary>
	[Serializable]
	public class SameContainerUserAndContainerSnapshotCollection : SerializableEditableKeyedDataObjectCollectionBase<string, UserAndContainerSnapshot>
	{
		public SameContainerUserAndContainerSnapshotCollection()
		{
		}

		protected SameContainerUserAndContainerSnapshotCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override string GetKeyForItem(UserAndContainerSnapshot item)
		{
			return item.UserID;
		}
	}
}
