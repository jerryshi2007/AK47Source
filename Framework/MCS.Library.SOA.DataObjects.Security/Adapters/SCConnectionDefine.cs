using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	internal static class SCConnectionDefine
	{
		public static readonly DateTime MaxVersionEndTime = new DateTime(9999, 9, 9);

		public static string DBConnectionName
		{
			get
			{
				return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
			}
		}

		//public static string DefaultAccreditConnectionName
		//{
		//    get
		//    {
		//        return ConnectionNameMappingSettings.GetConfig().GetConnectionName("AccreditAdmin", "AccreditAdmin");
		//    }
		//}
	}
}
