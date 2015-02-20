using MCS.Library.Caching;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace MCS.Web.Library
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalizationWebHelper
    {
        private const string LanguageCookieName = "DeluxeUserLanguage";

        /// <summary>
        /// url中，表示语言的参数名
        /// </summary>
        public const string LanguageParameterName = "lang";

        /// <summary>
        /// 从Url的参数中获取文种信息
        /// </summary>
        /// <param name="language">默认和返回的文种</param>
        /// <returns>url中是否包含了文种参数</returns>
        public static bool TryGetLanguageFromQueryString(ref string language)
        {
            bool result = false;

            if (EnvironmentHelper.IsUsingWebConfig && HttpContext.Current != null)
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.QueryString[LanguageParameterName].IsNotEmpty())
                {
                    language = request.QueryString[LanguageParameterName];
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 如果当前的Handler是页面，则从页面中获取language信息
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHandlerLanguageID()
        {
            string language = string.Empty;

            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;

            if (culture != null)
                language = culture.Name;

            return language;
        }

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
