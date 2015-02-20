using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class SCGenericObject : SchemaObjectBase
	{
		public SCGenericObject(string schemaTypeString) :
			base(schemaTypeString)
		{
		}
	}
}
