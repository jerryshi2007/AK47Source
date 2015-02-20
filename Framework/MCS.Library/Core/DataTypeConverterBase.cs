using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 数据类型转换器的基类
	/// </summary>
	public abstract class DataTypeConverterBase
	{
		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="srcObject">源数据</param>
		/// <param name="targetObject">目标数据</param>
		public abstract void Convert(object srcObject, object targetObject);

		/// <summary>
		/// 检查类型
		/// </summary>
		/// <param name="data"></param>
		/// <param name="type"></param>
		/// <param name="paramName"></param>
		protected static void CheckType(object data, Type type, string paramName)
		{
			data.NullCheck(paramName);

			type.IsInstanceOfType(data).FalseThrow("参数{0}的类型是{1}，不是类型{2}", paramName, data.GetType().Name, type.Name);
		}
	}

	/// <summary>
	/// 泛型的类型转换器
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TTarget"></typeparam>
	public abstract class DataTypeConverterGenericBase<TSource, TTarget> : DataTypeConverterBase
	{
		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="srcObject"></param>
		/// <param name="targetObject"></param>
		public override void Convert(object srcObject, object targetObject)
		{
			if (srcObject != null)
			{
				targetObject.NullCheck(string.Format("目标对象为NULL, 类型为{0}", typeof(TTarget).Name));

				CheckType(srcObject, typeof(TSource), "srcObject");
				CheckType(targetObject, typeof(TTarget), "targetObject");
				GenericConvert((TSource)srcObject, (TTarget)targetObject);
			}
		}

		/// <summary>
		/// 基于泛型的转换方法
		/// </summary>
		/// <param name="srcObject"></param>
		/// <param name="targetObject"></param>
		protected abstract void GenericConvert(TSource srcObject, TTarget targetObject);
	}
}
