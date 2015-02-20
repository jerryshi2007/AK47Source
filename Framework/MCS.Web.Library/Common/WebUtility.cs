using System;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.Script;
using System.Reflection;
using System.Web.Compilation;
using System.Globalization;
using System.Xml;
using System.Resources;

using MCS.Library.Core;

using MCS.Web.Library.Resources;
using MCS.Web.Library.Script;
using System.Web.Configuration;
using System.Configuration;
using MCS.Library.Globalization;

namespace MCS.Web.Library
{
	/// <summary>
	/// Web帮助类
	/// </summary>
	/// <remarks>Web帮助类</remarks>
	public static class WebUtility
	{
		//private static readonly string PageRenderModeQueryStringName = "PageRenderMode" + (new object()).GetHashCode().ToString();
		/// <summary>
		/// 沈峥注释，改为常量
		/// </summary>
		private static readonly string PageRenderModeQueryStringName = "PageRenderMode";
		internal static object PageRenderControlItemKey = new object();

		/// <summary>
		/// 根据ContentTypeKey，得到Response的ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>根据ContentTypeKey，得到Response的ContentType</remarks>
		public static string GetContentTypeByKey(string key)
		{
			ContentTypesSection section = ConfigSectionFactory.GetContentTypesSection();

			key = key.ToLower();
			ContentTypeConfigElement elt = section.ContentTypes[key];

			string contentType = elt != null ? elt.ContentType : string.Empty;

			return contentType;
		}

		/// <summary>
		/// 根据ContentTypeKey，得到Response的ContentType，如果值为空则返回默认key的ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <param name="defaultKey">默认ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>根据ContentTypeKey，得到Response的ContentType，如果值为空则返回默认key的ContentType</remarks>
		public static string GetContentTypeByKey(string key, string defaultKey)
		{
			string contentType = GetContentTypeByKey(key);
			if (contentType == string.Empty) contentType = GetContentTypeByKey(defaultKey);

			return contentType;
		}

		/// <summary>
		/// 根据ContentTypeKey，得到Response的ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>根据ContentTypeKey，得到Response的ContentType</remarks>
		public static string GetContentTypeByKey(ResponseContentTypeKey key)
		{
			return GetContentTypeByKey(key.ToString());
		}

		/// <summary>
		/// 根据文件名，得到Response的ContentType
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <returns>ContentType</returns>
		/// <remarks>根据文件名，得到Response的ContentType</remarks>
		public static string GetContentTypeByFileName(string fileName)
		{
			string fileExtesionName = GetFileExtesionName(fileName);

			return GetContentTypeByFileExtesionName(fileExtesionName);
		}

		private static string GetFileExtesionName(string fileName)
		{
			string fileExtesionName = Path.GetExtension(fileName);

			return string.IsNullOrEmpty(fileExtesionName) ? fileExtesionName : fileExtesionName.Substring(1);
		}

		/// <summary>
		/// 根据文件扩展名，得到Response的ContentType
		/// </summary>
		/// <param name="fileExtesionName">文件扩展名</param>
		/// <returns>ContentType</returns>
		/// <remarks>根据文件扩展名，得到Response的ContentType</remarks>
		public static string GetContentTypeByFileExtesionName(string fileExtesionName)
		{
			ContentTypesSection section = ConfigSectionFactory.GetContentTypesSection();

			foreach (ContentTypeConfigElement elt in section.ContentTypes)
			{
				if (StringInCollection(fileExtesionName, elt.FileExtensionNames, true))
				{
					return elt.ContentType;
				}
			}

			return string.Empty;
		}

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

			return string.IsNullOrEmpty(str) ? strDefault : str;
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
			return str == null ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
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
			return str == null ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
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

			return string.IsNullOrEmpty(str) ? strDefault : str;
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

			return string.IsNullOrEmpty(str) ? strDefault : str;
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
			return string.IsNullOrEmpty(str) ? strDefault : str;
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
		/// 获取当前请求的PageRenderMode
		/// </summary>
		/// <returns>PageRenderMode</returns>
		/// <remarks>获取当前请求的PageRenderMode</remarks>
		public static PageRenderMode GetRequestPageRenderMode()
		{
			HttpContext context = HttpContext.Current;
			string cacheKey = PageRenderModeQueryStringName;
			PageRenderMode mode = (PageRenderMode)context.Items[cacheKey];

			if (mode == null)
			{
				string str = GetRequestParamString(PageRenderModeQueryStringName, string.Empty);
				mode = str == string.Empty ? new PageRenderMode() : new PageRenderMode(str);
				context.Items[cacheKey] = mode;
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
		public static string GetRequestExecutionUrl(PageRenderMode pageRenderMode, params string[] ignoreParamNames)
		{
			return GetRequestExecutionUrl(PageRenderModeQueryStringName, pageRenderMode.ToString(), ignoreParamNames);
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

		/// <summary>
		/// 根据当前的HttpRequest中Url的，增加queryString，并返回新的Url。
		/// </summary>
		/// <param name="appendQueryString">Url中的查询串，例如：uid=sz&amp;name=Haha</param>
		/// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
		/// <returns>结果Url</returns>
		/// <remarks>根据当前的HttpRequest中Url的，增加queryString，并返回新的Url。</remarks>
		public static string GetRequestUrl(string appendQueryString, params string[] ignoreParamNames)
		{
			HttpRequest request = HttpContext.Current.Request;

			string result = GetRequestUrlInternal(request.FilePath, request.QueryString, appendQueryString, ignoreParamNames);

			return result;
		}

		/// <summary>
		/// 如果当前的HtppHandler是Page，判断它是否是PostBack状态
		/// </summary>
		/// <returns>是否是PostBack状态</returns>
		public static bool IsCurrentHandlerPostBack()
		{
			bool result = false;

			if (HttpContext.Current.CurrentHandler is Page)
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				PropertyInfo pi = page.GetType().GetProperty("IsPostBack", BindingFlags.Instance | BindingFlags.Public);

				if (pi != null)
					result = (bool)pi.GetValue(page, null);
			}

			return result;
		}

		/// <summary>
		/// 如果当前的HtppHandler是Page，判断它是否是Callback状态
		/// </summary>
		/// <returns>是否是Callback状态</returns>
		public static bool IsCurrentHandlerIsCallback()
		{
			bool result = false;

			if (HttpContext.Current.CurrentHandler is Page)
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				PropertyInfo pi = page.GetType().GetProperty("IsCallback", BindingFlags.Instance | BindingFlags.Public);

				if (pi != null)
					result = (bool)pi.GetValue(page, null);
			}

			return result;
		}

		/// <summary>
		/// 是否为输出指定控件页面
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public static bool IsRenderSpecialControlPage(Page page)
		{
			Control ctr = page.Items[PageRenderControlItemKey] as Control;

			return ctr != null;
		}

		/// <summary>
		/// 如果当前的HtppHandler是Page，那么找到其ViewState属性，获取其中的值
		/// </summary>
		/// <param name="key">ViewState的key</param>
		/// <returns>ViewState中的对象</returns>
		public static object LoadViewStateFromCurrentHandler(string key)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

			object result = null;

			if (HttpContext.Current.CurrentHandler is Page)
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

				if (pi != null)
				{
					StateBag vs = (StateBag)pi.GetValue(page, null);

					if (vs != null)
						result = vs[key];
				}
			}

			return result;
		}

		/// <summary>
		/// 如果当前的HtppHandler是Page，那么将数据存入到ViewState中
		/// </summary>
		/// <param name="key">ViewState的键值</param>
		/// <param name="data">需要存入的数据</param>
		public static void SaveViewStateToCurrentHandler(string key, object data)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

			if (HttpContext.Current.CurrentHandler is Page)
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

				if (pi != null)
				{
					StateBag vs = (StateBag)pi.GetValue(page, null);

					if (vs != null)
						vs[key] = data;
				}
			}
		}

		/// <summary>
		/// 检查脚本中的字符串，替换掉双引号和回车
		/// </summary>
		/// <param name="strData">字符串</param>
		/// <returns>替换后的结果</returns>
		public static string CheckScriptString(string strData)
		{
			return CheckScriptString(strData, true);
		}

		/// <summary>
		/// 检查脚本中的字符串，替换掉双引号和回车
		/// </summary>
		/// <param name="strData">字符串</param>
		/// <param name="changeCRToBR">是否将回车替换成Html的BR</param>
		/// <returns>替换后的结果</returns>
		public static string CheckScriptString(string strData, bool changeCRToBR)
		{
			if (strData.IsNotEmpty())
			{
				strData = strData.Replace("\\", "\\\\");
				strData = strData.Replace("\"", "\\\"");
				strData = strData.Replace("/", "\\/");
				strData = strData.Replace("\r\n", "\\n");
				strData = strData.Replace("\n\r", "\\n");
				strData = strData.Replace("\n", "\\n");
				strData = strData.Replace("\r", "\\n");

				if (changeCRToBR)
					strData = strData.Replace("\\n", "<br/>");
			}

			return strData;
		}

		/// <summary>
		/// 输出带ScriptTag的脚本
		/// </summary>
		/// <param name="response"></param>
		/// <param name="action"></param>
		public static void ResponseWithScriptTag(this HttpResponse response, Action<TextWriter> action)
		{
			if (response != null)
			{
				response.Write("<script type=\"text/javascript\">\n");

				StringBuilder strB = new StringBuilder();
				using (StringWriter writer = new StringWriter(strB))
				{
					if (action != null)
						action(writer);
				}

				response.Write(strB.ToString());
				response.Write("</script>\n");
				response.Flush();
			}
		}

		#region ClientMsg
		/// <summary>
		/// 通过ScriptManager.RegisterClientScriptBlock注册客户端提示信息的脚本
		/// </summary>
		/// <param name="strMessage"></param>
		/// <param name="strDetail"></param>
		/// <param name="strTitle"></param>
		public static void RegisterClientMessage(string strMessage, string strDetail, string strTitle)
		{
			Page page = GetCurrentPage();

			ScriptManager.RegisterClientScriptBlock(page, typeof(WebUtility), "ClientMsg",
				string.Format("$HGRootNS.ClientMsg.inform(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle)), true);
		}

		/// <summary>
		/// 通过ScriptManager.RegisterClientScriptBlock注册客户端错误信息的脚本
		/// </summary>
		/// <param name="ex"></param>
		public static void RegisterClientErrorMessage(System.Exception ex)
		{
			ex.NullCheck("ex");

			string detail = string.Empty;

			if (AllowResponseExceptionStackTrace())
			{
				detail = EnvironmentHelper.GetEnvironmentInfo();
				detail += "\r\n" + ex.StackTrace;
			}

			RegisterClientErrorMessage(ex.Message, detail, Translator.Translate(Define.DefaultCategory, "错误"));
		}

		/// <summary>
		/// 通过ScriptManager.RegisterClientScriptBlock注册客户端错误信息的脚本
		/// </summary>
		/// <param name="strMessage"></param>
		/// <param name="strDetail"></param>
		/// <param name="strTitle"></param>
		public static void RegisterClientErrorMessage(string strMessage, string strDetail, string strTitle)
		{
			Page page = GetCurrentPage();

			WebApplicationExceptionExtension.TryWriteAppLog(strMessage, strDetail);

			ScriptManager.RegisterStartupScript(page, typeof(WebUtility), "ClientMsg",
				string.Format("$HGRootNS.ClientMsg.stop(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle)), true);
		}

		/// <summary>
		/// 客户端弹出提示框
		/// </summary>
		/// <param name="strMessage">提示框消息</param>
		/// <param name="strDetail">提示框详细信息</param>
		/// <param name="strTitle">提示框Title</param>
		public static void ShowClientMessage(string strMessage, string strDetail, string strTitle)
		{
			Page page = GetCurrentPage();

			RegisterClientMessageScript(page);
			RegisterOnLoadScriptBlock(page,
				string.Format("$HGRootNS.ClientMsg.inform(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle)));
		}

		/// <summary>
		/// Response客户端弹出提示框
		/// </summary>
		/// <param name="strMessage">提示框消息</param>
		/// <param name="strDetail">提示框详细信息</param>
		/// <param name="strTitle">提示框Title</param>
		public static void ResponseShowClientMessageScriptBlock(string strMessage, string strDetail, string strTitle)
		{
			ResponseClientMessageCommonScriptBlock();

			string script = string.Format("$HGRootNS.ClientMsg.inform(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle));
			script = DeluxeClientScriptManager.AddScriptTags(script);

			HttpContext.Current.Response.Write(script);
		}

		/// <summary>
		/// 得到客户端弹出对话框的脚本
		/// </summary>
		/// <param name="strMessage"></param>
		/// <param name="strDetail"></param>
		/// <param name="strTitle"></param>
		/// <returns></returns>
		public static string GetShowClientErrorScript(string strMessage, string strDetail, string strTitle)
		{
			return string.Format("$HGRootNS.ClientMsg.stop(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle));
		}

		/// <summary>
		/// 客户端弹出错误框
		/// </summary>
		/// <param name="ex"></param>
		public static void ShowClientError(System.Exception ex)
		{
			ex.NullCheck("ex");

			string detail = string.Empty;

			if (AllowResponseExceptionStackTrace())
			{
				detail = EnvironmentHelper.GetEnvironmentInfo();
				detail += "\r\n" + ex.StackTrace;
			}

			ShowClientError(ex.Message, detail, Translator.Translate(Define.DefaultCategory, "错误"));
		}

		/// <summary>
		/// 客户端弹出错误框
		/// </summary>
		/// <param name="strMessage">错误框消息</param>
		/// <param name="strDetail">错误框详细信息</param>
		/// <param name="strTitle">错误框Title</param>
		public static void ShowClientError(string strMessage, string strDetail, string strTitle)
		{
			Page page = GetCurrentPage();

			if (AllowResponseExceptionStackTrace() == false)
				strDetail = string.Empty;

			WebApplicationExceptionExtension.TryWriteAppLog(strMessage, strDetail);

			RegisterClientMessageScript(page);
			RegisterOnLoadScriptBlock(page,
				string.Format("$HGRootNS.ClientMsg.stop(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle)));

			WebApplicationExceptionExtension.TryWriteAppLog(strMessage, strDetail);
		}

		/// <summary>
		/// 客户端弹出错误框
		/// </summary>
		/// <param name="ex"></param>
		public static void ResponseShowClientErrorScriptBlock(System.Exception ex)
		{
			ex.NullCheck("ex");

			ResponseShowClientErrorScriptBlock(ex.Message, ex.StackTrace, Translator.Translate(Define.DefaultCategory, "错误"));
		}

		/// <summary>
		/// Response客户端弹出错误框
		/// </summary>
		/// <param name="strMessage">错误框消息</param>
		/// <param name="strDetail">错误框详细信息</param>
		/// <param name="strTitle">错误框Title</param>
		public static void ResponseShowClientErrorScriptBlock(string strMessage, string strDetail, string strTitle)
		{
			ResponseClientMessageCommonScriptBlock();

			if (AllowResponseExceptionStackTrace() == false)
				strDetail = string.Empty;

			string script = string.Format("$HGRootNS.ClientMsg.stop(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle));

			script = DeluxeClientScriptManager.AddScriptTags(script);

			WebApplicationExceptionExtension.TryWriteAppLog(strMessage, strDetail);

			HttpContext.Current.Response.Write(script);
		}

		/// <summary>
		/// 客户端弹出确认框
		/// </summary>
		/// <param name="strMessage">提示框消息</param>
		/// <param name="strDetail">提示框详细信息</param>
		/// <param name="strTitle">提示框Title</param>
		/// <param name="okBtnText">确定按钮文本</param>
		/// <param name="cancelBtnText">取消按钮文本</param>
		/// <param name="onOKClientEventHandler">确认后客户端响应方法</param>
		/// <param name="onCancelClientEventHandler">取消后客户端响应方法</param>
		public static void ShowClientConfirm(string strMessage, string strDetail, string strTitle, string okBtnText, string cancelBtnText, string onOKClientEventHandler, string onCancelClientEventHandler)
		{
			Page page = GetCurrentPage();

			RegisterClientMessageScript(page);

			string script = string.Format("$HGRootNS.ClientMsg.confirm(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\",{5},{6});",
				CheckScriptString(strMessage),
				CheckScriptString(strDetail),
				CheckScriptString(strTitle),
				CheckScriptString(okBtnText),
				CheckScriptString(cancelBtnText),
				CheckScriptString(string.IsNullOrEmpty(onOKClientEventHandler) ? "null" : onOKClientEventHandler),
				CheckScriptString(string.IsNullOrEmpty(onCancelClientEventHandler) ? "null" : onCancelClientEventHandler));

			RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// 注册客户端弹出提示框脚本
		/// </summary>
		public static void RegisterClientMessageScript()
		{
			RegisterClientMessageScript(GetCurrentPage());
		}
		#endregion ClientMsg

		/// <summary>
		/// 获取当前页
		/// </summary>
		/// <returns></returns>
		public static Page GetCurrentPage()
		{
			Page page = (Page)HttpContext.Current.Items[_CurrentPageKey];
			if (page == null)
				page = HttpContext.Current.CurrentHandler as Page;

			//ExceptionHelper.TrueThrow(page == null, "当前没有处理请求的页面！");

			return page;
		}

		/// <summary>
		/// 设置当前页
		/// </summary>
		/// <param name="page"></param>
		public static void SetCurrentPage(Page page)
		{
			HttpContext.Current.Items[_CurrentPageKey] = page;
		}

		/// <summary>
		/// 调整窗口大小和位置
		/// </summary>
		/// <param name="windowFeature"></param>
		public static void AdjustWindow(IWindowFeature windowFeature)
		{
			RequiredScript(typeof(DeluxeScript));

			string clintObject = WindowFeatureHelper.GetClientObject(windowFeature);
			string script = string.Format("$HGRootNS.WindowFeatureFunction.adjustWindow({0});", clintObject);

			Page page = GetCurrentPage();
			DeluxeClientScriptManager.RegisterStartupScript(page, script);
		}

		#region WindowCommand

		/// <summary>
		/// 关闭窗口
		/// </summary>
		public static void CloseWindow()
		{
			RequireWindowCommandScript();
			string script = "$HGRootNS.WindowCommand.executeCommand('close');";
			Page page = GetCurrentPage();
			RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// 直接Response出关闭窗口脚本，然后结束Response
		/// </summary>
		public static void ResponseCloseWindowScriptBlock()
		{
			ResponseRequireWindowCommandScriptBlock();

			string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.executeCommand('close');");
			HttpContext.Current.Response.Write(script);
			HttpContext.Current.Response.End();
		}

		/// <summary>
		/// 直接Response输出window.setTimeout(script, ms);
		/// </summary>
		/// <param name="script"></param>
		/// <param name="ms"></param>
		public static void ResponseTimeoutScriptBlock(string script, int ms)
		{
			string allScript = string.Format("<script type=\"text/javascript\">\n window.setTimeout(new Function(\"{0}\"), {1});\n</script>",
				script, ms);

			HttpContext.Current.Response.Write(allScript);
		}

		/// <summary>
		/// 刷新父页面
		/// </summary>
		public static void RefreshParentWindow()
		{
			RequireWindowCommandScript();
			string script = "$HGRootNS.WindowCommand.openerExecuteCommand('refresh');";
			Page page = GetCurrentPage();
			RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// 直接Response出刷新父页面脚本
		/// </summary>
		public static void ResponseRefreshParentWindowScriptBlock()
		{
			ResponseRequireWindowCommandScriptBlock();

			string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.openerExecuteCommand('refresh');");
			HttpContext.Current.Response.Write(script);
		}

		/// <summary>
		/// 向页面中增加与scriptType类型相关的脚本
		/// </summary>
		/// <param name="scriptType">脚本相关类型</param>
		public static void RequiredScript(Type scriptType)
		{
			Page page = GetCurrentPage();

			IEnumerable<ScriptReference> srs = Script.ScriptObjectBuilder.GetScriptReferences(scriptType);

			ScriptManager sm = ScriptManager.GetCurrent(page);
			foreach (ScriptReference sr in srs)
			{
				if (sm != null)
					sm.Scripts.Add(sr);
				else
				{
					DeluxeClientScriptManager.RegisterHeaderScript(page, page.ClientScript.GetWebResourceUrl(scriptType, sr.Name));
				}
			}

			Script.ScriptObjectBuilder.RegisterCssReferences(GetCurrentPage(), scriptType);
		}

		/// <summary>
		/// 得到某类型对应的脚本块
		/// </summary>
		/// <param name="scriptType">类型信息</param>
		/// <returns></returns>
		public static string GetRequiredScriptBlock(Type scriptType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(scriptType != null, "scriptType");

			IEnumerable<ScriptReference> srs = Script.ScriptObjectBuilder.GetScriptReferences(scriptType);
			StringBuilder strB = new StringBuilder(1024);
			foreach (ScriptReference sr in srs)
			{
				Assembly asm = AppDomain.CurrentDomain.Load(sr.Assembly);
				Stream stream = asm.GetManifestResourceStream(sr.Name);
				StreamReader streamReader = new StreamReader(stream);

				string str = streamReader.ReadToEnd();

				strB.Append(str);

				strB.Append("\n");
			}

			return strB.ToString();
		}

		/// <summary>
		/// 得到某类型对应的脚本
		/// </summary>
		/// <param name="scriptType">类型信息</param>
		/// <returns></returns>
		public static string GetRequiredScript(Type scriptType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(scriptType != null, "scriptType");

			List<ResourceEntry> res = Script.ScriptObjectBuilder.GetScriptResourceEntries(scriptType);
			StringBuilder strB = new StringBuilder(1024);
			foreach (ResourceEntry re in res)
			{
				Page page = GetCurrentPage();
				if (page == null)
					page = new Page();
				string src = page.ClientScript.GetWebResourceUrl(re.ComponentType, re.ResourcePath);

				strB.Append(DeluxeClientScriptManager.GetScriptString(src));
				strB.Append("\n");
			}

			return strB.ToString();
		}

		/// <summary>
		/// 注册响应OnLoad事件的脚本
		/// </summary>
		/// <param name="page"></param>
		/// <param name="script"></param>
		public static void RegisterOnLoadScriptBlock(Page page, string script)
		{
			ScriptManager sm = ScriptManager.GetCurrent(page);
			if (sm != null)
				DeluxeClientScriptManager.RegisterAjaxApplicationLoadScriptBlock(page,
					script);
			else
				DeluxeClientScriptManager.RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// 将PageModule和page关联
		/// </summary>
		/// <param name="page"></param>
		public static void AttachPageModules(Page page)
		{
			foreach (IPageModule module in ConfigSectionFactory.GetPageModulesSection().Create().Values)
			{
				module.Init(page);
			}
		}

		/// <summary>
		/// 向页面添加配置扩展信息
		/// </summary>
		public static void LoadConfigPageContent()
		{
			LoadConfigPageContent(false);
		}

		/// <summary>
		/// 向页面添加配置扩展信息
		/// </summary>
		public static void LoadConfigPageContent(bool checkAutoLoad)
		{
			PageContentSection section = ConfigSectionFactory.GetPageExtensionSection();

			Page page = GetCurrentPage();

			if (checkAutoLoad)
			{
				if (!section.AutoLoad)
					return;

				if (page.Header == null)
					return;

				string headerAutoLoad = page.Header.Attributes["autoLoad"];

				if (headerAutoLoad.IsNotEmpty() && headerAutoLoad.ToLower() == "false")
					return;
			}

			foreach (FilePathConfigElement cssElm in section.CssClasses)
			{
				string path = cssElm.Path;
				if (path != string.Empty)
					ClientCssManager.RegisterHeaderEndCss(page, path);
			}

			foreach (FilePathConfigElement scriptElm in section.Scripts)
			{
				string path = scriptElm.Path;
				if (path != string.Empty)
					DeluxeClientScriptManager.RegisterHeaderEndScript(page, path);
			}
		}

		/// <summary>
		/// 是否允许向客户端输出异常详细信息
		/// </summary>
		public static bool AllowResponseExceptionStackTrace()
		{
			bool result = true;

			OutputStackTraceMode mode = ApplicationErrorLogSection.GetSection().OutputStackTrace;

			switch (mode)
			{
				case OutputStackTraceMode.ByCompilationMode:
					result = IsWebApplicationCompilationDebug();
					break;
				case OutputStackTraceMode.True:
					result = true;
					break;
				case OutputStackTraceMode.False:
					result = false;
					break;
			}

			return result;
		}

		/// <summary>
		/// 在Debug模式下，禁止使用缓存
		/// </summary>
		public static void SetResponseNoCacheWhenDebug()
		{
			if (IsWebApplicationCompilationDebug())
				HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		/// <summary>
		/// 获取客户端的IP
		/// </summary>
		/// <returns></returns>
		public static string GetClientIP()
		{
			string ip = WebUtility.GetRequestServerVariable("HTTP_X_FORWARDED_FOR", string.Empty);

			if (!ip.IsValidIPAddress())
			{
				ip = WebUtility.GetRequestServerVariable("REMOTE_ADDR", string.Empty);

				if (!ip.IsValidIPAddress())
					ip = HttpContext.Current.Request.UserHostAddress;
			}

			return ip;
		}

		/// <summary>
		/// 当前Web应用是否运行在Debug状态下
		/// </summary>
		/// <returns></returns>
		public static bool IsWebApplicationCompilationDebug()
		{
			bool debug = false;
			CompilationSection compilation = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");

			if (compilation != null)
			{
				debug = compilation.Debug;
			}

			return debug;
		}
		#endregion

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

		private static readonly string ClientMessageScriptKey = Guid.NewGuid().ToString();

		private static void RegisterClientMessageScript(Page page)
		{
			RequiredScript(typeof(ClientMsgResources));
		}

		private static readonly object ClientMessageCommonScriptKey = new object();

		private static void ResponseClientMessageCommonScriptBlock()
		{
			StringBuilder strB = new StringBuilder();

			PageContentModule.RegisterDefaultNameTable();

			strB.AppendFormat("<script type='text/javascript'>\n{0}\n</script>", DeluxeNameTableContextCache.Instance.GetNameTableScript());
			strB.Append("\n");

			strB.Append(GetRequiredScript(typeof(ClientMsgResources)));
			strB.Append("\n");

			ResponseString(HttpContext.Current, ClientMessageCommonScriptKey, strB.ToString());
		}

		private static object _CurrentPageKey = new object();

		private static void RequireWindowCommandScript()
		{
			RequiredScript(typeof(DeluxeScript));
			string script = string.Format("$HGRootNS.WindowCommand.set_commandInputID('{0}');", DeluxeScript.C_CommandIputClientID);
			script = DeluxeClientScriptManager.AddScriptTags(script);
			Page page = GetCurrentPage();
			page.ClientScript.RegisterStartupScript(page.GetType(), "RequireWindowCommandScript", script);
		}

		private static readonly object RequireWindowCommandScriptKey = new object();

		private static void ResponseRequireWindowCommandScriptBlock()
		{
			StringBuilder strB = new StringBuilder();
			strB.Append(GetRequiredScript(typeof(DeluxeScript)));
			strB.Append("\n");
			string script = string.Format("$HGRootNS.WindowCommand.set_commandInputID('{0}');", DeluxeScript.C_CommandIputClientID);
			strB.Append(DeluxeClientScriptManager.AddScriptTags(script));
			strB.Append("\n");

			ResponseString(HttpContext.Current, RequireWindowCommandScriptKey, strB.ToString());
		}

		private static readonly object ResponseStringKey = new object();

		private static void ResponseString(HttpContext context, object key, string str)
		{
			if (!context.Items.Contains(ResponseStringKey))
				context.Items.Add(ResponseStringKey, new Dictionary<object, object>());
			Dictionary<object, object> dict = (Dictionary<object, object>)context.Items[ResponseStringKey];

			if (!dict.ContainsKey(key))
			{
				dict.Add(key, null);
				HttpContext.Current.Response.Write(str);
			}
		}
		#endregion Private
	}
}
