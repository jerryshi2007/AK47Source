#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	DataConverter.cs
// Remark	：	提供字符串和枚举、TimeSpan的转换

// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.SOA.Contracts
{
	/// <summary>
	/// 提供字符串与枚举类型的转换，TimeSpan与整形的转换。
	/// </summary>
	/// <remarks>提供字符串和枚举、TimeSpan的转换
	/// </remarks>
	public static class DataConverter
	{
		/// <summary>
		/// 类型转换，提供字符串与枚举型、TimeSpan与整型之间的转换
		/// </summary>
		/// <typeparam name="TSource">源数据的类型</typeparam>
		/// <typeparam name="TResult">目标数据的类型</typeparam>
		/// <param name="srcValue">源数据的值</param>
		/// <returns>类型转换结果</returns>
		/// <remarks>
		/// 数据转换，主要调用系统Convert类的ChangeType方法，但是对于字符串与枚举，整型与TimeSpan类型之间的转换，进行了特殊处理。
		/// <seealso cref="MCS.Library.Core.XmlHelper"/>
		/// </remarks>
		public static TResult ChangeType<TSource, TResult>(TSource srcValue)
		{
			return (TResult)ChangeType(srcValue, typeof(TResult));
		}

		/// <summary>
		/// 字符串与枚举型、TimeSpan与整型之间转换的方法。
		/// </summary>
		/// <typeparam name="TSource">源数据类型</typeparam>
		/// <param name="srcValue">源数据的值</param>
		/// <param name="targetType">目标数据类型</param>
		/// <returns>类型转换后的结果</returns>
		/// <remarks>字符串与枚举型、TimeSpan与整型之间转换的方法。
		/// <seealso cref="MCS.Library.Core.XmlHelper"/>
		/// </remarks>
		public static object ChangeType<TSource>(TSource srcValue, System.Type targetType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(targetType != null, "targetType");
			bool dealed = false;
			object result = null;
			System.Type srcType = typeof(TSource);

			if (srcType == typeof(object))
				if (srcValue != null)
					srcType = srcValue.GetType();

			if (targetType.IsEnum)
			{
				if (srcType == typeof(string) || srcType == typeof(int))
				{
					result = Enum.Parse(targetType, srcValue.ToString());
					dealed = true;
				}
			}
			else
				if (targetType == typeof(TimeSpan))
				{
					if (srcType == typeof(TimeSpan))
						result = srcValue;
					else
						result = TimeSpan.FromSeconds((double)Convert.ChangeType(srcValue, typeof(double)));

					dealed = true;
				}
				else
					if (targetType == typeof(bool) && srcType == typeof(string))
						result = StringToBool(srcValue.ToString(), out dealed);
					else
						if (targetType == typeof(DateTime) && srcType == typeof(string))
						{
							if (srcValue == null || srcValue.ToString() == string.Empty)
							{
								result = DateTime.MinValue;
								dealed = true;
							}
						}

			if (dealed == false)
			{
				if (targetType != typeof(object) && targetType.IsAssignableFrom(srcType))
					result = srcValue;
				else
				{
					try
					{
						result = Convert.ChangeType(srcValue, targetType);
					}
					catch (System.Exception ex)
					{
						throw ex;
					}
				}
			}

			return result;
		}

		private static bool StringToBool(string srcValue, out bool dealed)
		{
			bool result = false;
			dealed = false;

			srcValue = srcValue.Trim();

			if (srcValue.Length > 0)
			{
				if (srcValue.Length == 1)
				{
					result = ((string.Compare(srcValue, "0") != 0) && (string.Compare(srcValue, "n", true) != 0));

					dealed = true;
				}
				else
				{
					if (string.Compare(srcValue, "YES", true) == 0 || string.Compare(srcValue, "是", true) == 0 || string.Compare(srcValue, "TRUE", true) == 0)
					{
						result = true;
						dealed = true;
					}
					else
					{
						if (string.Compare(srcValue, "NO", true) == 0 || string.Compare(srcValue, "否", true) == 0 || string.Compare(srcValue, "FALSE", true) == 0)
						{
							result = false;
							dealed = true;
						}
					}
				}
			}
			else
			{
				dealed = true;	//空串表示False
			}

			return result;
		}
	}
}
