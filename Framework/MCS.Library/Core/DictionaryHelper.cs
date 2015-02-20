using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MCS.Library.Core
{
	/// <summary>
	/// �ֵ���ʵĸ�����
	/// </summary>
	public static class DictionaryHelper
	{
		/// <summary>
		/// ��ȡ�����ֵ��е�ĳһ���������ڴ������ȱʡֵ
		/// </summary>
		/// <typeparam name="TKey">�ֵ�Key������</typeparam>
		/// <typeparam name="TValue">�ֵ��������</typeparam>
		/// <typeparam name="TReturnValue">����ֵ������</typeparam>
		/// <param name="dict">�����ֵ����</param>
		/// <param name="key">��Ҫ���ʵ�keyֵ</param>
		/// <param name="defaultValue">���������keyʱ�����ص�ȱʡֵ</param>
		/// <returns></returns>
		public static TReturnValue GetValue<TKey, TValue, TReturnValue>(this IDictionary<TKey, TValue> dict, TKey key, TReturnValue defaultValue)
		{
			TReturnValue result = defaultValue;

			if (dict != null)
			{
				TValue oResult = default(TValue);

				if (dict.TryGetValue(key, out oResult))
					result = (TReturnValue)DataConverter.ChangeType(oResult, typeof(TReturnValue));
			}

			return result;
		}

		/// <summary>
		/// ��ȡ�ֵ��е�ĳһ���������ڴ������ȱʡֵ
		/// </summary>
		/// <typeparam name="TReturnValue">�ֵ�ķ�������</typeparam>
		/// <param name="dict">�ֵ����</param>
		/// <param name="key">��Ҫ���ʵ�keyֵ</param>
		/// <param name="defaultValue">���������keyʱ�����ص�ȱʡֵ</param>
		/// <returns></returns>
		public static TReturnValue GetValue<TReturnValue>(this IDictionary dict, object key, TReturnValue defaultValue)
		{
			TReturnValue result = defaultValue;

			if (dict != null)
			{
				if (dict.Contains(key))
					result = (TReturnValue)DataConverter.ChangeType(dict[key], typeof(TReturnValue));
			}

			return result;
		}

		/// <summary>
		/// ����ӵ��ֵ���Ǹ����͵�ȱʡֵʱ������Ӵ���
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns>�Ƿ�����˸���</returns>
		public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");
			bool added = false;

			if (data != null)
				if (data is string)
				{
					if (data.ToString() != string.Empty)
					{
						dict.Add(key, data);
						added = true;
					}
				}
				else
					if (data.Equals(default(TValue)) == false)
					{
						dict.Add(key, data);
						added = true;
					}

			return added;
		}

		/// <summary>
		/// ����ӵ��ֵ����ָ����ȱʡֵʱ������Ӵ���
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <param name="defaultValue"></param>
		/// <returns>�Ƿ�����˸���</returns>
		public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data, TValue defaultValue)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");
			bool added = false;

			if (data != null)
			{
				if (data.Equals(defaultValue) == false)
				{
					dict.Add(key, data);
					added = true;
				}
			}
			else
				if (defaultValue != null)
				{
					dict.Add(key, data);
					added = true;
				}

			return added;
		}
	}
}
