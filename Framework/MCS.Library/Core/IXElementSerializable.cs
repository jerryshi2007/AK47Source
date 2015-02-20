using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.Core
{
	/// <summary>
	/// 对象可以通过实现此接口来完成Xml序列化和反序列化
	/// </summary>
	public interface IXElementSerializable
	{
		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		void Serialize(XElement node, XmlSerializeContext context);

		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		void Deserialize(XElement node, XmlDeserializeContext context);
	}
}
