using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程定义管理器的基类
    /// </summary>
    public abstract class WfProcessDescriptorManagerBase : IWfProcessDescriptorManager
    {
        #region IWfProcessDescriptorManager Members

        public IWfProcessDescriptor LoadDescriptor(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");
            XElement xml = LoadXml(processKey);

            return WfProcessDescriptorManager.DeserializeXElementToProcessDescriptor(xml);
        }

        public IWfProcessDescriptor GetDescriptor(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            string cacheKey = NormalizeCacheKey(processKey);

            XElement processXml = WfProcessDescriptorXmlCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
            {
                XElement xml = LoadXml(processKey);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, xml, dependency);

                return xml;
            });

            return WfProcessDescriptorManager.DeserializeXElementToProcessDescriptor(processXml);
        }

        public void SaveDescriptor(IWfProcessDescriptor processDesp)
        {
            processDesp.NullCheck("processDesp");

            XElementFormatter formatter = WfProcessDescriptorManager.CreateFormatter();

            XElement xml = formatter.Serialize(processDesp);

            SaveXml(processDesp, xml);

            SendCacheNotifyByProcessKey(processDesp.Key);
        }

        public void DeleteDescriptor(string processKey)
        {
            processKey.CheckStringIsNullOrEmpty("processKey");

            DeleteXml(processKey);

            SendCacheNotifyByProcessKey(processKey);
        }

        public void ClearAll()
        {
            this.ClearAllXml();

            CacheNotifyData notifyData = new CacheNotifyData(typeof(WfProcessDescriptorXmlCache), null, CacheNotifyType.Clear);

            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        /// <summary>
        /// 流程的描述是否重复
        /// </summary>
        /// <param name="processKey"></param>
        public abstract bool ExsitsProcessKey(string processKey);

        #endregion

        #region Protected
        /// <summary>
        /// 根据Key加载Xml
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        protected abstract XElement LoadXml(string processKey);

        /// <summary>
        /// 根据Key保存Xml
        /// </summary>
        /// <param name="processDesp">流程描述</param>
        /// <param name="xml"></param>
        protected abstract void SaveXml(IWfProcessDescriptor processDesp, XElement xml);

        /// <summary>
        /// 根据Key删除流程
        /// </summary>
        /// <param name="processKey"></param>
        protected abstract void DeleteXml(string processKey);

        /// <summary>
        /// 清除所有流程
        /// </summary>
        protected abstract void ClearAllXml();

        #endregion Protected

        private static void SendCacheNotifyByProcessKey(string processKey)
        {
            string cacheKey = NormalizeCacheKey(processKey);

            CacheNotifyData notifyData = new CacheNotifyData(typeof(WfProcessDescriptorXmlCache), cacheKey, CacheNotifyType.Invalid);

            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        /// <summary>
        /// 根据租户和流程描述的Key计算Cache的Key
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        private static string NormalizeCacheKey(string processKey)
        {
            string result = processKey.ToLower();

            if (TenantContext.Current.Enabled)
                result = string.Format("{0}-{1}", result, TenantContext.Current.TenantCode.ToLower());

            return result;
        }
    }
}
