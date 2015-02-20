using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class DataFieldDefineCollection : EditableKeyedDataObjectCollectionBase<string, DataFieldDefine>
	{
		protected override string GetKeyForItem(DataFieldDefine item)
		{
			return item.Name;
		}
	}
}
