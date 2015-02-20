using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MCS.Library.Core;
using System.Diagnostics;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Udp消息通知数据获取器
    /// </summary>
    public sealed class UdpCacheNotifyFetcher : CacheNotifyFetcherBase
    {
        private UdpClient udp = null;
        private IPEndPoint bindedEndPoint = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            this.udp = new UdpClient();
            this.bindedEndPoint = BindToEndPoint(udp.Client);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public override CacheNotifyData[] GetData()
        {
            CacheNotifyData[] result = CacheNotifyData.EmptyData;

            try
            {
                byte[] recBuffer = udp.Receive(ref bindedEndPoint);
                CacheNotifyData data = CacheNotifyData.FromBuffer(recBuffer);
                result = new CacheNotifyData[] { data };

                UdpCacheNotifier.TotalCounters.UdpReceivedItemsCounter.Increment();
                UdpCacheNotifier.TotalCounters.UdpReceivedCountPerSecond.Increment();

                UdpCacheNotifier.AppInstanceCounters.UdpReceivedItemsCounter.Increment();
                UdpCacheNotifier.AppInstanceCounters.UdpReceivedCountPerSecond.Increment();

				CacheNotifierSettings settings = CacheNotifierSettings.GetConfig();

				if (settings.ForwardUdpToMmf && settings.EnableMmfNotifier)
                {
                    CacheNotifyDataMap.WriteNotExistCacheNotifyData(DateTime.Now.Ticks, result);

                    UdpCacheNotifier.TotalCounters.ForwardedUdpToMmfItems.Increment();
                    UdpCacheNotifier.TotalCounters.ForwardUdpToMmfCountPerSecond.Increment();

                    UdpCacheNotifier.AppInstanceCounters.ForwardedUdpToMmfItems.Increment();
                    UdpCacheNotifier.AppInstanceCounters.ForwardUdpToMmfCountPerSecond.Increment();
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.TimedOut)
                    throw;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.udp != null)
                ((IDisposable)this.udp).Dispose();
        }

        private static IPEndPoint BindToEndPoint(Socket socket)
        {
            IPEndPoint bindEndPoint = null;

            UdpCacheClientSettings settings = UdpCacheClientSettings.GetConfig();

            int[] ports = settings.GetPorts();

            for (int i = 0; i < ports.Length; i++)
            {
                try
                {
                    bindEndPoint = new IPEndPoint(settings.Address, ports[i]);
                    socket.Bind(bindEndPoint);
                    socket.ReceiveTimeout = 1000;

                    break;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.AddressAlreadyInUse)
                        throw;
                }
            }

            ExceptionHelper.FalseThrow(socket.IsBound, "Cache监听线程不能找到可以监听的端口");

            return bindEndPoint;
        }
    }
}
