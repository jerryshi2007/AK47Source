using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace UDPCacheChat
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpNotifierCacheDependency dependency = new UdpNotifierCacheDependency();

            ObjectCacheQueue.Instance.Add("ClientData", string.Empty, dependency);

            object oldCacheData = ObjectCacheQueue.Instance["ClientData"];

            Console.Title = "UdpCacheDependency测试Chat";

            Console.WriteLine("Please input text...");

            Thread thread = new Thread((ThreadStart)MonitorThread);

            thread.Start();

            string cmd = Console.ReadLine();

            while (cmd.ToLower() != "exit")
            {
                if (cmd.IsNotEmpty())
                {
                    if (cmd.ToLower() == "send100")
                        SendOneHundredMessages();
                    else
                        SendOneMessage(cmd);
                }

                cmd = Console.ReadLine();
            }

            thread.Abort();
            thread.Join();
        }

        private static void SendOneHundredMessages()
        {
            for (int i = 0; i < 100; i++)
                SendOneMessage(string.Format("Test: {0}", i));
        }

        private static void SendOneMessage(string text)
        {
            CacheNotifyData data = new CacheNotifyData(typeof(ObjectCacheQueue), "ClientData", CacheNotifyType.Update);

            data.CacheData = text;

            UdpCacheNotifier.Instance.SendNotify(data);
        }

        private static void MonitorThread()
        {
            object oldCacheData = ObjectCacheQueue.Instance["ClientData"];

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
