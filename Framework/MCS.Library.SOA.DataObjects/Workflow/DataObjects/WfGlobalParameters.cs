using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    [TenantRelativeObject]
    public class WfGlobalParameters
    {
        private PropertyValueCollection _Properties = null;
        private WfNetworkCredentialCollection _Credentials = null;
        private WfServiceAddressDefinitionCollection _ServiceAddressDefs = null;

        private WfGlobalParameters()
        {
        }

        /// <summary>
        /// 得到默认的全局参数
        /// </summary>
        public static WfGlobalParameters Default
        {
            get
            {
                return GetProperties("Default");
            }
        }

        public string Key
        {
            get;
            internal set;
        }

        /// <summary>
        /// 类别所对应的属性
        /// </summary>
        public PropertyValueCollection Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new PropertyValueCollection();

                return this._Properties;
            }
        }

        public WfNetworkCredentialCollection Credentials
        {
            get
            {
                if (this._Credentials == null)
                    this._Credentials = new WfNetworkCredentialCollection();

                return this._Credentials;
            }
        }

        public WfServiceAddressDefinitionCollection ServiceAddressDefs
        {
            get
            {
                if (this._ServiceAddressDefs == null)
                    this._ServiceAddressDefs = new WfServiceAddressDefinitionCollection();

                return this._ServiceAddressDefs;
            }
        }

        /// <summary>
        /// 向上递归查找参数。如果能找到appCodeName和progName对应参数，且参数值不是默认值，则使用。否则使用Default的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appCodeName"></param>
        /// <param name="progCodeName"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueRecursively<T>(string appCodeName, string progCodeName, string propertyName, T defaultValue)
        {
            propertyName.CheckStringIsNullOrEmpty("propertyName");

            WfGlobalParameters parameters = GetProperties(appCodeName, progCodeName);

            if (parameters == null)
            {
                parameters = WfGlobalParameters.Default;
            }
            else
            {
                if (parameters.Properties.ContainsKey(propertyName))
                {
                    if (parameters.Properties[propertyName].IsDefaultValue())
                        parameters = WfGlobalParameters.Default;
                }
            }

            return parameters.Properties.GetValue<T>(propertyName, defaultValue);
        }

        public static WfGlobalParameters LoadDefault()
        {
            return LoadProperties("Default");
        }

        public void Update()
        {
            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

            string properties = formatter.Serialize(this).ToString();

            string updateSQL = string.Format("UPDATE WF.GLOBAL_PARAMETERS SET [PROPERTIES] = {0} WHERE {1}",
                TSqlBuilder.Instance.CheckQuotationMark(properties, true),
                GetWhereSqlClauseBuilder(this.Key).ToSqlString(TSqlBuilder.Instance));

            using (DbContext context = DbContext.GetContext(WorkflowSettings.GetConfig().ConnectionName))
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    if (DbHelper.RunSql(updateSQL, WorkflowSettings.GetConfig().ConnectionName) == 0)
                    {
                        InsertSqlClauseBuilder iBuilder = new InsertSqlClauseBuilder();

                        iBuilder.AppendItem("[KEY]", this.Key);
                        iBuilder.AppendItem("[PROPERTIES]", properties);
                        iBuilder.AppendTenantCode(typeof(WfGlobalParameters));

                        string insertSQL = string.Format("INSERT INTO WF.GLOBAL_PARAMETERS {0}",
                            iBuilder.ToSqlString(TSqlBuilder.Instance));

                        DbHelper.RunSql(insertSQL, WorkflowSettings.GetConfig().ConnectionName);
                    }

                    scope.Complete();
                }
            }

            CacheNotifyData notifyData = new CacheNotifyData(typeof(WfGlobalParametersCache), CalculateCacheKey(this.Key), CacheNotifyType.Invalid);
            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
        }

        public static WfGlobalParameters LoadProperties(string appCodeName, string progCodeName)
        {
            return LoadProperties(WfApplicationProgramCodeName.ToKey(appCodeName, progCodeName));
        }

        public static WfGlobalParameters LoadProperties(string key)
        {
            key.CheckStringIsNullOrEmpty("key");

            WfGlobalParameters parameters = LoadFromDB(key);

            if (parameters == null)
                parameters = new WfGlobalParameters() { Key = key };

            PropertyDefineCollection definedProperties = new PropertyDefineCollection();

            definedProperties.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["GlobalParemeters"]);
            parameters.Properties.MergeDefinedProperties(definedProperties);

            return parameters;
        }

        /// <summary>
        /// 根据Application和Program的CodeName获取参数值
        /// </summary>
        /// <param name="appCodeName"></param>
        /// <param name="progCodeName"></param>
        /// <returns></returns>
        public static WfGlobalParameters GetProperties(string appCodeName, string progCodeName)
        {
            return GetProperties(WfApplicationProgramCodeName.ToKey(appCodeName, progCodeName));
        }

        public static WfGlobalParameters GetProperties(string propsKey)
        {
            propsKey.CheckStringIsNullOrEmpty("propsKey");
            string cacheKey = CalculateCacheKey(propsKey);

            WfGlobalParameters parameters = WfGlobalParametersCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
            {
                WfGlobalParameters ps = LoadProperties(propsKey);

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, ps, dependency);

                return ps;
            });

            return parameters;
        }

        private static WfGlobalParameters LoadFromDB(string key)
        {
            string sql = string.Format("SELECT [PROPERTIES] FROM WF.GLOBAL_PARAMETERS WHERE {0}",
                GetWhereSqlClauseBuilder(key).ToSqlString(TSqlBuilder.Instance));

            WfGlobalParameters result = null;

            string properties = (string)DbHelper.RunSqlReturnScalar(sql, WorkflowSettings.GetConfig().ConnectionName);

            if (properties.IsNotEmpty())
            {
                XElementFormatter formatter = new XElementFormatter();

                formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

                XElement root = XElement.Parse(properties);

                object deserializedData = formatter.Deserialize(root);

                if (deserializedData is PropertyValueCollection)
                {
                    result = new WfGlobalParameters();
                    result.Key = key;
                    result._Properties = (PropertyValueCollection)deserializedData;
                }
                else
                {
                    result = (WfGlobalParameters)deserializedData;
                }
            }

            return result;
        }

        private static WhereSqlClauseBuilder GetWhereSqlClauseBuilder(string key)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("[KEY]", key);
            builder.AppendTenantCode(typeof(WfGlobalParameters));

            return builder;
        }

        private static string CalculateCacheKey(string key)
        {
            string result = key;

            if (TenantContext.Current.Enabled)
                result = string.Format("{0}-{1}", TenantContext.Current.TenantCode, key);

            return result.ToLower();
        }
    }

    internal sealed class WfGlobalParametersCache : CacheQueue<string, WfGlobalParameters>
    {
        public static readonly WfGlobalParametersCache Instance = CacheManager.GetInstance<WfGlobalParametersCache>();

        private WfGlobalParametersCache()
        {
        }
    }
}
