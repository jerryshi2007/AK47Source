using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Logging;

namespace MCS.Library.Caching
{
    /// <summary>
    /// ͨ��Udp��ʽ������Ϣ����Classʹ���˵���ģʽ��ֱ��ʹ��UdpCacheNotifier.Instance
    /// </summary>
    public sealed class UdpCacheNotifier
    {
        internal static CacheNotifierPerformanceCounters TotalCounters = null;
        internal static CacheNotifierPerformanceCounters AppInstanceCounters = null;

        static UdpCacheNotifier()
        {
            TotalCounters = new CacheNotifierPerformanceCounters("_Total_");
            AppInstanceCounters = new CacheNotifierPerformanceCounters(PerformanceCountersWrapperBase.GetInstanceName());
        }

        private class SendDataWrapper
        {
            public List<byte[]> DataList;
            public UdpCacheNotifierTargetCollection Targets;
            public CacheNotifyData[] OriginalNotifyData;

            public SendDataWrapper(CacheNotifyData[] originalNotifyData, List<byte[]> dl, UdpCacheNotifierTargetCollection t)
            {
                OriginalNotifyData = originalNotifyData;
                DataList = dl;
                Targets = t;
            }
        }

        /// <summary>
        /// �ڲ�������ʵ����󣬵���ģʽ�ı��ַ�ʽ
        /// </summary>
        public static readonly UdpCacheNotifier Instance = new UdpCacheNotifier();

        private UdpCacheNotifier()
        {
        }

        /// <summary>
        /// ����Cache֪ͨ
        /// </summary>
        /// <param name="notifyDataArray">Cache֪ͨ����</param>
        public void SendNotify(params CacheNotifyData[] notifyDataArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(notifyDataArray != null, "notifyDataArray");

            UdpCacheNotifierSettings settings = UdpCacheNotifierSettings.GetConfig();

            List<byte[]> dataList = GetSerializedCacheData(notifyDataArray);

            SendSerializedData(notifyDataArray, dataList, settings.EndPoints);
        }

        /// <summary>
        /// �첽����Cache֪ͨ
        /// </summary>
        /// <param name="notifyDataArray">Cache֪ͨ����</param>
        public void SendNotifyAsync(params CacheNotifyData[] notifyDataArray)
        {
            SendDataWrapper sdWrapper = new SendDataWrapper(
                notifyDataArray,
                GetSerializedCacheData(notifyDataArray),
                UdpCacheNotifierSettings.GetConfig().EndPoints);

            ThreadPool.QueueUserWorkItem(new WaitCallback(SendNotifyThreadCallBack), sdWrapper);
        }

        private static void SendNotifyThreadCallBack(object context)
        {
            SendDataWrapper sdWrapper = (SendDataWrapper)context;

            SendSerializedData(sdWrapper.OriginalNotifyData, sdWrapper.DataList, sdWrapper.Targets);
        }

        private static void SendSerializedData(CacheNotifyData[] originalNotifyData, List<byte[]> serializedData, UdpCacheNotifierTargetCollection targets)
        {
            Logger logger = null;

            try
            {
                logger = LoggerFactory.Create("UdpCache");
            }
            catch (SystemSupportException)
            {
            }

            for (int i = 0; i < originalNotifyData.Length; i++)
            {
                byte[] data = serializedData[i];
                CacheNotifyData notifyData = originalNotifyData[i];

                foreach (UdpCacheNotifierTarget endPoint in targets)
                {
                    foreach (int port in endPoint.GetPorts())
                    {
                        using (UdpClient udp = new UdpClient())
                        {
                            IPEndPoint remoteEndPoint = new IPEndPoint(endPoint.Address, port);

                            udp.Connect(remoteEndPoint);

                            udp.Send(data, data.Length);
                        }
                    }

                    if (logger != null)
                        logger.Write(CreateLogEntity(notifyData, endPoint));
                }

                UdpCacheNotifier.TotalCounters.UdpSentItemsCounter.Increment();
                UdpCacheNotifier.TotalCounters.UdpSentCountPerSecond.Increment();

                UdpCacheNotifier.AppInstanceCounters.UdpSentItemsCounter.Increment();
                UdpCacheNotifier.AppInstanceCounters.UdpSentCountPerSecond.Increment();
            }
        }

        private static LogEntity CreateLogEntity(CacheNotifyData notifyData, UdpCacheNotifierTarget endPoint)
        {
            string message = string.Format("{0}, {1}",
                    notifyData.ToString(),
                    endPoint.ToString()
                );

            LogEntity logEntity = new LogEntity(message);

            logEntity.EventID = 7001;
            logEntity.LogEventType = TraceEventType.Information;
            logEntity.Source = "UdpCache";
            logEntity.Title = "����UdpCache";
            logEntity.Priority = LogPriority.Normal;

            return logEntity;
        }

        private static List<byte[]> GetSerializedCacheData(CacheNotifyData[] notifyDataArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(notifyDataArray != null, "notifyDataArray");

            UdpCacheNotifierSettings settings = UdpCacheNotifierSettings.GetConfig();

            List<byte[]> dataList = new List<byte[]>();

            foreach (CacheNotifyData notifyData in notifyDataArray)
            {
                byte[] data = notifyData.ToBytes();

                ExceptionHelper.FalseThrow(data.Length <= settings.PackageSize,
                    "Cache Key{0}��֪ͨ���ݰ���СΪ{1}�ֽ�, ����С�ڵ���{2}�ֽڣ����Ե���udpCacheNotifierSettings���ýڵ�packageSize���������������",
                    notifyData.CacheKey, data.Length, settings.PackageSize);

                dataList.Add(data);
            }

            return dataList;
        }
    }
}
