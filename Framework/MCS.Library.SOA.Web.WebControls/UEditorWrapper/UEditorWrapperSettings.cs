using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 百度的Html编辑器包装控件相关的配置信息
    /// </summary>
    public sealed class UEditorWrapperSettings : ConfigurationSection
    {
        private UEditorWrapperSettings()
        {
        }

        /// <summary>
        /// 得到配置信息
        /// </summary>
        /// <returns></returns>
        public static UEditorWrapperSettings GetConfig()
        {
            UEditorWrapperSettings result = (UEditorWrapperSettings)ConfigurationBroker.GetSection("ueditorWrapperSettings");

            if (result == null)
                result = new UEditorWrapperSettings();

            return result;
        }

        /// <summary>
        /// UEditor客户端脚本的根路径
        /// </summary>
        [ConfigurationProperty("scriptRootPath", IsRequired = false, DefaultValue = "~/UEditor/")]
        public string ScriptRootPath
        {
            get
            {
                return (string)this["scriptRootPath"];
            }
        }

        /// <summary>
        /// UEditor客户端脚本的根路径
        /// </summary>
        [ConfigurationProperty("showImageHandlerUrl", IsRequired = false, DefaultValue = "~/Handlers/ShowUEditorImage.ashx")]
        public string ShowImageHandlerUrl
        {
            get
            {
                return (string)this["showImageHandlerUrl"];
            }
        }

        /// <summary>
        /// UEditor是否自动长高
        /// </summary>
        [ConfigurationProperty("autoHeightEnabled", IsRequired = false, DefaultValue = "False")]
        public bool AutoHeightEnabled
        {
            get
            {
                return (bool)this["autoHeightEnabled"];
            }
        }
    }
}
