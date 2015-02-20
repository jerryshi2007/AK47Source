using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
    public sealed class DialogHelperWrapperSettings : ConfigurationSection
    {
        private DialogHelperWrapperSettings()
        {
        }

        public static DialogHelperWrapperSettings GetConfig()
        {
            DialogHelperWrapperSettings settings = (DialogHelperWrapperSettings)ConfigurationBroker.GetSection("dialogHelperWrapperSettings");

            if (settings == null)
                settings = new DialogHelperWrapperSettings();

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

        [ConfigurationProperty("classID", IsRequired = false, DefaultValue = "clsid:C86C48A2-0DAD-41B6-BB85-AAB912FEE3AB")]
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
