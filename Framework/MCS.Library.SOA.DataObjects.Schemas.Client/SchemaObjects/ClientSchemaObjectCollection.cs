using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientSchemaObjectCollection : EditableKeyedDataObjectCollectionBase<string, ClientSchemaObjectBase>
	{
		/// <summary>
		/// 初始化<see cref="ClientSchemaObjectCollection"/>的新实例。
		/// </summary>
		public ClientSchemaObjectCollection()
		{
		}

		public ClientSchemaObjectCollection(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// 使用一组<see cref="ClientSCBase"/>初始化<see cref="ClientSCBaseCollection"/>的新实例。
		/// </summary>
		/// <param name="clientObjects"></param>
		public ClientSchemaObjectCollection(IEnumerable<ClientSchemaObjectBase> clientObjects)
		{
			clientObjects.NullCheck("clientObjects");

			clientObjects.ForEach(obj => this.Add(obj));
		}

		protected override string GetKeyForItem(ClientSchemaObjectBase item)
		{
			return item.ID;
		}
	}
}
