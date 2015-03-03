using MCS.Library.Passport;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// Principal的配置信息
    /// </summary>
    public class PrincipalSettings : DeluxeConfigurationSection
    {
        private PrincipalSettings()
        {
        }

        /// <summary>
        /// 得到配置
        /// </summary>
        /// <returns></returns>
        public static PrincipalSettings GetConfig()
        {
            PrincipalSettings result = (PrincipalSettings)ConfigurationBroker.GetSection("principalSettings");

            if (result == null)
                result = new PrincipalSettings();

            return result;
        }

        /// <summary>
        /// Principal创建者的定义，如果没有定义，使用缺省的Builder
        /// </summary>
        /// <remarks>
        /// <param name="autoThrow">如果配置不存在，是否直接抛出异常。默认是true</param>
        /// <![CDATA[
        /// <typeFactories>
        /// <add name="pricipalBuilder" type="MCS.Library.Principal.DefaultPrincipalBuilder, DeluxeWorks.Library.Passport" />
        /// </typeFactories>
        /// ]]>
        /// </remarks>
        public IPrincipalBuilder GetPrincipalBuilder(bool autoThrow = true)
        {
            return this.TypeFactories.CheckAndGetInstance<IPrincipalBuilder>("pricipalBuilder", autoThrow);
        }

        /// <summary>
        /// 用户身份扮演信息的加载器，如果没有定义，则不使用身份扮演
        /// </summary>
        /// <param name="autoThrow">如果配置不存在，是否直接抛出异常。默认是true</param>
        /// <remarks>
        /// <![CDATA[
        /// <typeFactories>
        /// <add name="impersonatingInfoLoader" type="typeinfo..., assembly info..." />
        /// </typeFactories>
        /// ]]>
        /// </remarks>
        public IUserImpersonatingInfoLoader GetImpersonatingInfoLoader(bool autoThrow = true)
        {
            return this.TypeFactories.CheckAndGetInstance<IUserImpersonatingInfoLoader>("impersonatingInfoLoader", autoThrow);
        }

        [ConfigurationProperty("typeFactories", IsRequired = false)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }
    }
}
