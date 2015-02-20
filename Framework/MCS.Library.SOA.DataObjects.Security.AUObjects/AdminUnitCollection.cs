using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public class AdminUnitCollection : SchemaObjectEditableKeyedCollectionBase<AdminUnit, AdminUnitCollection>
	{
		protected override AdminUnitCollection CreateFilterResultCollection()
		{
			return new AdminUnitCollection();
		}

		protected override string GetKeyForItem(AdminUnit item)
		{
			return item.ID;
		}
	}
}
