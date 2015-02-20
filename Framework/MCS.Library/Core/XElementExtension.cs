using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.Core
{
    /// <summary>
    /// XElement的扩展
    /// </summary>
    public static class XElementExtension
    {
        /// <summary>
        /// 扩展Element方法，如果Element不存在，返回缺省值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Element<T>(this XElement parent, XName name, T defaultValue)
        {
            T result = defaultValue;

            if (parent != null)
            {
                XElement element = parent.Element(name);

                if (element != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(element.Value);
                    else
                        result = (T)DataConverter.ChangeType(element.Value, defaultValue.GetType());
                }
            }

            return result;
        }

        /// <summary>
        /// 得到Element的字符串Value，如果不存在，返回空串
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ElementValue(this XElement parent, XName name)
        {
            return Element(parent, name, string.Empty);
        }

        /// <summary>
        /// 得到属性的值，如果不存在，则返回缺省值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Attribute<T>(this XElement parent, XName name, T defaultValue)
        {
            T result = defaultValue;

            if (parent != null)
            {
                XAttribute attribute = parent.Attribute(name);

                if (attribute != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(attribute.Value);
                    else
                        result = (T)DataConverter.ChangeType(attribute.Value, defaultValue.GetType());
                }
            }

            return result;
        }

        /// <summary>
        /// 得到Attribute的字符串Value，如果不存在，返回空串
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string AttributeValue(this XElement parent, XName name)
        {
            return Attribute(parent, name, string.Empty);
        }

        /// <summary>
        /// 得到属性的值，先用AlterName去取，如果不存在，则使用name，如果还不存在，则返回缺省值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="alterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T AttributeWithAlterName<T>(this XElement parent, XName name, XName alterName, T defaultValue)
        {
            T result = defaultValue;

            if (parent != null)
            {
                XAttribute attribute = parent.Attribute(alterName);

                if (attribute == null)
                    attribute = parent.Attribute(name);

                if (attribute != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(attribute.Value);
                    else
                        result = (T)DataConverter.ChangeType(attribute.Value, defaultValue.GetType());
                }
            }

            return result;
        }

        /// <summary>
        /// 得到Attribute的字符串Value，先用AlterName去取，如果不存在，则使用name，如果还不存在，返回空串
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="alterName"></param>
        /// <returns></returns>
        public static string AttributeValueWithAlterName(this XElement parent, XName name, XName alterName)
        {
            return AttributeWithAlterName(parent, name, alterName, string.Empty);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement AddChildElement(this XElement parent, XName name)
        {
            (parent != null).FalseThrow<ArgumentNullException>("parent");

            XElement node = new XElement(name);

            parent.Add(node);

            return node;
        }

        /// <summary>
        /// 添加子节点的同时，设置子节点的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XElement AddChildElement<T>(this XElement parent, XName name, T nodeValue)
        {
            XElement node = AddChildElement(parent, name);

            if (nodeValue != null)
                node.Value = nodeValue.ToString();

            return node;
        }
    }
}
