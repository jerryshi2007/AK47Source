using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	public class ObjectHistoryDataSource : MCS.Library.SOA.DataObjects.Security.DataSources.ObjectHistoryDataSource
	{
		protected override string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}
	}
}
