using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.Core
{
    /// <summary>
    /// 简单XML序列化器
    /// </summary>
    public interface ISimpleXmlSerializer
    {
        /// <summary>
        /// 将对象序列化到一个XElement元素上
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName">可参照的节点名称，如果此参数不为空，则增加此名称的子节点</param>
        void ToXElement(XElement element, string refNodeName);
    }
}
