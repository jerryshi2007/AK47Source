#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	UriHelper.cs
// Remark	：	提供和Uri相关处理的相关函数。这里采用静态方法的形式提供出Uri中的参数提取及Uri解析等功能。 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20080522		创建
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MCS.Library.Core
{
    /// <summary>
    /// 提供和Uri相关处理的相关函数。这里采用静态方法的形式提供出Uri中的参数提取及Uri解析等功能。 
    /// </summary>
    public static class UriHelper
    {
        private static readonly char[] _SlashArray = new char[] { '/', '\\' };

        #region Public
        /// <summary>
        /// 得到Url中不带参数的部分，也就是？左边的部分
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static string GetUrlWithoutParameters(string uriString)
        {
            string result = uriString;

            if (result != null)
            {
                int startIndex = uriString.IndexOf("?");

                if (startIndex >= 0)
                    result = uriString.Substring(0, startIndex);
                else
                    result = uriString;
            }

            return result;
        }

        /// <summary>
        /// 分析Url，得到所有的参数集合
        /// </summary>
        /// <param name="url">Uri类型的Url，绝对路径或相对路径</param>
        /// <returns>NameValueCollection，参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(this Uri url)
        {
            NameValueCollection result = null;

            if (url == null)
                result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            else
                result = GetUriParamsCollection(url.ToString());

            return result;
        }

        /// <summary>
        /// 从url中，获取参数的集合
        /// </summary>
        /// <param name="uriString">url</param>
        /// <returns>参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(string uriString)
        {
            return GetUriParamsCollection(uriString, true);
        }

        /// <summary>
        /// 从url中，获取参数的集合
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="urlDecode">是否执行decode</param>
        /// <returns>参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(string uriString, bool urlDecode)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

            string bookmarkString = GetBookmarkStringInUrl(uriString);

            if (bookmarkString != string.Empty)
                uriString = uriString.Remove(uriString.Length - bookmarkString.Length, bookmarkString.Length);

            string query = uriString;

            int startIndex = query.IndexOf("?");

            if (startIndex >= 0)
                query = query.Substring(startIndex + 1);

            if (query != string.Empty)
            {
                string[] parts = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i++)
                {
                    int equalsSignIndex = parts[i].IndexOf("=");

                    string paramName = string.Empty;
                    string paramValue = string.Empty;

                    if (equalsSignIndex >= 0)
                    {
                        //存在等号
                        paramName = parts[i].Substring(0, equalsSignIndex);
                        paramValue = parts[i].Substring(equalsSignIndex + 1);
                    }

                    if (string.IsNullOrEmpty(paramName) == false)
                    {
                        if (urlDecode)
                        {
                            paramName = HttpUtility.UrlDecode(paramName);
                            paramValue = HttpUtility.UrlDecode(paramValue);
                        }

                        AddValueToCollection(paramName, paramValue, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 将Url中的参数进行排序，返回参数排序后的url串
        /// </summary>
        /// <param name="url">Uri类型的Url，绝对路径或相对路径</param>
        /// <returns>参数排序后的url串</returns>
        public static string GetUrlWithSortedParams(this Uri url)
        {
            string result = string.Empty;

            if (url != null)
                result = GetUrlWithSortedParams(url.ToString());

            return result;
        }

        /// <summary>
        /// 将Url中的参数进行排序，返回参数排序后的url串
        /// </summary>
        /// <param name="uriString">Url，绝对路径或相对路径</param>
        /// <returns>参数排序后的url串</returns>
        public static string GetUrlWithSortedParams(string uriString)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            string query = string.Empty;
            string leftPart = string.Empty;

            int startIndex = uriString.IndexOf("?");

            if (startIndex >= 0)
            {
                leftPart = uriString.Substring(0, startIndex) + "?";
                query = uriString.Substring(startIndex + 1);
            }
            else
                leftPart = uriString;

            StringBuilder strB = new StringBuilder(2048);

            if (query != string.Empty)
            {
                NameValueCollection paramCollection = GetUriParamsCollection(query);
                string[] allKeys = paramCollection.AllKeys;

                Array.Sort(allKeys, System.Collections.CaseInsensitiveComparer.Default);

                for (int i = 0; i < allKeys.Length; i++)
                {
                    string key = allKeys[i];

                    if (strB.Length > 0)
                        strB.Append("&");

                    strB.Append(HttpUtility.UrlEncode(key));

                    if (key != string.Empty)
                        strB.Append("=");

                    strB.Append(HttpUtility.UrlEncode(paramCollection[key]));
                }
            }

            return leftPart + strB.ToString();
        }

        /// <summary>
        /// 移除Url中指定的参数
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static string RemoveUriParams(string uriString, params string[] paramNames)
        {
            return RemoveUriParams(uriString, Encoding.UTF8, paramNames);
        }

        /// <summary>
        /// 移除Url中指定的参数
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="encoding"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static string RemoveUriParams(string uriString, Encoding encoding, params string[] paramNames)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");
            ExceptionHelper.FalseThrow<ArgumentNullException>(encoding != null, "encoding");
            ExceptionHelper.FalseThrow<ArgumentNullException>(paramNames != null, "paramNames");

            StringBuilder strB = new StringBuilder(1024);

            NameValueCollection originalParams = GetUriParamsCollection(uriString);

            foreach (string paramName in paramNames)
            {
                if (originalParams[paramName] != null)
                    originalParams.Remove(paramName);
            }

            return CombineUrlParams(uriString, encoding, originalParams);
        }

        /// <summary>
        /// 移除Url中指定的参数
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static Uri RemoveUriParams(this Uri uriString, params string[] paramNames)
        {
            return RemoveUriParams(uriString, Encoding.UTF8, paramNames);
        }

        /// <summary>
        /// 移除Url中指定的参数
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="encoding"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static Uri RemoveUriParams(this Uri uriString, Encoding encoding, params string[] paramNames)
        {
            Uri result = uriString;

            if (uriString != null)
                result = new Uri(RemoveUriParams(uriString.ToString(), encoding, paramNames), UriKind.RelativeOrAbsolute);

            return result;
        }

        /// <summary>
        /// 替换Url中的参数。将参数变成NameValueCollection后提供给调用者，由调用者提供的替换方式
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="action"></param>
        /// <returns>返回替换参数后的Url</returns>
        public static string ReplaceUriParams(string uri, Action<NameValueCollection> action)
        {
            return ReplaceUriParams(uri, Encoding.UTF8, action);
        }

        /// <summary>
        /// 替换Url中的参数。将参数变成NameValueCollection后提供给调用者，由调用者提供的替换方式
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="encodeParams">是否使用UTF编码替换参数</param>
        /// <param name="action"></param>
        /// <returns>返回替换参数后的Url</returns>
        public static string ReplaceUriParams(string uri, bool encodeParams, Action<NameValueCollection> action)
        {
            string result = uri;

            if (encodeParams)
                result = ReplaceUriParams(uri, Encoding.UTF8, action);
            else
                result = ReplaceUriParams(uri, null, action);

            return result;
        }

        /// <summary>
        /// 替换Url中的参数。将参数变成NameValueCollection后提供给调用者，由调用者提供的替换方式
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="encoding">编码方式，如果是null，则不编码</param>
        /// <param name="action"></param>
        /// <returns>返回替换参数后的Url</returns>
        public static string ReplaceUriParams(string uri, Encoding encoding, Action<NameValueCollection> action)
        {
            string result = uri;

            if (uri.IsNotEmpty())
            {
                NameValueCollection uriParams = GetUriParamsCollection(uri);

                if (action != null)
                    action(uriParams);

                result = UriHelper.CombineUrlParams(result, encoding, uriParams);
            }

            return result;
        }

        /// <summary>
        /// 将当前的Url转换成绝对路径的Url，这个地址相对于refUri的地址
        /// </summary>
        /// <param name="thisUri"></param>
        /// <param name="refUri"></param>
        /// <returns></returns>
        public static Uri MakeAbsolute(this Uri thisUri, Uri refUri)
        {
            Uri result = thisUri;

            if (thisUri != null && refUri != null)
            {
                string relativePath = thisUri.ToString();

                if (thisUri.IsAbsoluteUri)
                    relativePath = thisUri.LocalPath;

                string dir = string.Empty;

                if (relativePath.IndexOf('/') == 0)
                {
                    if (refUri.IsAbsoluteUri)
                        dir = refUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

                    result = new Uri(CombinePath(dir, relativePath), UriKind.RelativeOrAbsolute);
                }
                else
                {
                    result = refUri.MergePath(new Uri(relativePath, UriKind.RelativeOrAbsolute));
                }
            }

            return result;
        }

        /// <summary>
        /// 将参数重新组合成Url
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="encoding">字符编码，如果为null，表示不用Encode</param>
        /// <param name="requestParamsArray">参数集合的数组</param>
        /// <returns>补充了参数的url</returns>
        public static string CombineUrlParams(string uriString, Encoding encoding, params NameValueCollection[] requestParamsArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            ExceptionHelper.FalseThrow<ArgumentNullException>(requestParamsArray != null, "requestParamsArray");

            NameValueCollection requestParams = MergeParamsCollection(requestParamsArray);

            StringBuilder strB = new StringBuilder(1024);

            string leftPart = GetUrlWithoutParameters(uriString);

            for (int i = 0; i < requestParams.Count; i++)
            {
                if (i == 0)
                    strB.Append("?");
                else
                    strB.Append("&");

                //徐磊修改  2012/3/5
                if (encoding == null)
                {
                    strB.Append(requestParams.Keys[i]);
                    strB.Append("=");
                    strB.Append(requestParams[i]);
                }
                else
                {
                    strB.Append(HttpUtility.UrlEncode(requestParams.Keys[i], encoding));
                    strB.Append("=");
                    strB.Append(HttpUtility.UrlEncode(requestParams[i], encoding));
                }
            }

            return leftPart + strB.ToString();
        }

        /// <summary>
        /// 将参数重新组合成Url
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="requestParamsArray">参数集合的数组</param>
        /// <returns>补充了参数的url</returns>
        public static string CombineUrlParams(string uriString, params NameValueCollection[] requestParamsArray)
        {
            return CombineUrlParams(uriString, Encoding.UTF8, requestParamsArray);
        }

        /// <summary>
        /// 是否需要Encode
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="encodeParams"></param>
        /// <param name="requestParamsArray"></param>
        /// <returns></returns>
        public static string CombineUrlParams(string uriString, bool encodeParams, params NameValueCollection[] requestParamsArray)
        {
            string result = string.Empty;

            if (encodeParams)
                result = CombineUrlParams(uriString, Encoding.UTF8, requestParamsArray);
            else
                result = CombineUrlParams(uriString, null, requestParamsArray);

            return result;
        }

        /// <summary>
        /// 得到url中的书签部分。“#”后面的部分
        /// </summary>
        /// <param name="queryString">http://localhost/lianhome#littleTurtle</param>
        /// <returns>littleTurtle</returns>
        public static string GetBookmarkStringInUrl(string queryString)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(queryString != null, "queryString");

            int bookmarkStart = -1;

            for (int i = queryString.Length - 1; i >= 0; i--)
            {
                if (queryString[i] == '#')
                    bookmarkStart = i;
                else
                    if (queryString[i] == '&' || queryString[i] == '?')
                        break;
            }

            string result = string.Empty;

            if (bookmarkStart >= 0)
                result = queryString.Substring(bookmarkStart);

            return result;
        }

        /// <summary>
        /// 解析Uri，如果Uri为相对路径，处理Uri中~，将其替换为当前的Web应用
        /// </summary>
        /// <param name="uriString">Uri</param>
        /// <returns>如果Uri为相对路径，处理Uri中~，将其替换为当前的Web应用</returns>
        public static Uri ResolveUri(string uriString)
        {
            uriString.NullCheck("uriString");

            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web)
                {
                    if (uriString[0] == '~')
                    {
                        HttpRequest request = HttpContext.Current.Request;
                        string appPathAndQuery = request.ApplicationPath + uriString.Substring(1);

                        appPathAndQuery = appPathAndQuery.Replace("//", "/");

                        uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
                                    appPathAndQuery;

                        url = new Uri(uriString);
                    }
                }
            }

            return url;
        }

        /// <summary>
        /// 解析Uri，如果Uri为相对路径，处理Uri中~，将其替换为当前的Web应用相对路径
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static string ResolveRelativeUri(string uriString)
        {
            uriString.NullCheck("uriString");

            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            string result = uriString;

            if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web)
                {
                    if (uriString[0] == '~')
                    {
                        HttpRequest request = HttpContext.Current.Request;
                        string appPathAndQuery = request.ApplicationPath + uriString.Substring(1);

                        result = appPathAndQuery.Replace("//", "/");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 得到Url中的目录部分。从后面数第一个斜线的左侧部分，包括斜线本身
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static string GetDiractoryUri(string uriString)
        {
            string result = string.Empty;

            if (uriString != null)
            {
                int index = uriString.LastIndexOfAny(UriHelper._SlashArray);

                if (index >= 0)
                    result = uriString.Substring(0, index + 1);
            }

            return result;
        }

        /// <summary>
        /// 得到Url中的目录部分。从后面数第一个斜线的左侧部分，包括斜线本身。如果uriString为null，则返回地址为空串的uri。
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static Uri GetDiractoryUri(this Uri uriString)
        {
            string resultString = string.Empty;

            if (uriString != null)
                resultString = GetDiractoryUri(uriString.ToString());

            return new Uri(resultString, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 得到Uri中所有的目录
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static string[] GetAllDirectories(this Uri uriString)
        {
            List<string> result = new List<string>();

            if (uriString != null)
            {
                string relativePath = uriString.ToString();

                if (uriString.IsAbsoluteUri)
                    relativePath = uriString.LocalPath;

                string[] dirs = relativePath.Split(UriHelper._SlashArray);

                for (int i = 0; i < dirs.Length; i++)
                {
                    if (dirs[i] != string.Empty && i < dirs.Length - 1)
                        result.Add(dirs[i]);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 组合不同的Uri，将两个Url之间合并在一起，并且去除它们之间的双斜线
        /// </summary>
        /// <param name="uriString1"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static Uri CombinePath(this Uri uriString1, params Uri[] strings)
        {
            strings.NullCheck("strings");

            string baseUrl = string.Empty;

            if (uriString1 != null)
                baseUrl = uriString1.ToString();

            StringBuilder strB = new StringBuilder();

            strB.Append(baseUrl.TrimEnd('\\', '/'));

            foreach (Uri part in strings)
            {
                strB.Append("/" + part.ToString().Trim('\\', '/'));
            }

            return new Uri(strB.ToString(), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 组合不同的Uri，去除它们之间的双斜线
        /// </summary>
        /// <param name="uriString1"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string CombinePath(string uriString1, params string[] strings)
        {
            strings.NullCheck("strings");

            if (uriString1 == null)
                uriString1 = string.Empty;

            StringBuilder strB = new StringBuilder();

            strB.Append(uriString1.TrimEnd('\\', '/'));

            foreach (string part in strings)
            {
                strB.Append("/" + part.Trim('\\', '/'));
            }

            return strB.ToString();
        }

        /// <summary>
        /// 合并多个Url，并且根据..或.进行目录合并
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relUri"></param>
        /// <returns></returns>
        public static Uri MergePath(this Uri baseUri, Uri relUri)
        {
            StringBuilder strB = new StringBuilder();

            Uri result = baseUri;

            if (baseUri != null && relUri != null)
            {
                if (baseUri.IsAbsoluteUri)
                    strB.Append(baseUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));

                string[] baseDirs = GetAllDirectories(baseUri);

                List<string> mergedDirs = new List<string>(baseDirs);

                MergeDirs(mergedDirs, relUri.ToString());

                foreach (string dir in mergedDirs)
                    strB.Append("/" + dir);

                result = new Uri(strB.ToString(), UriKind.RelativeOrAbsolute);
            }

            return result;
        }

        /// <summary>
        /// 直接转成字符串，可以保证uri为null的时候转换成空串
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string ToUriString(this Uri uri)
        {
            string result = string.Empty;

            if (uri != null)
                result = uri.ToString();

            return result;
        }

        #region Compare
        /// <summary>
        /// 比较两个Url的scheme部分host部分和port部分
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <returns></returns>
        public static bool CompareSchemeAndHost(this Uri uri1, Uri uri2)
        {
            bool hasNull = false;

            bool result = uri1.ReferenceEqualWithNull(uri2, out hasNull);

            if (result == false)
            {
                if (hasNull == false)
                {
                    result = uri1.IsAbsoluteUri == uri2.IsAbsoluteUri;

                    if (result)
                    {
                        if (uri1.IsAbsoluteUri)
                            result = (uri1.Scheme.IgnoreCaseCompare(uri2.Scheme) == 0).
                                TrueFunc(() => uri1.Host.IgnoreCaseCompare(uri2.Host) == 0).
                                TrueFunc(() => uri1.Port == uri2.Port);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 比较两个url的路径和参数部分，不包含host，scheme和port。两个Uri必须都是绝对路径，如果有一个为相对，则返回false。两个都是，则抛出异常
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <param name="ignoreParameters">需要忽略的参数名称</param>
        /// <returns></returns>
        public static bool ComparePathAndParameters(this Uri uri1, Uri uri2, params string[] ignoreParameters)
        {
            bool hasNull = false;

            bool result = uri1.ReferenceEqualWithNull(uri2, out hasNull);

            if (result == false)
            {
                if (hasNull == false)
                {
                    string path1 = string.Empty;
                    string path2 = string.Empty;

                    if (uri1.IsAbsoluteUri)
                        path1 = "/" + uri1.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
                    else
                        path1 = GetUrlWithoutParameters(uri1.ToString());

                    if (uri2.IsAbsoluteUri)
                        path2 = "/" + uri2.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
                    else
                        path2 = GetUrlWithoutParameters(uri2.ToString());

                    result = (path1.IgnoreCaseCompare(path2) == 0).TrueFunc(() => CompareParameters(uri1, uri2, ignoreParameters));
                }
            }

            return result;
        }

        /// <summary>
        /// 是否是相同的Uri。两者都是null，则返回true
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <param name="ignoreParameters"></param>
        /// <returns></returns>
        public static bool AreSameUri(this Uri uri1, Uri uri2, params string[] ignoreParameters)
        {
            return CompareSchemeAndHost(uri1, uri2).TrueFunc(() => uri1.ComparePathAndParameters(uri2, ignoreParameters));
        }
        #endregion

        #endregion

        #region Private
        private static bool CompareParameters(Uri uri1, Uri uri2, params string[] ignoreParameters)
        {
            NameValueCollection params1 = uri1.GetUriParamsCollection();
            NameValueCollection params2 = uri2.GetUriParamsCollection();

            params1.RemoveKeys(ignoreParameters);
            params2.RemoveKeys(ignoreParameters);

            bool result = params1.Count == params2.Count;

            if (result)
            {
                foreach (string key in params1.AllKeys)
                {
                    if (string.Compare(params1[key], params2[key], true) != 0)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private static string RemoveLastSplash(string path)
        {
            string result = path;

            if (path.Length > 0)
            {
                char lastChar = path[path.Length - 1];

                if (lastChar == '/' || lastChar == '\\')
                {
                    result = path.Substring(0, path.Length - 1);
                }
            }

            return result;
        }

        private static NameValueCollection MergeParamsCollection(NameValueCollection[] requestParams)
        {
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < requestParams.Length; i++)
                MergeTwoParamsCollection(result, requestParams[i]);

            return result;
        }

        private static void MergeTwoParamsCollection(NameValueCollection target, NameValueCollection src)
        {
            foreach (string key in src.Keys)
            {
                if (target[key] == null)
                    target.Add(key, src[key]);
            }
        }

        private static void AddValueToCollection(string paramName, string paramValue, NameValueCollection result)
        {
            string oriValue = result[paramName];

            if (oriValue == null)
                result.Add(paramName, paramValue);
            else
            {
                string rValue = oriValue;

                if (oriValue.Length > 0)
                    rValue += ",";

                rValue += paramValue;

                result[paramName] = rValue;
            }
        }

        private static void MergeDirs(List<string> mergedDirs, string relUri)
        {
            string[] relDirs = relUri.ToString().Split(UriHelper._SlashArray, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < relDirs.Length; i++)
            {
                switch (relDirs[i])
                {
                    case ".":
                        break;
                    case "..":
                        (mergedDirs.Count > 0).FalseThrow("相对目录{0}已经超出了根目录的范围", relUri);
                        mergedDirs.RemoveAt(mergedDirs.Count - 1);
                        break;
                    default:
                        mergedDirs.Add(relDirs[i]);
                        break;
                }
            }
        }
        #endregion
    }
}
