using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Adapters
{
	public class ADReverseSynchronizeLogAdapter : UpdatableAndLoadableAdapterBase<ADReverseSynchronizeLog, ADReverseSynchronizeLogCollection>
	{
		public static readonly ADReverseSynchronizeLogAdapter Instance = new ADReverseSynchronizeLogAdapter();

		private ADReverseSynchronizeLogAdapter()
		{
		}

		protected override string GetConnectionName()
		{
			return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
		}
	}
}
