using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace UdpCacheTestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "UdpCacheDependency测试客户端";

			Console.WriteLine("Please input cache data...");

			string cmd = Console.ReadLine();

			while (cmd.ToLower() != "exit")
			{
				if (string.IsNullOrEmpty(cmd) == false)
				{
					CacheNotifyData data = new CacheNotifyData(typeof(ObjectCacheQueue), "ClientData", CacheNotifyType.Update);

					data.CacheData = cmd;

					UdpCacheNotifier.Instance.SendNotify(data);
				}

				cmd = Console.ReadLine();
			}
		}
	}
}
