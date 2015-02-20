#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	DataConverter.cs
// Remark	��	�ṩ�ַ�����ö�١�TimeSpan��ת��

// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.SOA.Contracts
{
	/// <summary>
	/// �ṩ�ַ�����ö�����͵�ת����TimeSpan�����ε�ת����
	/// </summary>
	/// <remarks>�ṩ�ַ�����ö�١�TimeSpan��ת��
	/// </remarks>
	public static class DataConverter
	{
		/// <summary>
		/// ����ת�����ṩ�ַ�����ö���͡�TimeSpan������֮���ת��
		/// </summary>
		/// <typeparam name="TSource">Դ���ݵ�����</typeparam>
		/// <typeparam name="TResult">Ŀ�����ݵ�����</typeparam>
		/// <param name="srcValue">Դ���ݵ�ֵ</param>
		/// <returns>����ת�����</returns>
		/// <remarks>
		/// ����ת������Ҫ����ϵͳConvert���ChangeType���������Ƕ����ַ�����ö�٣�������TimeSpan����֮���ת�������������⴦��
		/// <seealso cref="MCS.Library.Core.XmlHelper"/>
		/// </remarks>
		public static TResult ChangeType<TSource, TResult>(TSource srcValue)
		{
			return (TResult)ChangeType(srcValue, typeof(TResult));
		}

		/// <summary>
		/// �ַ�����ö���͡�TimeSpan������֮��ת���ķ�����
		/// </summary>
		/// <typeparam name="TSource">Դ��������</typeparam>
		/// <param name="srcValue">Դ���ݵ�ֵ</param>
		/// <param name="targetType">Ŀ����������</param>
		/// <returns>����ת����Ľ��</returns>
		/// <remarks>�ַ�����ö���͡�TimeSpan������֮��ת���ķ�����
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
					if (string.Compare(srcValue, "YES", true) == 0 || string.Compare(srcValue, "��", true) == 0 || string.Compare(srcValue, "TRUE", true) == 0)
					{
						result = true;
						dealed = true;
					}
					else
					{
						if (string.Compare(srcValue, "NO", true) == 0 || string.Compare(srcValue, "��", true) == 0 || string.Compare(srcValue, "FALSE", true) == 0)
						{
							result = false;
							dealed = true;
						}
					}
				}
			}
			else
			{
				dealed = true;	//�մ���ʾFalse
			}

			return result;
		}
	}
}
