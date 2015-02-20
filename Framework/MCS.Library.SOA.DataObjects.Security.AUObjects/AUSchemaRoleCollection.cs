using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public class AUSchemaRoleCollection : SchemaObjectEditableKeyedCollectionBase<AUSchemaRole, AUSchemaRoleCollection>
	{
		protected override AUSchemaRoleCollection CreateFilterResultCollection()
		{
			return new AUSchemaRoleCollection();
		}

		protected override string GetKeyForItem(AUSchemaRole item)
		{
			return item.ID;
		}

		public AUSchemaRole GetRoleByCodeName(string codeName)
		{
			foreach (AUSchemaRole r in this)
			{
				if (r.CodeName == codeName)
					return r;
			}

			return null;
		}
	}
}
