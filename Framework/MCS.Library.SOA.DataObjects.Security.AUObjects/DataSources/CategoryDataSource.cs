using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class CategoryDataSource
	{
		public static IEnumerable<AUSchemaCategory> GetCategories()
		{
			return Adapters.SchemaCategoryAdapter.Instance.LoadSubCategories(null);
		}
	}
}
