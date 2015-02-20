using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace MCS.Library.Globalization
{
	/// <summary>
	/// 默认的翻译器
	/// </summary>
	public interface ITranslator
	{
		/// <summary>
		/// 翻译字符串
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceCulture"></param>
		/// <param name="sourceText"></param>
		/// <param name="targetCulture"></param>
		/// <param name="objParams"></param>
		/// <returns></returns>
		string Translate(string category, CultureInfo sourceCulture, string sourceText, CultureInfo targetCulture, params object[] objParams);
	}
}
