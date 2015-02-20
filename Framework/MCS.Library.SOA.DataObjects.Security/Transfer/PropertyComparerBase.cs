using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 属性比较器的基类
	/// </summary>
	public abstract class PropertyComparerBase
	{
		/// <summary>
		/// 虚方法，属性比较
		/// </summary>
		/// <param name="srcObject">源对象</param>
		/// <param name="srcPropertyName">源对象的属性名称</param>
		/// <param name="targetObject">目标对象</param>
		/// <param name="targetObjectName">目标对象的属性名称</param>
		/// <param name="context">传递过来的上下文对象</param>
		/// <returns>比较结果</returns>
		public abstract bool Compare(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context);
	}
}
