using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 类成员访问器的接口
	/// </summary>
	public interface IMemberAccessor
	{
		/// <summary>
		/// 读取成员的值
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="memberName">成员名称</param>
		/// <returns>成员的值</returns>
		object GetValue(object instance, string memberName);

		/// <summary>
		/// 设置成员的值
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="memberName">成员名称</param>
		/// <param name="newValue">值</param>
		void SetValue(object instance, string memberName, object newValue);
	}
}
