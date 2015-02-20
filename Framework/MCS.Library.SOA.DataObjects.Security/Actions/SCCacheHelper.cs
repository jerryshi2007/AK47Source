using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	/// <summary>
	/// 权限中心的缓存相关的辅助代码
	/// </summary>
	public static class SCCacheHelper
	{
		public static void InvalidateAllCache()
		{
			string[] cacheQueueType = {
										"MCS.Library.OGUPermission.OguObjectIDCache, MCS.Library.OGUPermission", 
										"MCS.Library.OGUPermission.OguObjectFullPathCache, MCS.Library.OGUPermission", 
										"MCS.Library.OGUPermission.OguObjectLogOnNameCache, MCS.Library.OGUPermission",
										"PermissionCenter.Extensions.PCServiceMethodCache, PermissionCenterServices"
									  };

			CacheNotifyData[] data = new CacheNotifyData[cacheQueueType.Length];

			for (int i = 0; i < cacheQueueType.Length; i++)
			{
				data[i] = new CacheNotifyData();
				data[i].CacheQueueTypeDesp = cacheQueueType[i];
				data[i].NotifyType = CacheNotifyType.Clear;
			}

			UdpCacheNotifier.Instance.SendNotify(data);
            MmfCacheNotifier.Instance.SendNotify(data);
		}
	}
}
