using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;

namespace MCS.Library.Globalization
{
	/// <summary>
	/// 翻译器
	/// </summary>
	public static class Translator
	{
		/// <summary>
		/// 翻译
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceCulture"></param>
		/// <param name="sourceText"></param>
		/// <param name="targetCulture"></param>
		/// <param name="objParams"></param>
		/// <returns></returns>
		public static string Translate(string category, CultureInfo sourceCulture, string sourceText, CultureInfo targetCulture, params object[] objParams)
		{
			ITranslator t = TranslatorConfigSettings.GetConfig().Translator;

			string result = t.Translate(category, sourceCulture, sourceText, targetCulture);

			if (result == null)
			{
				return result;
			}

			if (objParams.Length > 0)
				result = string.Format(result, objParams);

			return result;
		}

		/// <summary>
		/// 翻译
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceText"></param>
		/// <param name="targetCulture"></param>
		/// <param name="objParams"></param>
		/// <returns></returns>
		public static string Translate(string category, string sourceText, CultureInfo targetCulture, params object[] objParams)
		{
			return Translate(category, TranslatorConfigSettings.GetConfig().DefaultCulture, sourceText, targetCulture, objParams);
		}

		/// <summary>
		/// 翻译
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceText"></param>
		/// <param name="objParams"></param>
		/// <returns></returns>
		public static string Translate(string category, string sourceText, params object[] objParams)
		{
			return Translate(category, sourceText, Thread.CurrentThread.CurrentUICulture, objParams);
		}
	}
}
