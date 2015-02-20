using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.Core
{
    /// <summary>
    /// 简单XML反序列化器
    /// </summary>
    public interface ISimpleXmlDeserializer
    {
        /// <summary>
        /// 从一个XElement元素序列化对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="refNodeName">可参照的节点名称，如果此参数不为空，则从此名称的子节点序列化对象</param>
        void FromXElement(XElement element, string refNodeName);
    }
}
