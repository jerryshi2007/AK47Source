using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test
{
	[Serializable]
	public class RoleTable : 
		TableBase<SOARolePropertyRow, SOARolePropertyValue, SOARolePropertyValueCollection, SOARolePropertyRowCollection, string, SOARolePropertyDefinition, SOARolePropertyDefinitionCollection>
	{
	}
}
