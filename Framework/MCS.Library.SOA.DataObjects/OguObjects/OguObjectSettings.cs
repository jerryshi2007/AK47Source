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
    /// Ӧ����ʹ�õ�ȫ�ֵĻ�����Ա������
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
        /// OguObject��ȫ��ַ�ַ���
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
                            Translator.Translate(Define.DefaultCulture, "�����ҵ�·��Ϊ'{0}'�Ļ�����Ա����", FullPath));

                        result = objs[0];
                    }
                    else
                    {
                        OguObjectCollection<IUser> objs =
                            OguMechanismFactory.GetMechanism().GetObjects<IUser>(PathType, FullPath);

                        ExceptionHelper.FalseThrow(objs.Count > 0,
                            Translator.Translate(Define.DefaultCulture, "�����ҵ���¼��Ϊ'{0}'����Ա����", FullPath));

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
    /// ����OguObject���������
    /// </summary>
    public class OguObjectConfigurationElementCollection : NamedConfigurationElementCollection<OguObjectConfigurationElement>
    {
        public new OguObjectConfigurationElement this[string name]
        {
            get
            {
                OguObjectConfigurationElement result = base[name];

                ExceptionHelper.FalseThrow(result != null,
                    Translator.Translate(Define.DefaultCulture, "������oguObjectSettings���ýڵ�objects���������ҵ�nameΪ'{0}'��������", name));

                return result;
            }
        }
    }

    internal sealed class OguContextCache : ContextCacheQueueBase<string, IOguObject>
    {
        /// <summary>
        /// �˴����������ԣ���̬���㣬������ReadOnly����
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
