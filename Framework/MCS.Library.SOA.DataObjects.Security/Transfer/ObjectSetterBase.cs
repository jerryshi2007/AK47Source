using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 数据传输时的转换两个对象的值
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TTarget"></typeparam>
	public abstract class ObjectSetterBase<TSource, TTarget>
	{
		/// <summary>
		/// 对象转换
		/// </summary>
		/// <param name="modifyType"></param>
		/// <param name="srcObject"></param>
		/// <param name="targetObject"></param>
		/// <param name="context"></param>
		public abstract void Convert(ObjectModifyType modifyType, TSource srcObject, TTarget targetObject, string context);
	}
}
