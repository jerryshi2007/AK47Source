using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	public class SchemaLogDataSource : MCS.Library.SOA.DataObjects.Security.DataSources.SchemaLogDataSource
	{
		protected override string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}
	}
}
