using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
    public sealed class ComponentHelperWrapperSettings : ConfigurationSection
    {
        private ComponentHelperWrapperSettings()
        {
        }

        public static ComponentHelperWrapperSettings GetConfig()
        {
            ComponentHelperWrapperSettings settings = (ComponentHelperWrapperSettings)ConfigurationBroker.GetSection("componentHelperWrapperSettings");

            if (settings == null)
                settings = new ComponentHelperWrapperSettings();

            return settings;
        }

        [ConfigurationProperty("codebase", IsRequired = false, DefaultValue = "/MCSWebApp/HBWebHelperControl/HBWebHelperControl.CAB#version=1,0,0,13")]
        public string Codebase
        {
            get
            {
                return (string)this["codebase"];
            }
            set
            {
                this["codebase"] = value;
            }
        }

        [ConfigurationProperty("classID", IsRequired = false, DefaultValue = "clsid:918CFB81-4755-4167-BFC7-879E9DD52C9E")]
        public string ClassID
        {
            get
            {
                return (string)this["classID"];
            }
            set
            {
                this["classID"] = value;
            }
        }
    }
}
