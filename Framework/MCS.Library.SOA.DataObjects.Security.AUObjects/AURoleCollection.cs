using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public class AURoleCollection : SchemaObjectCollectionBase<AURole, AURoleCollection>
	{
		protected override AURoleCollection CreateFilterResultCollection()
		{
			return new AURoleCollection();
		}
	}

	public class AURoleKeyedCollection : SchemaObjectEditableKeyedCollectionBase<AURole, AURoleKeyedCollection>
	{
		protected override AURoleKeyedCollection CreateFilterResultCollection()
		{
			return new AURoleKeyedCollection();
		}

		protected override string GetKeyForItem(AURole item)
		{
			return item.ID;
		}
	}
}
