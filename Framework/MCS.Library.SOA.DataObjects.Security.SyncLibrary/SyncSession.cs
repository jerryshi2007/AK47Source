using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Configuration;
using System.Runtime;
using MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration;
using System.Collections;
using System.Configuration.Provider;
using MCS.Library.Configuration;
using System.Collections.ObjectModel;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    /// <summary>
    /// 同步任务
    /// </summary>
    public sealed class SyncSession
    {
        private PropertyMappingCollection mappings;
        private IList<LogProviderBase> loggers;
        private PropertySetterCollection setters;
        private PropertyComparerCollection comparers;
        private string planName;
        private int batchSize;

        private DataProviderBase provider;
        private static readonly string[] EmptyArray = new string[0];
        private int numOfUpdated;
        private int numOfErrors;

        class WrappedNameValueCollection : NameValueCollection
        {
            public WrappedNameValueCollection()
            {
                this.SetReadOnly(true);
            }

            public WrappedNameValueCollection(NameValueCollection other)
                : base(other)
            {
                this.SetReadOnly(true);
            }

            internal void SetReadOnly(bool readOnly)
            {
                this.IsReadOnly = readOnly;
            }
        }


        private SyncSession(string planName, DataProviderBase provider, PropertyComparerCollection comparers, PropertySetterCollection setters, IList<LogProviderBase> loggers, PropertyMappingCollection mappings)
        {
            this.planName = planName;
            this.provider = provider;
            this.comparers = comparers;
            this.setters = setters;
            this.loggers = loggers;
            this.mappings = mappings;
            this.batchSize = 1;
        }


        #region 创建 方法

        /// <summary>
        /// 根据配置初始化一个<see cref="SyncSession"/>
        /// </summary>
        /// <param name="planName">配置文件中的计划名称</param>
        /// <returns></returns>
        public static SyncSession CreateSession(string planName)
        {
            Configuration.PermissionCenterIncomeSyncSettings config = Configuration.PermissionCenterIncomeSyncSettings.GetConfig();
            var planConfig = config.SyncPlans[planName];
            if (planConfig == null)
                throw new ArgumentException("未找到同步方案 " + planName, "providerName");

            if (planConfig.DataProvider == null)
                throw new System.Configuration.ConfigurationErrorsException("配置文件中，未配置dataProvider");

            DataProviderBase provider = (DataProviderBase)CreateDataProvider(planConfig.DataProvider);

            return CreateSession(planName, provider);
        }

        /// <summary>
        /// 根据配置和指定的数据源初始化<see cref="SyncSession"/>。
        /// </summary>
        /// <param name="planName">配置文件中的计划名称</param>
        /// <param name="dataProvider"><see cref="DataProviderBase"/>的派生类型的对象，提供源数据。</param>
        /// <returns></returns>
        public static SyncSession CreateSession(string planName, DataProviderBase dataProvider)
        {
            Configuration.PermissionCenterIncomeSyncSettings config = Configuration.PermissionCenterIncomeSyncSettings.GetConfig();

            var planConfig = config.SyncPlans[planName];
            if (planConfig == null)
                throw new ArgumentException("未找到同步方案 " + planName, "providerName");

            HashSet<string> comparerNames = new HashSet<string>();
            HashSet<string> setterNames = new HashSet<string>();
            PropertyMappingCollection mappings = new PropertyMappingCollection();

            mappings.SourceKeyProperty = planConfig.SourceKeyProperty;

            foreach (PropertyMappingConfigurationElement item in planConfig.PropertyMappings)
            {
                comparerNames.Add(item.ComparerName);
                setterNames.Add(item.SetterName); //需要哪些比较器和设置器
                mappings.Add(item.SourceProperty, new PropertyMapping(item.SourceProperty, item.TargetProperty, item.ComparerName, item.SetterName, new WrappedNameValueCollection(item.Parameters)));
            }

            var comparers = CreateComparers(comparerNames, config.Comparers);
            var setters = CreateSetters(setterNames, config.PropertySetters);

            IList<LogProviderBase> loggers = CreateLoggers(planConfig);

            SyncSession task = new SyncSession(planName, dataProvider, comparers, setters, loggers, mappings);
            task.batchSize = planConfig.BatchSize;

            return task;
        }

        private static IList<LogProviderBase> CreateLoggers(SyncPlanConfigurationElement planConfig)
        {
            IList<LogProviderBase> loggers = new List<LogProviderBase>();

            if (planConfig.Loggers != null)
            {
                foreach (LoggerConfigurationElement item in planConfig.Loggers)
                {
                    loggers.Add((LogProviderBase)CreateProvider(item.Name, item.Type, item.Parameters));
                }
            }

            return loggers;
        }

        private static PropertySetterCollection CreateSetters(HashSet<string> setterNames, ComparerConfigurationElementCollection setterConfigSet)
        {
            var setters = new PropertySetterCollection();

            foreach (string name in setterNames)
            {
                var itemConfig = setterConfigSet[name];
                if (itemConfig == null)
                    throw new ConfigurationErrorsException(string.Format("没有找到名为 {0} 的设置器", name));

                setters.Add(name, CreateInstance<IPropertySetter>(itemConfig.Type));
            }

            return setters;
        }

        private static PropertyComparerCollection CreateComparers(HashSet<string> comparerNames, ComparerConfigurationElementCollection comparerConfigSet)
        {
            var comparers = new PropertyComparerCollection();

            foreach (string name in comparerNames)
            {
                var itemConfig = comparerConfigSet[name];
                if (itemConfig == null)
                    throw new ConfigurationErrorsException(string.Format("没有找到名为 {0} 的比较器", name));

                comparers.Add(name, CreateInstance<IPropertyComparer>(itemConfig.Type));
            }

            return comparers;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private static ProviderBase CreateDataProvider(Configuration.DataProviderConfigurationElement config)
        {
            return CreateProvider(config.Name, config.Type, config.Parameters);
        }

        private static T CreateInstance<T>(string typeName) where T : class
        {
            Type type = Type.GetType(typeName);
            if (type == null)
                throw new ConfigurationErrorsException(string.Format("无法加载类型 {0}", typeName));

            T provider = SecureUtil.SecureCreateInstance(type, null, true) as T;
            if (provider == null)
                throw new ConfigurationErrorsException(string.Format("未能创建类型 {0}", typeName));

            return provider;
        }

        private static ProviderBase CreateProvider(string name, string providerTypeName, NameValueCollection parameters)
        {
            Type type = Type.GetType(providerTypeName);
            if (type == null)
                throw new ConfigurationErrorsException(string.Format("无法加载提供程序类型 {0}", providerTypeName));


            ProviderBase provider = SecureUtil.SecureCreateInstance(type, null, true) as ProviderBase;
            if (provider == null)
                throw new ConfigurationErrorsException(string.Format("未能创建提供程序类型 {0}", providerTypeName));

            NameValueCollection copy = new NameValueCollection(parameters);

            provider.Initialize(name, copy);

            return provider;
        }

        #endregion

        #region 属性集

        /// <summary>
        /// 获取方案的名称
        /// </summary>
        public string PlanName
        {
            get { return planName; }
        }

        /// <summary>
        /// 获取或设置已更新的数据个数
        /// </summary>
        public int NumerOfUpdated
        {
            get { return numOfUpdated; }
            set { numOfUpdated = value; }
        }

        /// <summary>
        /// 获取或设置更新出错的个数
        /// </summary>
        public int NumerOfErrors
        {
            get { return numOfErrors; }
            set { numOfErrors = value; }
        }

        /// <summary>
        /// 批次大小
        /// </summary>
        public int BatchSize
        {
            get { return batchSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                batchSize = value;
            }
        }


        /// <summary>
        /// 获取映射的集合
        /// </summary>
        public PropertyMappingCollection Mappings
        {
            get { return mappings; }
        }

        /// <summary>
        /// 获取一个<see cref="IList~1"/>集合，表示日志记录器的集合
        /// </summary>
        public IList<LogProviderBase> Loggers
        {
            get
            {
                return this.loggers;
            }
        }

        /// <summary>
        /// 获取一个<see cref="PropertyComparerCollection"/>，表示比较器的集合
        /// </summary>
        public PropertyComparerCollection Comparers
        {
            get
            {
                return this.comparers;
            }
        }

        /// <summary>
        /// 获取一个<see cref="PropertySetterCollection"/>，表示属性转换器的集合
        /// </summary>
        public PropertySetterCollection Setters
        {
            get
            {
                return this.setters;
            }
        }

        /// <summary>
        /// 获取数据源提供程序
        /// </summary>
        public DataProviderBase SourceProvider
        {
            get
            {
                return this.provider;
            }
        }

        #endregion

        internal void WriteUpdateLog(SchemaObjectBase scObj)
        {
            if (this.Loggers != null)
            {
                foreach (var item in this.Loggers)
                {
                    try
                    {
                        item.WriteLog(this, scObj);
                    }
                    catch
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Log(1, "日志", "记录日志时发生错误。");
                        }
                    }
                }
            }
        }

        internal void WriteErrorLog(SchemaObjectBase scObj, Exception ex)
        {
            if (this.Loggers != null)
            {
                foreach (var item in this.Loggers)
                {
                    try
                    {
                        item.WriteErrorLog(this, scObj, ex);
                    }
                    catch
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Log(1, "日志", "记录日志时发生错误。");
                        }
                    }
                }
            }
        }

        internal void WriteStartLog(SyncSession session)
        {
            if (this.Loggers != null)
            {
                foreach (var item in Loggers)
                {
                    try
                    {
                        item.WriteStartLog(session);
                    }
                    catch
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Log(1, "日志", "记录日志时发生错误。");
                        }

                    }
                }
            }
        }

        internal void WriteEndLog(SyncSession session, bool success)
        {
            if (this.Loggers != null)
            {
                foreach (var item in Loggers)
                {
                    try
                    {
                        item.WriteEndLog(session, success);
                    }
                    catch
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Log(1, "日志", "记录日志时发生错误。");
                        }
                    }
                }
            }
        }
    }
}
