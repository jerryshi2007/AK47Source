using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace MCS.Library.Core
{
    /// <summary>
    /// 命中类型的性能监视指针们的基类，常用于Cache的性能监控。
    /// 里面包含了一组指针
    /// </summary>
    public abstract class PerformanceCountersWrapperBase
    {
        private Dictionary<string, PerformanceCounterWrapper> counters = new Dictionary<string, PerformanceCounterWrapper>();

        /// <summary>
        /// 构造方法
        /// </summary>
        public PerformanceCountersWrapperBase()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="instanceName">实例名称</param>
        public PerformanceCountersWrapperBase(string instanceName)
        {
            InitCountersFromAttribute(instanceName);
        }

        /// <summary>
        /// 构造方法，从Installer类构造
        /// </summary>
        /// <param name="installer"></param>
        /// <param name="instanceName"></param>
        public PerformanceCountersWrapperBase(PerformanceCounterInstaller installer, string instanceName)
        {
            InitCountersFromInstaller(installer, instanceName);
        }

        /// <summary>
        /// 根据环境信息得到实例的名称
        /// </summary>
        /// <returns></returns>
        public static string GetInstanceName()
        {
            string instanceName = string.Empty;

            if (EnvironmentHelper.IsUsingWebConfig)
                instanceName = HostingEnvironment.SiteName + "/" + HostingEnvironment.ApplicationVirtualPath;
            else
                instanceName = AppDomain.CurrentDomain.FriendlyName;

            return instanceName.Replace('/', '_');
        }

        /// <summary>
        /// 得到计数器
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        protected PerformanceCounterWrapper GetCounter(string counterName)
        {
            counterName.CheckStringIsNullOrEmpty("counterName");

            PerformanceCounterWrapper result = null;

            if (this.counters.TryGetValue(counterName, out result) == false)
                throw new SystemSupportException(string.Format("不能在PerformanceCountersWrapperBase中找到名称为{0}的性能计数器", counterName));

            return result;
        }

        private void InitCountersFromInstaller(PerformanceCounterInstaller installer, string instanceName)
        {
            installer.NullCheck("installer");

            PerformanceCounterInitData initData = new PerformanceCounterInitData(installer.CategoryName, string.Empty, instanceName);

            foreach (CounterCreationData counter in installer.Counters)
            {
                initData.CounterName = counter.CounterName;
                this.counters[initData.CounterName] = new PerformanceCounterWrapper(initData);
            }
        }

        private void InitCountersFromAttribute(string instanceName)
        {
            PerformanceCounterDescriptionAttribute attribute = AttributeHelper.GetCustomAttribute<PerformanceCounterDescriptionAttribute>(this.GetType());

            if (attribute != null && attribute.CategoryName.IsNotEmpty() && attribute.CounterNames.IsNotEmpty())
            {
                PerformanceCounterInitData initData = new PerformanceCounterInitData(attribute.CategoryName, string.Empty, instanceName);

                string[] counterNames = attribute.CounterNames.Split(',');

                foreach (string counterName in counterNames)
                {
                    initData.CounterName = counterName;

                    this.counters[counterName] = new PerformanceCounterWrapper(initData);
                }
            }
        }
    }
}
