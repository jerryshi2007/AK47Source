using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace UdpCacheTestServer
{
	class Program
	{
		static void Main(string[] args)
		{
			UdpNotifierCacheDependency dependency = new UdpNotifierCacheDependency();

			ObjectCacheQueue.Instance.Add("ClientData", string.Empty, dependency);

			object oldCacheData = ObjectCacheQueue.Instance["ClientData"];

			Console.Title = "UdpCacheDependency测试服务器端";

			Console.WriteLine("Monitoring cache data...");

			while (true)
			{
				Thread.Sleep(10);

				object cacheData;

				if (ObjectCacheQueue.Instance.TryGetValue("ClientData", out cacheData))
				{
					if (cacheData != oldCacheData)
					{
						oldCacheData = cacheData;

						Console.Write("Cahce data changed to ");
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine("'{0}'", cacheData);
						Console.ResetColor();
					}
				}
			}
		}
	}
}
