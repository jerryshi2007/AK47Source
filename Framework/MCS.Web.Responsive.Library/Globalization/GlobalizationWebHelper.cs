using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using MCS.Library.Caching;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 
	/// </summary>
	public static class GlobalizationWebHelper
	{
		private static string LanguageCookieName = "DeluxeUserLanguage";
		/// <summary>
		/// 得到用户缺省的语言
		/// </summary>
		/// <returns></returns>
		public static string GetUserDefaultLanguage()
		{
			string language = "en-US";

			HttpRequest request = HttpContext.Current.Request;

			string savedLanguage = string.Empty;

			HttpCookie cookie = request.Cookies[LanguageCookieName];

			if (cookie != null)
				savedLanguage = HttpUtility.UrlDecode(cookie.Value);

			if (string.IsNullOrEmpty(savedLanguage))
			{
				if (request.UserLanguages.Length > 0)
				{
					CultureInfo culture = GetMatchedCulture(request.UserLanguages);

					if (culture != null)
						savedLanguage = culture.Name;
				}
			}

			if (string.IsNullOrEmpty(savedLanguage) == false)
				language = savedLanguage;

			return language;
		}

		/// <summary>
		/// 在Cookie中保存用户选择语言
		/// </summary>
		/// <param name="language"></param>
		public static void SaveUserDefaultLanguage(string language)
		{
			HttpResponse response = HttpContext.Current.Response;

			HttpCookie cookie = new HttpCookie(LanguageCookieName);

			cookie.Value = HttpUtility.UrlEncode(language);
			cookie.Expires = DateTime.MaxValue;

			response.SetCookie(cookie);
		}

		/// <summary>
		/// 得到匹配的Culture
		/// </summary>
		/// <param name="userLanguages"></param>
		/// <returns></returns>
		private static CultureInfo GetMatchedCulture(string[] userLanguages)
		{
			CultureInfo result = null;
			Dictionary<string, CultureInfo> cultures = GetSystemCultures();

			for (int i = 0; i < userLanguages.Length; i++)
			{
				string language = userLanguages[i].Split(';')[0];

				if (cultures.TryGetValue(language, out result))
					break;
			}

			return result;
		}

		/// <summary>
		/// 从Cache中得到系统所包含的Culture
		/// </summary>
		/// <returns></returns>
		private static Dictionary<string, CultureInfo> GetSystemCultures()
		{
			Dictionary<string, CultureInfo> result = (Dictionary<string, CultureInfo>)ObjectCacheQueue.Instance.GetOrAddNewValue("SystemCultures", (cache, key) =>
			{
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

				Dictionary<string, CultureInfo> item = new Dictionary<string, CultureInfo>(cultures.Length, StringComparer.OrdinalIgnoreCase);

				foreach (CultureInfo culture in cultures)
				{
					if (item.ContainsKey(culture.Name) == false)
						item.Add(culture.Name, culture);
				}

				cache.Add("SystemCultures", item);

				return item;
			});

			return result;
		}
	}
}
