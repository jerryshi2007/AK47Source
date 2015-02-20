using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Library.Core
{
    /// <summary>
    /// Cookie相关的辅助方法
    /// </summary>
    public static class CookieHelper
    {
        /// <summary>
        /// 得到Http请求中的Cookie值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetRequestCookieValue<T>(string key, T defaultValue)
        {
            key.CheckStringIsNullOrEmpty("key");

            T result = defaultValue;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
            {
                HttpRequest request = HttpContext.Current.Request;

                HttpCookie cookie = request.Cookies[key];

                if (cookie != null)
                {
                    string cookieValue = DecodeCookieValue(cookie.Value);
                    result = (T)DataConverter.ChangeType(cookieValue, typeof(T));
                }
            }

            return result;
        }

        /// <summary>
        /// 在Response中创建Cookie，并且设置它的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpCookie SetResponseCookieValue<T>(string key, T data)
        {
            key.CheckStringIsNullOrEmpty("key");

            (EnvironmentHelper.Mode == InstanceMode.Web).FalseThrow("只有Web应用且能够获得HttpContext才可以调用此方法");

            HttpResponse response = HttpContext.Current.Response;

            HttpCookie cookie = new HttpCookie(key);

            if (data != null)
            {
                string cookieValue = (string)DataConverter.ChangeType(data, typeof(string));
                cookie.Value = EncodeCookieValue(cookieValue);
            }

            cookie.Expires = DateTime.MinValue;

            HttpContext.Current.Response.Cookies.Add(cookie);

            return cookie;
        }

        private static string EncodeCookieValue(string cookieValue)
        {
            return HttpUtility.UrlEncode(cookieValue);
        }

        private static string DecodeCookieValue(string cookieValue)
        {
            return HttpUtility.UrlDecode(cookieValue);
        }
    }
}
