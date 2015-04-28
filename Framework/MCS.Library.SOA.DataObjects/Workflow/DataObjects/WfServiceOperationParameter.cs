using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务参数描述
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfServiceOperationParameter : ISimpleXmlSerializer
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WfServiceOperationParameter()
        {
        }

        /// <summary>
        /// 从配置信息项构造
        /// </summary>
        /// <param name="element"></param>
        public WfServiceOperationParameter(WfServiceOperationParameterConfigurationElement element)
        {
            this.FromConfigurationElement(element);
        }

        /// <summary>
        /// 初始化服务方法调用时的参数，默认是字符串类型的
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value"></param>
        public WfServiceOperationParameter(string name, string value)
        {
            name.CheckStringIsNullOrEmpty("name");

            this.Name = name;
            this.Type = WfSvcOperationParameterType.String;
            this.Value = value;
        }

        /// <summary>
        /// 初始化服务方法调用时的参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="value">参数值</param>
        public WfServiceOperationParameter(string name, WfSvcOperationParameterType type, object value)
        {
            name.CheckStringIsNullOrEmpty("name");

            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// 从配置信息项转换
        /// </summary>
        /// <param name="element"></param>
        public void FromConfigurationElement(WfServiceOperationParameterConfigurationElement element)
        {
            element.NullCheck("element");

            this.Name = element.Name;
            this.Type = element.Type;
            this.Value = element.Value;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        public WfSvcOperationParameterType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 参数的默认值。当参数类型是RuntimeParameter时，参数值是流程上下文参数名称
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            element.SetAttributeValue("name", this.Name);

            if (this.Value != null)
            {
                if (this.Value is ISimpleXmlSerializer)
                    ((ISimpleXmlSerializer)this.Value).ToXElement(element, string.Empty);
                else
                    SetXElementStringValue(element, "value", this.Value);
            }
        }

        #endregion

        private static void SetXElementStringValue(XElement element, string key, object data)
        {
            try
            {
                element.SetAttributeValue(key, DataConverter.ChangeType(data, typeof(string)));
            }
            catch (System.Exception)
            {
            }
        }
    }

    [Serializable]
    [XElementSerializable]
    public class WfServiceOperationParameterCollection : EditableDataObjectCollectionBase<WfServiceOperationParameter>, ISimpleXmlSerializer
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WfServiceOperationParameterCollection()
        {
        }

        /// <summary>
        /// 从配置项中构造
        /// </summary>
        /// <param name="elements"></param>
        public WfServiceOperationParameterCollection(WfServiceOperationParameterConfigurationElementCollection elements)
        {
            this.FromConfigurationElement(elements);
        }

        /// <summary>
        /// 从配置项中构造
        /// </summary>
        /// <param name="elements"></param>
        public void FromConfigurationElement(WfServiceOperationParameterConfigurationElementCollection elements)
        {
            elements.NullCheck("elements");

            this.Clear();

            foreach (WfServiceOperationParameterConfigurationElement element in elements)
            {
                this.Add(new WfServiceOperationParameter(element));
            }
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            foreach (WfServiceOperationParameter param in this)
            {
                XElement paramElement = element.AddChildElement("Param", string.Empty);

                ((ISimpleXmlSerializer)param).ToXElement(paramElement, string.Empty);
            }
        }

        #endregion
    }
}
