﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 字符串操作的扩展方法
	/// </summary>
	public static class StringExtension
	{
		/// <summary>
		/// 空的数组
		/// </summary>
		public static readonly string[] EmptyStringArray = new string[0];

		/// <summary>
		/// 将byte数组转换为base16的字符串
		/// </summary>
		/// <param name="data">待转换的byte数组</param>
		/// <returns>转换好的16进制字符串</returns>
		public static string ToBase16String(this byte[] data)
		{
			StringBuilder strB = new StringBuilder();

			if (data != null)
			{
				for (int i = 0; i < data.Length; i++)
					strB.AppendFormat("{0:x2}", data[i]);
			}

			return strB.ToString();
		}

		/// <summary>
		/// 将保存好的16进制字符串转换为byte数组
		/// </summary>
		/// <param name="strData"></param>
		/// <returns></returns>
		public static byte[] ToBase16Bytes(this string strData)
		{
			byte[] data = null;

			if (strData != null)
			{
				data = new Byte[strData.Length / 2];

				for (int i = 0; i < strData.Length / 2; i++)
					data[i] = Convert.ToByte(strData.Substring(i * 2, 2), 16);
			}
			else
				data = new byte[0];

			return data;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="data"></param>
		public static void AppendWithSplitChars(this StringBuilder strB, string data)
		{
			AppendWithSplitChars(strB, data, " ");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="data"></param>
		/// <param name="splitChars"></param>
		public static void AppendWithSplitChars(this StringBuilder strB, string data, string splitChars)
		{
			if (data.IsNotEmpty())
			{
				if (strB.Length > 0 && splitChars.IsNotEmpty())
					strB.Append(splitChars);

				strB.Append(data);
			}
		}
	}
}
