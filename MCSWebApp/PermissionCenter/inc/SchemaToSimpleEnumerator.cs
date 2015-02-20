using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class SchemaToSimpleEnumerator : IEnumerable<SCSimpleObject>
	{
		private SchemaObjectCollection dataItems;

		public SchemaToSimpleEnumerator(SchemaObjectCollection dataItems)
		{
			// dataItems.NullCheck<ArgumentNullException>("");
			if (dataItems == null)
				throw new ArgumentNullException("dataItems");
			this.dataItems = dataItems;
		}

		public IEnumerator<SCSimpleObject> GetEnumerator()
		{
			foreach (var item in this.dataItems)
			{
				yield return item.ToSimpleObject();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var item in this.dataItems)
			{
				yield return item.ToSimpleObject();
			}
		}
	}
}