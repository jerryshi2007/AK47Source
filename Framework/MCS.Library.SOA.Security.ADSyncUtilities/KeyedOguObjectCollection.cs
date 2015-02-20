using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class KeyedOguObjectCollection : EditableKeyedDataObjectCollectionBase<string, IOguObject>
	{
		protected override string GetKeyForItem(IOguObject item)
		{
			return item.ID;
		}
	}
}
