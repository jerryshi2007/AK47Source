
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
    public sealed class RelativeLinkControlConfigSetting : ConfigurationSection
    {
        private const string LockSettingsName = "relativeLinkControlConfigSetting";

        public static RelativeLinkControlConfigSetting GetConfig()
        {
            var result =
                (RelativeLinkControlConfigSetting)ConfigurationBroker.GetSection(LockSettingsName);

            result.CheckSectionNotNull(LockSettingsName);

            return result;
        }
        /// <summary>
        /// 是否自动打开或关闭相关信息控件
        /// </summary>
        [ConfigurationProperty("DefaultShowingMode", IsRequired = true)]
        public string DefaultShowingMode
        {
            get
            {
                return this["DefaultShowingMode"].ToString();
            }
        }
    }
}
