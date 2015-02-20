using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class ADReverseSyncLogDataSource : DataViewDataSourceQueryAdapterBase
	{
		public ADReverseSyncLogDataSource()
			: base("SC.ADReverseSynchronizeLog")
		{
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
