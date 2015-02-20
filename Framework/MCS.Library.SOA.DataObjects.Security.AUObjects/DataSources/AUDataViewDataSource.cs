using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public abstract class AUDataViewDataSource : DataViewDataSourceQueryAdapterBase
	{
		protected override string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}
	}
}
