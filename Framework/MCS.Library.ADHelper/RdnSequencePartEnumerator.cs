using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace MCS.Library
{
	/// <summary>
	/// 用于从Rdn(相对区分名称)序列提取名称
	/// </summary>
	public sealed class RdnSequencePartEnumerator : IEnumerable<string>
	{
		/*
		 Converting an AttributeValue from ASN.1 to a String


   If the AttributeValue is of a type which does not have a string
   representation defined for it, then it is simply encoded as an
   octothorpe character ('#' ASCII 35) followed by the hexadecimal
   representation of each of the bytes of the BER encoding of the X.500
   AttributeValue.  This form SHOULD be used if the AttributeType is of
   the dotted-decimal form.

   Otherwise, if the AttributeValue is of a type which has a string
   representation, the value is converted first to a UTF-8 string
   according to its syntax specification (see for example section 6 of
   [4]).

   If the UTF-8 string does not have any of the following characters
   which need escaping, then that string can be used as the string
   representation of the value.

    o   a space or "#" character occurring at the beginning of the
        string

    o   a space character occurring at the end of the string

    o   one of the characters ",", "+", """, "\", "<", ">" or ";"

   Implementations MAY escape other characters.

   If a character to be escaped is one of the list shown above, then it
   is prefixed by a backslash ('\' ASCII 92).

   Otherwise the character to be escaped is replaced by a backslash and
   two hex digits, which form a single byte in the code of the
   character.
		 */
		private char[] buffer;

		public RdnSequencePartEnumerator(string dn)
		{
			this.buffer = dn.ToCharArray();
		}

		public IEnumerator<string> GetEnumerator()
		{
			int checkPoint = 0;
			// 例子： CN=Before\0DAfter,O=Test,C=GB
			// http://tools.ietf.org/html/rfc2253#section-5
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == '\\')
				{
					SkipEscape(buffer, ref i); // 跳过转义字符
					continue;
				}
				else if (buffer[i] == ',') // 分隔符
				{
					yield return new string(buffer, checkPoint, i - checkPoint);
					checkPoint = i + 1;
				}
			}

			if (checkPoint < buffer.Length)
			{
				yield return new string(buffer, checkPoint, buffer.Length - checkPoint);
			}
		}

		public int Count()
		{
			int count = 0;
			int checkPoint = 0;
			// 例子： CN=Before\0DAfter,O=Test,C=GB
			// http://tools.ietf.org/html/rfc2253#section-5
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == '\\')
				{
					SkipEscape(buffer, ref i); // 跳过转义字符
					continue;
				}
				else if (buffer[i] == ',') // 分隔符
				{
					count++;
					checkPoint = i + 1;
				}
			}

			if (checkPoint < buffer.Length)
			{
				count++;
			}

			return count;
		}

		/// <summary>
		/// 跳过转义字符
		/// </summary>
		/// <param stringValue="buffer">存放缓冲字符</param>
		/// <param stringValue="i">从这里开始转义（转义符开始），返回转义符结束的下标</param>
		public static void SkipEscape(char[] buffer, ref int i)
		{
			if (buffer[i] == '\\')
			{
				i++;
				if (i < buffer.Length)
				{
					char c = buffer[i]; // 通常直接跟一个被转义的字符
					if (AttributeHelper.IsHexDigit(c))
					{
						// 如果是十六进制数字，则需要2个数字
						i++;
						if (i < buffer.Length)
						{
							c = buffer[i];
							if (AttributeHelper.IsHexDigit(c) == false)
							{
								throw new FormatException("十六进制转义应该由2个十六进制数字组成");
							}
						}
						else
							throw new FormatException("遇到了未终止的十六进制转义序列");
					}
				}
				else
					throw new FormatException("遇到了未终止的转义符");
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
