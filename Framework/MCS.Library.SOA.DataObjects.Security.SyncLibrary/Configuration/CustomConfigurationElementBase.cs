using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    public abstract class CustomConfigurationElementBase : ConfigurationElement
    {
        private ConfigurationPropertyCollection _properties;
        private NameValueCollection _PropertyNameCollection;

        protected static readonly StringValidator NotEmptyStringValidator = new StringValidator(1);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                this.UpdatePropertyCollection();
                return this._properties;
            }
        }

        public NameValueCollection Parameters
        {
            get
            {
                if (this._PropertyNameCollection == null)
                {
                    lock (this)
                    {
                        if (this._PropertyNameCollection == null)
                        {
                            this._PropertyNameCollection = new NameValueCollection(System.StringComparer.Ordinal);
                            foreach (object obj2 in this._properties)
                            {
                                ConfigurationProperty property = (ConfigurationProperty)obj2;
                                if (this.IsBuiltInProperty(property.Name) == false)
                                {
                                    this._PropertyNameCollection.Add(property.Name, (string)base[property]);
                                }
                            }
                        }
                    }
                }
                return this._PropertyNameCollection;
            }
        }

        protected CustomConfigurationElementBase()
        {
            this._properties = new ConfigurationPropertyCollection();
            this._PropertyNameCollection = null;
            InitializeBuildInProperties(_properties);
        }

        protected virtual void InitializeBuildInProperties(ConfigurationPropertyCollection properties)
        {
        }

        private string GetProperty(string PropName)
        {
            if (this._properties.Contains(PropName))
            {
                ConfigurationProperty property = this._properties[PropName];
                if (property != null)
                {
                    return (string)base[property];
                }
            }
            return null;
        }

        protected override bool IsModified()
        {
            if (!this.UpdatePropertyCollection())
            {
                return base.IsModified();
            }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);
            this._properties.Add(property);
            base[property] = value;
            this.Parameters[name] = value;
            return true;
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            CustomTypeConfigurationElementBase settings = parentElement as CustomTypeConfigurationElementBase;
            if (settings != null)
            {
                settings.UpdatePropertyCollection();
            }

            base.Reset(parentElement);
        }

        private bool SetProperty(string PropName, string value)
        {
            ConfigurationProperty property = null;
            if (this._properties.Contains(PropName))
            {
                property = this._properties[PropName];
            }
            else
            {
                property = new ConfigurationProperty(PropName, typeof(string), null);
                this._properties.Add(property);
            }
            if (property != null)
            {
                base[property] = value;
                return true;
            }
            return false;
        }

        protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            CustomConfigurationElementBase settings = parentElement as CustomConfigurationElementBase;
            if (settings != null)
            {
                settings.UpdatePropertyCollection();
            }
            CustomConfigurationElementBase settings2 = sourceElement as CustomConfigurationElementBase;
            if (settings2 != null)
            {
                settings2.UpdatePropertyCollection();
            }
            base.Unmerge(sourceElement, parentElement, saveMode);
            this.UpdatePropertyCollection();
        }

        internal bool UpdatePropertyCollection()
        {
            bool flag = false;
            ArrayList list = null;
            if (this._PropertyNameCollection != null)
            {
                foreach (ConfigurationProperty property in this._properties)
                {
                    if ((IsBuiltInProperty(property.Name) == false) && (this._PropertyNameCollection.Get(property.Name) == null))
                    {
                        if (list == null)
                        {
                            list = new ArrayList();
                        }
                        //if ((base.Values.GetConfigValue(property.Name).ValueFlags & ConfigurationValueFlags.Locked) == ConfigurationValueFlags.Default)
                        //{
                        list.Add(property.Name);
                        flag = true;
                        //}
                    }
                }
                if (list != null)
                {
                    foreach (string str in list)
                    {
                        this._properties.Remove(str);
                    }
                }
                foreach (string str2 in this._PropertyNameCollection)
                {
                    string str3 = this._PropertyNameCollection[str2];
                    string str4 = this.GetProperty(str2);
                    if ((str4 == null) || (str3 != str4))
                    {
                        this.SetProperty(str2, str3);
                        flag = true;
                    }
                }
            }
            this._PropertyNameCollection = null;
            return flag;
        }

        protected virtual bool IsBuiltInProperty(string propertyName)
        {
            return false;
        }
    }
}
