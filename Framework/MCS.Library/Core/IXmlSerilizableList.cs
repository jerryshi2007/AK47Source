using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// Xml序列化所需要实现的特殊List
	/// </summary>
	public interface IXmlSerilizableList
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		void Add(object data);
		
		/// <summary>
		/// 
		/// </summary>
		void Clear();
	}
}
