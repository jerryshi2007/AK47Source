using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
	[Serializable]
	public class ClientPropertyDefineCollection : EditableKeyedDataObjectCollectionBase<string, ClientPropertyDefine>
	{
		protected override string GetKeyForItem(ClientPropertyDefine item)
		{
			return item.Name;
		}
	}
}
