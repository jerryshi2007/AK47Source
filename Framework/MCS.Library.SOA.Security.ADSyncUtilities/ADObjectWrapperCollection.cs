using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	internal class ADObjectWrapperCollection : EditableKeyedDataObjectCollectionBase<string, ADObjectWrapper>
	{
		protected override string GetKeyForItem(ADObjectWrapper item)
		{
			return item.NativeGuid;
		}
	}
}
