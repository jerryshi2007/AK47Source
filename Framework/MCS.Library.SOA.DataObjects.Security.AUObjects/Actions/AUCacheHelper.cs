using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	internal static class AUCacheHelper
	{
		public static void InvalidateAllCache()
		{
			//TODO:实现缓存通知机制
			string[] cacheQueueType = {
										"MCS.Library.OGUPermission.OguObjectIDCache, MCS.Library.OGUPermission", 
										"MCS.Library.OGUPermission.OguObjectFullPathCache, MCS.Library.OGUPermission", 
										"MCS.Library.OGUPermission.OguObjectLogOnNameCache, MCS.Library.OGUPermission",
										"AUCenterServices.Services.AUServiceMethodCache, AUCenterServices"
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
