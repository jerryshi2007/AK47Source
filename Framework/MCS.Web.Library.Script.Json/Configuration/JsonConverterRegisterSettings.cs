using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Configuration
{
    public class JsonConverterRegisterSettings : DeluxeConfigurationSection
    {
        public static JsonConverterRegisterSettings GetConfig()
        {
            JsonConverterRegisterSettings result = (JsonConverterRegisterSettings)ConfigurationBroker.GetSection("jsonConverterRegisterSettings");

            return result ?? new JsonConverterRegisterSettings();
        }

        public IEnumerable<IJsonConverterRegister> GetRegisters()
        {
            foreach (TypeConfigurationElement element in this.RegisterConfigElements)
            {
                IJsonConverterRegister register = element.CreateInstance() as IJsonConverterRegister;

                if (register != null)
                    yield return register;
            }
        }

        /// <summary>
        /// 插件配置信息
        /// </summary>
        [ConfigurationProperty("registers", IsRequired = false)]
        private TypeConfigurationCollection RegisterConfigElements
        {
            get
            {
                return (TypeConfigurationCollection)this["registers"];
            }
        }
    }
}
