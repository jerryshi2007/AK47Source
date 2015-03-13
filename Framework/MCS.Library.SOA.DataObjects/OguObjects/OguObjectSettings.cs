using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Configuration;
using MCS.Library.OGUPermission;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 应用所使用的全局的机构人员对象定义
    /// </summary>
    public sealed class OguObjectSettings : ConfigurationSection
    {
        public static OguObjectSettings GetConfig()
        {
            OguObjectSettings settings = (OguObjectSettings)ConfigurationBroker.GetSection("oguObjectSettings");

            if (settings == null)
                settings = new OguObjectSettings();

            return settings;
        }

        private OguObjectSettings()
        {
        }

        [ConfigurationProperty("objects", IsRequired = false)]
        public OguObjectConfigurationElementCollection Objects
        {
            get
            {
                return (OguObjectConfigurationElementCollection)this["objects"];
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OguObjectConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// OguObject的全地址字符串
        /// </summary>
        [ConfigurationProperty("fullPath", IsRequired = true)]
        public string FullPath
        {
            get
            {
                return (string)this["fullPath"];
            }
        }

        [ConfigurationProperty("pathType", IsRequired = false, DefaultValue = SearchOUIDType.FullPath)]
        public SearchOUIDType PathType
        {
            get
            {
                return (SearchOUIDType)this["pathType"];
            }
        }

        public IOguObject Object
        {
            get
            {
                IOguObject result = null;

                if (OguContextCache.Instance.TryGetValue(FullPath, out result) == false)
                {
                    if (PathType != SearchOUIDType.LogOnName)
                    {
                        OguObjectCollection<IOguObject> objs =
                            OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(PathType, FullPath);

                        ExceptionHelper.FalseThrow(objs.Count > 0,
                            Translator.Translate(Define.DefaultCulture, "不能找到路径为'{0}'的机构人员对象", FullPath));

                        result = objs[0];
                    }
                    else
                    {
                        OguObjectCollection<IUser> objs =
                            OguMechanismFactory.GetMechanism().GetObjects<IUser>(PathType, FullPath);

                        ExceptionHelper.FalseThrow(objs.Count > 0,
                            Translator.Translate(Define.DefaultCulture, "不能找到登录名为'{0}'的人员对象", FullPath));

                        result = objs[0];
                    }

                    OguContextCache.Instance.Add(FullPath, result);
                }

                result = OguBase.CreateWrapperObject(result);

                return result;
            }
        }

        public IUser User
        {
            get
            {
                return this.Object as IUser;
            }
        }

        public IOrganization Department
        {
            get
            {
                return this.Object as IOrganization;
            }
        }

        public IGroup Group
        {
            get
            {
                return this.Object as IGroup;
            }
        }
    }

    /// <summary>
    /// 关于OguObject的配置项集合
    /// </summary>
    public class OguObjectConfigurationElementCollection : NamedConfigurationElementCollection<OguObjectConfigurationElement>
    {
        public new OguObjectConfigurationElement this[string name]
        {
            get
            {
                OguObjectConfigurationElement result = base[name];

                ExceptionHelper.FalseThrow(result != null,
                    Translator.Translate(Define.DefaultCulture, "不能在oguObjectSettings配置节的objects配置项下找到name为'{0}'的配置项", name));

                return result;
            }
        }
    }

    internal sealed class OguContextCache : ContextCacheQueueBase<string, IOguObject>
    {
        /// <summary>
        /// 此处必须是属性，动态计算，不能是ReadOnly变量
        /// </summary>
        public static OguContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<OguContextCache>();
            }
        }

        private OguContextCache()
        {
        }
    }
}
