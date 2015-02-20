using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using System.Collections.Specialized;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// 处理HttpRequest的辅助方法
    /// </summary>
    public static class Request
    {
        /// <summary>
        /// 得到某一项Request的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的Request的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Request的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Request的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestQueryString(string strName, string strDefault)
        {
            string str = HttpContext.Current.Request.QueryString[strName];

            return str.IsNullOrEmpty() ? strDefault : str;
        }

        /// <summary>
        /// 得到某一项Request的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <typeparam name="T">获取值的类型</typeparam>
        /// <param name="strName">所要获取的Request的数据项名称</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Request的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Request的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static T GetRequestQueryValue<T>(string strName, T defaultValue)
            where T : IConvertible
        {
            string str = GetRequestQueryString(strName, null);

            return str.IsNullOrEmpty() ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
        }

        /// <summary>
        /// 得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="strName">所要获取的Form的数据项</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替</returns>
        public static T GetRequestFormValue<T>(string strName, T defaultValue)
            where T : IConvertible
        {
            string str = GetRequestFormString(strName, null);

            return str.IsNullOrEmpty() ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
        }

        /// <summary>
        /// 得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的Form的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestFormString(string strName, string strDefault)
        {
            string str = HttpContext.Current.Request.Form[strName];

            return str.IsNullOrEmpty() ? strDefault : str;
        }

        /// <summary>
        /// 得到某一项ServerVariables数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的ServerVariables的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项ServerVariables数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestServerVariable(string strName, string strDefault)
        {
            string str = HttpContext.Current.Request.ServerVariables[strName];

            return str.IsNullOrEmpty() ? strDefault : str;
        }

        /// <summary>
        /// 得到某一项Cookies数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的Cookies的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Cookies数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestCookieString(string strName, string strDefault)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];

            string str = cookie == null ? strDefault : cookie.Value;

            return str.IsNullOrEmpty() ? strDefault : str;
        }

        /// <summary>
        /// 得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的Params的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestParamString(string strName, string strDefault)
        {
            string str = HttpContext.Current.Request.Params[strName];

            return string.IsNullOrEmpty(str) ? strDefault : str;
        }

        /// <summary>
        /// 获取客户端的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string ip = GetRequestServerVariable("HTTP_X_FORWARDED_FOR", string.Empty);

            if (!ip.IsValidIPAddress())
            {
                ip = GetRequestServerVariable("REMOTE_ADDR", string.Empty);

                if (!ip.IsValidIPAddress())
                    ip = HttpContext.Current.Request.UserHostAddress;
            }

            return ip;
        }

        /// <summary>
        /// 获取当前请求的PageRenderMode
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>PageRenderMode</returns>
        /// <remarks>获取当前请求的PageRenderMode</remarks>
        public static PageRenderMode GetRequestPageRenderMode()
        {
            string cacheKey = PageExtension.PageRenderModeQueryStringName;
            PageRenderMode mode = (PageRenderMode)HttpContext.Current.Items[cacheKey];

            if (mode == null)
            {
                string str = Request.GetRequestParamString(PageExtension.PageRenderModeQueryStringName, string.Empty);
                mode = str == string.Empty ? new PageRenderMode() : new PageRenderMode(str);
                HttpContext.Current.Items[cacheKey] = mode;
            }

            return mode;
        }

        /// <summary>
        /// 将PageRenderMode添加到当前请求ExecutionUrl，并返回Url
        /// </summary>
        /// <param name="pageRenderMode">PageRenderMode</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>将PageRenderMode添加到当前请求ExecutionUrl，并返回Url</remarks>
        public static string AppendToExecutionUrl(PageRenderMode pageRenderMode, params string[] ignoreParamNames)
        {
            return AppendToExecutionUrl(PageExtension.PageRenderModeQueryStringName, pageRenderMode.ToString(), ignoreParamNames);
        }

        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendQueryString">Url中的查询串，例如：uid=sz&amp;name=Haha</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。</remarks>
        public static string AppendToExecutionUrl(string appendQueryString, params string[] ignoreParamNames)
        {
            HttpRequest request = HttpContext.Current.Request;
            string currentUrl = request.CurrentExecutionFilePath;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;
                currentUrl = page.ResolveUrl(page.AppRelativeVirtualPath);
            }

            return GetRequestUrlInternal(currentUrl, request.QueryString, appendQueryString, ignoreParamNames);
        }

        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendParamName">添加到QueryString中的参数名称</param>
        /// <param name="appendParamValue">添加到QueryString中的参数值</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </remarks>
        public static string AppendToExecutionUrl(string appendParamName, string appendParamValue, params string[] ignoreParamNames)
        {
            ignoreParamNames = StringArrayAdd(ignoreParamNames, appendParamName);

            string appendQueryString = string.Format("{0}={1}", appendParamName, appendParamValue);

            string result = AppendToExecutionUrl(appendQueryString, ignoreParamNames);

            return result;
        }

        /// <summary>
        /// 将PageRenderMode添加到当前请求ExecutionUrl，并返回Url
        /// </summary>
        /// <param name="pageRenderMode">PageRenderMode</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>将PageRenderMode添加到当前请求ExecutionUrl，并返回Url</remarks>
        public static string GetRequestExecutionUrl(PageRenderMode pageRenderMode, params string[] ignoreParamNames)
        {
            return GetRequestExecutionUrl(PageExtension.PageRenderModeQueryStringName, pageRenderMode.ToString(), ignoreParamNames);
        }

        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendQueryString">Url中的查询串，例如：uid=sz&amp;name=Haha</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。</remarks>
        public static string GetRequestExecutionUrl(string appendQueryString, params string[] ignoreParamNames)
        {
            HttpRequest request = HttpContext.Current.Request;
            string currentUrl = request.CurrentExecutionFilePath;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;
                currentUrl = page.ResolveUrl(page.AppRelativeVirtualPath);
            }

            return GetRequestUrlInternal(currentUrl, request.QueryString, appendQueryString, ignoreParamNames);
        }

        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendParamName">添加到QueryString中的参数名称</param>
        /// <param name="appendParamValue">添加到QueryString中的参数值</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </remarks>
        public static string GetRequestExecutionUrl(string appendParamName, string appendParamValue, params string[] ignoreParamNames)
        {
            ignoreParamNames = StringArrayAdd(ignoreParamNames, appendParamName);

            string appendQueryString = string.Format("{0}={1}", appendParamName, appendParamValue);

            string result = GetRequestExecutionUrl(appendQueryString, ignoreParamNames);

            return result;
        }

        #region Private
        private static string GetRequestUrlInternal(string filePath, NameValueCollection queryString, string appendQueryString, params string[] ignoreParamNames)
        {
            string result = filePath;

            string originalQuery = GetQueryString(queryString, ignoreParamNames);

            if (originalQuery != string.Empty)
                result += "?" + originalQuery + "&" + appendQueryString;
            else
                result += "?" + appendQueryString;

            return result;
        }

        private static string GetQueryString(NameValueCollection queryString, params string[] ignoreParamKeys)
        {
            StringBuilder strB = new StringBuilder(1024);

            foreach (string key in queryString.Keys)
            {
                if (StringInCollection(key, ignoreParamKeys, true) == false)
                {
                    if (strB.Length > 0)
                        strB.Append("&");

                    strB.Append(key + "=" + queryString[key]);
                }
            }

            return strB.ToString();
        }

        private static bool StringInCollection(string strValue, IEnumerable<string> strList, bool ignoreCase)
        {
            bool bResult = false;

            StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

            foreach (string str in strList)
            {
                if (string.Equals(strValue, str, comparison))
                {
                    bResult = true;
                    break;
                }
            }

            return bResult;
        }

        private static string[] StringArrayAdd(string[] array, string addStr)
        {
            StringCollection strList = new StringCollection();
            strList.AddRange(array);
            strList.Add(addStr);
            string[] result = new string[strList.Count];
            strList.CopyTo(result, 0);

            return result;
        }
        #endregion private
    }
}
