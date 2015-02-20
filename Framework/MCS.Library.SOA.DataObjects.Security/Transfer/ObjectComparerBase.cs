using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	/// <summary>
	/// 用于数据传输时的对象比较。数据源为一般为权限中心，目的随意
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TTarget"></typeparam>
	public abstract class ObjectComparerBase<TSource, TTarget>
	{
		public abstract ObjectModifyType Compare(TSource srcObject, TTarget targetObject);
	}
}
