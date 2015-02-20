using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library
{
	public static class AttributeHelper
	{
		private static readonly char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

		private static readonly long DatetimeMinTimeTicks = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

		/// <summary>
		/// 将十六进制数字转换成整数
		/// </summary>
		/// <param name="c">0-9或a-f</param>
		/// <returns>整数值</returns>
		public static byte FromHex(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - '0');
			}
			else if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 'a' + 0xA);
			}
			else if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 'A' + 0xA);
			}
			else
				throw new FormatException("不是十六进制数字");
		}

		public static byte[] FromHex(string hexString)
		{
			if (hexString == null)
				throw new ArgumentNullException("hexString");
			if (hexString.Length % 2 != 0)
				throw new FormatException("十六进制数字必须2个一组，中间不留空格和分隔符");

			byte[] result = new byte[hexString.Length >> 1];

			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (byte)((FromHex(hexString[i << 1]) << 4) & FromHex(hexString[(i << 1) + 1]));
			}

			return result;
		}

		public static byte[] FromHex(char[] hexString)
		{
			if (hexString == null)
				throw new ArgumentNullException("hexString");
			if (hexString.Length % 2 != 0)
				throw new FormatException("十六进制数字必须2个一组，中间不留空格和分隔符");

			byte[] result = new byte[hexString.Length >> 1];

			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (byte)((FromHex(hexString[i << 1]) << 4) & FromHex(hexString[(i << 1) + 1]));
			}

			return result;
		}

		/// <summary>
		/// 获取表示指定字节的高位或低位的十六进制数字
		/// </summary>
		/// <param name="b">指定的字节</param>
		/// <param name="highOrder">为<see langword="true"/>时获取高位，为<see langword="true"/>时获取低位。</param>
		/// <returns></returns>
		public static char Hex(byte b, bool highOrder)
		{
			if (highOrder)
			{
				b = (byte)((b >> 4) & 0x0f);
			}
			else
			{
				b = (byte)(b & 0x0f);
			}

			return hexDigits[b];
		}

		/// <summary>
		/// 将指定的字节数组转换为十六进制（小写）
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static string Hex(byte[] buffer)
		{
			char[] chars = new char[buffer.Length << 1];
			for (int i = 0; i < buffer.Length; i++)
			{
				chars[i << 1] = Hex(buffer[i], true);
				chars[(i << 1) + 1] = Hex(buffer[i], false);
			}

			return new string(chars);
		}

		/// <summary>
		/// 确定一个字符是否属于十六进制数字类别
		/// </summary>
		/// <param name="c">要检测的字符</param>
		/// <returns>是十六进制数字则为<see langword="true"/>，否则为<see langword="false"/>。</returns>
		public static bool IsHexDigit(char c)
		{
			c = char.ToUpperInvariant(c);
			return ((c >= 'A' && c <= 'F') || (c >= '0' && c <= '9'));
		}

		/// <summary>
		/// 将UTC日期转换为Javascript的日期值
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static long ToJavascriptData(DateTime date)
		{
			return (date.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710L;
		}

		public static DateTime FromJavascriptDate(int dateValue)
		{
			return new DateTime((dateValue * 0x2710L) + DatetimeMinTimeTicks, DateTimeKind.Utc);
		}
	}
}
