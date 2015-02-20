using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 命令行参数的分析器，将命令行参数解析为字符串数组
	/// </summary>
	public static class ArgumentsParser
	{
		/// <summary>
		/// 分析和格式化命令行参数，返回Key、Value的字典
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static StringDictionary Parse(string[] arguments)
		{
			arguments.NullCheck("arguments");

			StringDictionary result = new StringDictionary();

			for (int i = 0; i < arguments.Length; i++)
			{
				KeyValuePair<string, string> kp = ParseArgument(arguments[i]);

				result[kp.Key] = kp.Value;
			}

			return result;
		}

		private static KeyValuePair<string, string> ParseArgument(string keyValue)
		{
			int equalIndex = keyValue.IndexOf('=');

			string key = keyValue;
			string value = string.Empty;

			if (equalIndex >= 0)
			{
				key = keyValue.Substring(0, equalIndex);
				value = keyValue.Substring(equalIndex + 1);
			}

			key = NormalizeKey(key);
			value = NormalizeValue(value);

			return new KeyValuePair<string, string>(key, value);
		}

		private static string NormalizeKey(string key)
		{
			string result = key;

			if (key != null)
				result = key.Trim('-', '/', '\\', ' ');

			return result;
		}

		private static string NormalizeValue(string value)
		{
			string result = value;

			if (value != null)
				result = value.Trim(' ', '\"');

			return result;
		}
	}
}
