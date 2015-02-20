using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{

    public class WfMatrixGlobalParameterSetting : ConfigurationSection
    {
     
        private WfMatrixGlobalParameterSetting()
        {
        }

        public static WfMatrixGlobalParameterSetting GetInfo()
        {
            WfMatrixGlobalParameterSetting result =
              (WfMatrixGlobalParameterSetting)ConfigurationBroker.GetSection(WfMatrixGlobalParameterConfig.Element.SectionName);

            if (result == null)
            {
                result = new WfMatrixGlobalParameterSetting(); 
            }           

            return result;
        }

        /// <summary>
        /// 参数列表
        /// </summary>        
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Element.Parameters, IsRequired = true)]
        public WfMatrixGlobalParameters Parameters
        {
            get
            {
                return (WfMatrixGlobalParameters)this[WfMatrixGlobalParameterConfig.Element.Parameters];
            }
        }

        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.Xmlns, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public string Xmlns
        {
            get
            {
                return (string)this[WfMatrixGlobalParameterConfig.Attribute.Xmlns];
            }
        }
    }

    public class WfMatrixGlobalParameterElement : ConfigurationElement
    {
        #region 属性
        /// <summary>
        /// 标识（内部名称）      
        /// </summary>
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.Key, IsRequired = true, IsKey = true)]
        public string Key
        {
            get
            {
                return this[WfMatrixGlobalParameterConfig.Attribute.Key].ToString();
            }
        }

        /// <summary>
        /// 名称      
        /// </summary>
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.Name, IsRequired = true)]
        public string Name
        {
            get
            {
                return this[WfMatrixGlobalParameterConfig.Attribute.Name].ToString();
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.DefaultValue, IsRequired = true)]
        public string DefaultValue
        {
            get
            {
                return this[WfMatrixGlobalParameterConfig.Attribute.DefaultValue].ToString();
            }
        }

        /// <summary>
        ///    启用  
        /// </summary>
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.Enable, IsRequired = true)]
        public bool Enable
        {
            get
            {
                return (bool)this[WfMatrixGlobalParameterConfig.Attribute.Enable];
            }
        }


        /// <summary>
        /// 描述      
        /// </summary>
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.Description, IsRequired = false)]
        public string Description
        {
            get
            {
                return this[WfMatrixGlobalParameterConfig.Attribute.Description].ToString();
            }
        }

        /// <summary>
        /// 默认值类型      
        /// </summary>       
        [ConfigurationProperty(WfMatrixGlobalParameterConfig.Attribute.ValueType, IsRequired = true,DefaultValue=Common.ParaType.String)]
        public Common.ParaType ValueType
        {
            get
            {
                return (Common.ParaType)this[WfMatrixGlobalParameterConfig.Attribute.ValueType];
            }
        }
        #endregion

 
    }

    [ConfigurationCollection(typeof(WfMatrixGlobalParameterElement), AddItemName = WfMatrixGlobalParameterConfig.Element.Parameter, CollectionType = ConfigurationElementCollectionType.BasicMap)]
     public class WfMatrixGlobalParameters : ConfigurationElementCollection
    {
        protected override void BaseAdd(ConfigurationElement element)
        {
            string elementKey = this.GetElementKey(element) as string;
            if (base.BaseGet(elementKey) is WfMatrixGlobalParameterElement)
            {
                throw new ArgumentException(string.Format("配置参数错误{0}:{1}", WfMatrixGlobalParameterConfig.Element.Parameter, elementKey));
            }
            base.BaseAdd(element);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WfMatrixGlobalParameterElement();
        }

        public WfMatrixGlobalParameterElement GetElement(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            WfMatrixGlobalParameterElement element = base.BaseGet(key) as WfMatrixGlobalParameterElement;

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            WfMatrixGlobalParameterElement temp = element as WfMatrixGlobalParameterElement;
            if (temp == null)
            {
                throw new ArgumentException("配置节配置参数不正确");
            }
         
          return temp.Key;
        }

        internal bool IsConfigured
        {
            get
            {
                return (base.Count > 0);
            }
        }
    }

    public class WfMatrixGlobalParameterConfig
    {
        public static class Element
        {
            public const string SectionName = "wfMatrixGlobalParameterSetting";
            public const string Parameters = "parameters";
            public const string Parameter = "parameter";
         
            
        }

        public static class Attribute
        {
            public const string Xmlns = "xmlns";
            public const string Key = "key";
            public const string Name = "name";
            public const string DefaultValue = "defaultValue";
            public const string Enable = "enable";
            public const string Description = "description";
            public const string ValueType = "valueType";
        }
    }
}
