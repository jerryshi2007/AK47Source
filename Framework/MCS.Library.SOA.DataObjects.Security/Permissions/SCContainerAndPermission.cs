using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Security.Debugger;

namespace MCS.Library.SOA.DataObjects.Security.Permissions
{
	/// <summary>
	/// 表示容器和权限的对象。用于根据MemberID查询返回的结果
	/// </summary>
	[Serializable]
	public struct SCContainerAndPermission
	{
		/// <summary>
		/// 根据ContainerID和Permission构造SCContainerAndPermission
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="permission"></param>
		/// <returns></returns>
		public static SCContainerAndPermission Construct(string containerID, string containerPermission)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");
			containerPermission.CheckStringIsNullOrEmpty("containerPermission");

			SCContainerAndPermission result = new SCContainerAndPermission();

			result.ContainerID = containerID;
			result.ContainerPermission = containerPermission;

			return result;
		}

		[ORFieldMapping("ContainerID")]
		public string ContainerID
		{
			get;
			set;
		}

		[ORFieldMapping("ContainerPermission")]
		public string ContainerPermission
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 表示容器和权限的对象集合。用于根据MemberID查询返回的结果
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(SCContainerAndPermissionCollection.DictionaryDebugView))]
	public class SCContainerAndPermissionCollection : SerializableEditableKeyedDataObjectCollectionBase<SCContainerAndPermission, SCContainerAndPermission>
	{
		public SCContainerAndPermissionCollection()
		{
		}

		protected SCContainerAndPermissionCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		/// <summary>
		/// 判断指定的ContaienrID和Permission组合是否存在
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="permission"></param>
		/// <returns></returns>
		public bool ContainsKey(string containerID, string containerPermission)
		{
			return base.ContainsKey(SCContainerAndPermission.Construct(containerID, containerPermission));
		}

		protected override SCContainerAndPermission GetKeyForItem(SCContainerAndPermission item)
		{
			return item;
		}

		class DictionaryDebugView
		{
			private SCContainerAndPermissionCollection collection = null;

			public DictionaryDebugView(SCContainerAndPermissionCollection collection)
			{
				this.collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public ListKeyAndValue[] Objects
			{
				get
				{
					ListKeyAndValue[] keys = new ListKeyAndValue[this.collection.Count];

					int i = 0;
					foreach (SCContainerAndPermission obj in collection)
					{
						keys[i] = new ListKeyAndValue(this.collection, obj.ContainerID, obj.ContainerPermission);
						i++;
					}

					return keys;
				}
			}


		}
	}
}
