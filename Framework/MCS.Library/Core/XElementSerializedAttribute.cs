using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 对象是否按照字段进行XElement序列化的
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public class XElementSerializableAttribute : Attribute
	{
	}
}
