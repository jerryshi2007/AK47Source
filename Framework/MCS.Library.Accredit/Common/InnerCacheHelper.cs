using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using MCS.Library.Caching;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.Common
{
	internal class InnerCacheHelper
	{
		private static readonly string cacheDependenceFile = Path.GetTempPath() + CommonResource.CacheDependenceFile;
		#region private CacheDependence
		private readonly static object syncFile = new object();
		public static DependencyBase PrepareDependency()
		{
			if (false == File.Exists(InnerCacheHelper.cacheDependenceFile))
			{
				lock (InnerCacheHelper.syncFile)
				{
					if (false == File.Exists(InnerCacheHelper.cacheDependenceFile))
						File.Create(InnerCacheHelper.cacheDependenceFile);
				}
			}

			TimeSpan ts = new TimeSpan(0, 1, 0);
			MixedDependency result = new MixedDependency(
				new FileCacheDependency(InnerCacheHelper.cacheDependenceFile),
				new SlidingTimeDependency(new TimeSpan(AccreditSection.GetConfig().AccreditSettings.CacheSlideMinutes * ts.Ticks)));

			return result;
		}
		#endregion

		#region RemoveAllCache
		public static void RemoveAllCache()
		{
			if (AccreditSection.GetConfig().AccreditSettings.ClearLocalCache)
			{
				if (false == File.Exists(InnerCacheHelper.cacheDependenceFile))
				{
					File.Create(InnerCacheHelper.cacheDependenceFile);
				}

				using (StreamWriter writer = new StreamWriter(InnerCacheHelper.cacheDependenceFile, true))
				{
					writer.WriteLine(Guid.NewGuid().ToString());
					writer.Flush();
				}
			}

			if (AccreditSection.GetConfig().AccreditSettings.ClearRemoteCache)
			{
				string[] cacheQueueType = {	"MCS.Library.OGUPermission.OguObjectIDCache, MCS.Library.OGUPermission", 
										  "MCS.Library.OGUPermission.OguObjectFullPathCache, MCS.Library.OGUPermission", 
										  "MCS.Library.OGUPermission.OguObjectLogOnNameCache, MCS.Library.OGUPermission" };

				CacheNotifyData[] data = new CacheNotifyData[cacheQueueType.Length];

				for (int i = 0; i < cacheQueueType.Length; i++)
				{
					data[i] = new CacheNotifyData();
					data[i].CacheQueueTypeDesp = cacheQueueType[i];
					data[i].NotifyType = CacheNotifyType.Clear;
				}

				UdpCacheNotifier.Instance.SendNotifyAsync(data);
                MmfCacheNotifier.Instance.SendNotify(data);
			}
		}
		#endregion

		internal static string BuildCacheKey(params object[] values)
		{
			StringBuilder builder = new StringBuilder(128);
			foreach (object obj in values)
			{
				if (obj is DateTime)
					builder.Append(((DateTime)obj).Ticks);
				else
					builder.Append(obj);
			}
#if DEBUG
			System.Diagnostics.Trace.WriteLine(builder.ToString(), "CacheKey");
#endif
			return builder.ToString();
		}
	}
}
