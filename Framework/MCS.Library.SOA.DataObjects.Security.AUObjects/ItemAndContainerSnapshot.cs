using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using System.Runtime.Serialization;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	[Serializable]
	[ORTableMapping("SC.ItemAndContainerSnapshot")]
	public class ItemAndContainerSnapshot
	{
		/// <summary>
		/// 初始化<see cref="ItemAndContainerSnapshot"/>的新实例。
		/// </summary>
		public ItemAndContainerSnapshot()
		{
		}

		/// <summary>
		/// 使用指定的<see cref="SCMemberRelation"/>初始化<see cref="ItemAndContainerSnapshot"/>的新实例。
		/// </summary>
		/// <param name="mr"></param>
		public ItemAndContainerSnapshot(SCMemberRelation mr)
		{
			this.ItemID = mr.ID;
			this.ItemSchemaType = mr.MemberSchemaType;

			this.ContainerID = mr.ContainerID;
			this.ContainerSchemaType = mr.ContainerSchemaType;
		}

		/// <summary>
		/// 获取或设置表示用户ID的字符串
		/// </summary>
		[ORFieldMapping("ItemID", PrimaryKey = true)]
		public string ItemID { get; set; }

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
		[ORFieldMapping("ItemSchemaType")]
		public string ItemSchemaType { get; set; }

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
	public class ItemAndContainerSnapshotCollection : EditableDataObjectCollectionBase<ItemAndContainerSnapshot>
	{
	}

	/// <summary>
	/// 同一个Container下的用户和容器信息快照，ContainerID是相同的，用户ID为Key
	/// </summary>
	[Serializable]
	public class SameContainerItemAndContainerSnapshotCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ItemAndContainerSnapshot>
	{
		public SameContainerItemAndContainerSnapshotCollection()
		{
		}

		protected SameContainerItemAndContainerSnapshotCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override string GetKeyForItem(ItemAndContainerSnapshot item)
		{
			return item.ItemID;
		}
	}
}
