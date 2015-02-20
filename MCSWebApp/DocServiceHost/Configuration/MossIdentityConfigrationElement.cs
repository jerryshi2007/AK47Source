using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Configuration;
using System.Configuration;

namespace DocServiceHost.Configuration
{
    public class MossIdentityConfigrationElement : NamedConfigurationElement
    {
        #region Private const
        private const string UserIdItem = "userId";
        private const string PasswordItem = "password";
        private const string DomainItem = "domain";
        private const string IntegratedItem = "integrated";
        #endregion

        /// <summary>
        /// 用户编号
        /// </summary>
        [ConfigurationProperty(MossIdentityConfigrationElement.UserIdItem)]
        public string UserId
        {
            get
            {
                return base[MossIdentityConfigrationElement.UserIdItem] as string;
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        [ConfigurationProperty(MossIdentityConfigrationElement.PasswordItem)]
        public string Password
        {
            get
            {
                return base[MossIdentityConfigrationElement.PasswordItem] as string;
            }
        }

        /// <summary>
        /// 用户所在的域
        /// </summary>
        [ConfigurationProperty(MossIdentityConfigrationElement.DomainItem)]
        public string Domain
        {
            get
            {
                return base[MossIdentityConfigrationElement.DomainItem] as string;
            }
        }
    }

    [ConfigurationCollection(typeof(MossIdentityConfigrationElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class MossIdentityConfigrationElementCollection : NamedConfigurationElementCollection<MossIdentityConfigrationElement>
    {
    }


}