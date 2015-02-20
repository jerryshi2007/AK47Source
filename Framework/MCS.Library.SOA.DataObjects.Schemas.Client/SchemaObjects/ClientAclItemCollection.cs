using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientAclItemCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ClientAclItem>
	{
		public ClientAclItemCollection()
		{
		}

		public ClientAclItemCollection(IEnumerable<ClientAclItem> items)
		{
			foreach (var item in items)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// 合并需要变更的AclMember集合。假设当前集合是新集合，提供原始的集合，比较出哪些是需要变更的。
		/// </summary>
		/// <param name="original"></param>
		/// <returns>是否有变化产生</returns>
		public bool MergeChangedItems(ClientAclItemCollection original)
		{
			original.NullCheck("original");

			bool result = false;

			foreach (ClientAclItem item in this)
			{
				//确认新集合每一项在老集合中是否存在
				if (original.ContainsKey(item.ContainerPermission, item.MemberID) == false)
					result = true;
			}

			foreach (ClientAclItem item in original)
			{
				//确认老集合每一项在新集合中是否存在，如果不存在，则添加删除的记录
				if (this.ContainsKey(item.ContainerPermission, item.MemberID) == false)
				{
					item.Status = ClientSchemaObjectStatus.Deleted;
					this.Add(item);
					result = true;
				}
			}

			return result;
		}

		public void Add(string containerPermission, ClientSchemaObjectBase member)
		{
			this.Add(new ClientAclItem(containerPermission, member));
		}

		protected override string GetKeyForItem(ClientAclItem item)
		{
			return item.ContainerPermission + "|" + item.MemberID;
		}

		public ClientAclItem this[string permission, string memberID]
		{
			get { return this[permission + "|" + memberID]; }
		}

		private new ClientAclItem this[string key]
		{
			get { return base[key]; }
		}

		public bool ContainsKey(string permission, string memberID)
		{
			return base.ContainsKey(permission + "|" + memberID);
		}

		public void ContainsKey(string permission, string memberID, Action<ClientAclItem> action)
		{
			base.ContainsKey(permission + "|" + memberID, action);
		}

		private new bool ContainsKey(string key)
		{
			return base.ContainsKey(key);
		}

		private new void ContainsKey(string key, Action<ClientAclItem> action)
		{
			base.ContainsKey(key, action);
		}
	}
}
