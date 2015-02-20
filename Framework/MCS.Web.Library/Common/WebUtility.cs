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
	/// Web������
	/// </summary>
	/// <remarks>Web������</remarks>
	public static class WebUtility
	{
		//private static readonly string PageRenderModeQueryStringName = "PageRenderMode" + (new object()).GetHashCode().ToString();
		/// <summary>
		/// ���ע�ͣ���Ϊ����
		/// </summary>
		private static readonly string PageRenderModeQueryStringName = "PageRenderMode";
		internal static object PageRenderControlItemKey = new object();

		/// <summary>
		/// ����ContentTypeKey���õ�Response��ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>����ContentTypeKey���õ�Response��ContentType</remarks>
		public static string GetContentTypeByKey(string key)
		{
			ContentTypesSection section = ConfigSectionFactory.GetContentTypesSection();

			key = key.ToLower();
			ContentTypeConfigElement elt = section.ContentTypes[key];

			string contentType = elt != null ? elt.ContentType : string.Empty;

			return contentType;
		}

		/// <summary>
		/// ����ContentTypeKey���õ�Response��ContentType�����ֵΪ���򷵻�Ĭ��key��ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <param name="defaultKey">Ĭ��ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>����ContentTypeKey���õ�Response��ContentType�����ֵΪ���򷵻�Ĭ��key��ContentType</remarks>
		public static string GetContentTypeByKey(string key, string defaultKey)
		{
			string contentType = GetContentTypeByKey(key);
			if (contentType == string.Empty) contentType = GetContentTypeByKey(defaultKey);

			return contentType;
		}

		/// <summary>
		/// ����ContentTypeKey���õ�Response��ContentType
		/// </summary>
		/// <param name="key">ContentTypeKey</param>
		/// <returns>ContentType</returns>
		/// <remarks>����ContentTypeKey���õ�Response��ContentType</remarks>
		public static string GetContentTypeByKey(ResponseContentTypeKey key)
		{
			return GetContentTypeByKey(key.ToString());
		}

		/// <summary>
		/// �����ļ������õ�Response��ContentType
		/// </summary>
		/// <param name="fileName">�ļ���</param>
		/// <returns>ContentType</returns>
		/// <remarks>�����ļ������õ�Response��ContentType</remarks>
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
		/// �����ļ���չ�����õ�Response��ContentType
		/// </summary>
		/// <param name="fileExtesionName">�ļ���չ��</param>
		/// <returns>ContentType</returns>
		/// <remarks>�����ļ���չ�����õ�Response��ContentType</remarks>
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
		/// �õ�ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <param name="strName">��Ҫ��ȡ��Request��������</param>
		/// <param name="strDefault">ȱʡֵ</param>
		/// <returns>ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static string GetRequestQueryString(string strName, string strDefault)
		{
			string str = HttpContext.Current.Request.QueryString[strName];

			return string.IsNullOrEmpty(str) ? strDefault : str;
		}

		/// <summary>
		/// �õ�ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <typeparam name="T">��ȡֵ������</typeparam>
		/// <param name="strName">��Ҫ��ȡ��Request������������</param>
		/// <param name="defaultValue">ȱʡֵ</param>
		/// <returns>ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��Request�����ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static T GetRequestQueryValue<T>(string strName, T defaultValue)
			where T : IConvertible
		{
			string str = GetRequestQueryString(strName, null);
			return str == null ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
		}

		/// <summary>
		/// �õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <typeparam name="T">���ص���������</typeparam>
		/// <param name="strName">��Ҫ��ȡ��Form��������</param>
		/// <param name="defaultValue">ȱʡֵ</param>
		/// <returns>�õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����</returns>
		public static T GetRequestFormValue<T>(string strName, T defaultValue)
			where T : IConvertible
		{
			string str = GetRequestFormString(strName, null);
			return str == null ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
		}

		/// <summary>
		/// �õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <param name="strName">��Ҫ��ȡ��Form��������</param>
		/// <param name="strDefault">ȱʡֵ</param>
		/// <returns>ĳһ��Post�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static string GetRequestFormString(string strName, string strDefault)
		{
			string str = HttpContext.Current.Request.Form[strName];

			return string.IsNullOrEmpty(str) ? strDefault : str;
		}

		/// <summary>
		/// �õ�ĳһ��ServerVariables���ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <param name="strName">��Ҫ��ȡ��ServerVariables��������</param>
		/// <param name="strDefault">ȱʡֵ</param>
		/// <returns>ĳһ��Post�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��ServerVariables���ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static string GetRequestServerVariable(string strName, string strDefault)
		{
			string str = HttpContext.Current.Request.ServerVariables[strName];

			return string.IsNullOrEmpty(str) ? strDefault : str;
		}

		/// <summary>
		/// �õ�ĳһ��Cookies���ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <param name="strName">��Ҫ��ȡ��Cookies��������</param>
		/// <param name="strDefault">ȱʡֵ</param>
		/// <returns>ĳһ��Post�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��Cookies���ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static string GetRequestCookieString(string strName, string strDefault)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];

			string str = cookie == null ? strDefault : cookie.Value;
			return string.IsNullOrEmpty(str) ? strDefault : str;
		}

		/// <summary>
		/// �õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����
		/// </summary>
		/// <param name="strName">��Ҫ��ȡ��Params��������</param>
		/// <param name="strDefault">ȱʡֵ</param>
		/// <returns>ĳһ��Post�����ݣ���������ڸ����������ȱʡֵ����</returns>
		/// <remarks>�õ�ĳһ��Post�ύ�����ݣ���������ڸ����������ȱʡֵ����</remarks>
		public static string GetRequestParamString(string strName, string strDefault)
		{
			string str = HttpContext.Current.Request.Params[strName];

			return string.IsNullOrEmpty(str) ? strDefault : str;
		}

		/// <summary>
		/// ��ȡ��ǰ�����PageRenderMode
		/// </summary>
		/// <returns>PageRenderMode</returns>
		/// <remarks>��ȡ��ǰ�����PageRenderMode</remarks>
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
		/// ��PageRenderMode��ӵ���ǰ����ExecutionUrl��������Url
		/// </summary>
		/// <param name="pageRenderMode">PageRenderMode</param>
		/// <param name="ignoreParamNames">����ԭʼ��QueryString�в�������</param>
		/// <returns>���Url</returns>
		/// <remarks>��PageRenderMode��ӵ���ǰ����ExecutionUrl��������Url</remarks>
		public static string GetRequestExecutionUrl(PageRenderMode pageRenderMode, params string[] ignoreParamNames)
		{
			return GetRequestExecutionUrl(PageRenderModeQueryStringName, pageRenderMode.ToString(), ignoreParamNames);
		}

		/// <summary>
		/// ���ݵ�ǰ��HttpRequest��ExecutionUrl�ģ�����queryString���������µ�Url��ͨ����CurrentExecutionFilePath��Request��ҳ��·����
		/// һ���ģ����Ƕ���Server.Transfer���͵�ҳ�棬���ǲ�һ���ġ�
		/// </summary>
		/// <param name="appendQueryString">Url�еĲ�ѯ�������磺uid=sz&amp;name=Haha</param>
		/// <param name="ignoreParamNames">����ԭʼ��QueryString�в�������</param>
		/// <returns>���Url</returns>
		/// <remarks>���ݵ�ǰ��HttpRequest��ExecutionUrl�ģ�����queryString���������µ�Url��ͨ����CurrentExecutionFilePath��Request��ҳ��·����
		/// һ���ģ����Ƕ���Server.Transfer���͵�ҳ�棬���ǲ�һ���ġ�</remarks>
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
		/// ���ݵ�ǰ��HttpRequest��ExecutionUrl�ģ����Ӳ������������µ�Url��ͨ����CurrentExecutionFilePath��Request��ҳ��·����
		/// һ���ģ����Ƕ���Server.Transfer���͵�ҳ�棬���ǲ�һ���ġ�
		/// </summary>
		/// <param name="appendParamName">��ӵ�QueryString�еĲ�������</param>
		/// <param name="appendParamValue">��ӵ�QueryString�еĲ���ֵ</param>
		/// <param name="ignoreParamNames">����ԭʼ��QueryString�в�������</param>
		/// <returns>���Url</returns>
		/// <remarks>���ݵ�ǰ��HttpRequest��ExecutionUrl�ģ����Ӳ������������µ�Url��ͨ����CurrentExecutionFilePath��Request��ҳ��·����
		/// һ���ģ����Ƕ���Server.Transfer���͵�ҳ�棬���ǲ�һ���ġ�
		/// </remarks>
		public static string GetRequestExecutionUrl(string appendParamName, string appendParamValue, params string[] ignoreParamNames)
		{
			ignoreParamNames = StringArrayAdd(ignoreParamNames, appendParamName);

			string appendQueryString = string.Format("{0}={1}", appendParamName, appendParamValue);

			string result = GetRequestExecutionUrl(appendQueryString, ignoreParamNames);

			return result;
		}

		/// <summary>
		/// ���ݵ�ǰ��HttpRequest��Url�ģ�����queryString���������µ�Url��
		/// </summary>
		/// <param name="appendQueryString">Url�еĲ�ѯ�������磺uid=sz&amp;name=Haha</param>
		/// <param name="ignoreParamNames">����ԭʼ��QueryString�в�������</param>
		/// <returns>���Url</returns>
		/// <remarks>���ݵ�ǰ��HttpRequest��Url�ģ�����queryString���������µ�Url��</remarks>
		public static string GetRequestUrl(string appendQueryString, params string[] ignoreParamNames)
		{
			HttpRequest request = HttpContext.Current.Request;

			string result = GetRequestUrlInternal(request.FilePath, request.QueryString, appendQueryString, ignoreParamNames);

			return result;
		}

		/// <summary>
		/// �����ǰ��HtppHandler��Page���ж����Ƿ���PostBack״̬
		/// </summary>
		/// <returns>�Ƿ���PostBack״̬</returns>
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
		/// �����ǰ��HtppHandler��Page���ж����Ƿ���Callback״̬
		/// </summary>
		/// <returns>�Ƿ���Callback״̬</returns>
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
		/// �Ƿ�Ϊ���ָ���ؼ�ҳ��
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public static bool IsRenderSpecialControlPage(Page page)
		{
			Control ctr = page.Items[PageRenderControlItemKey] as Control;

			return ctr != null;
		}

		/// <summary>
		/// �����ǰ��HtppHandler��Page����ô�ҵ���ViewState���ԣ���ȡ���е�ֵ
		/// </summary>
		/// <param name="key">ViewState��key</param>
		/// <returns>ViewState�еĶ���</returns>
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
		/// �����ǰ��HtppHandler��Page����ô�����ݴ��뵽ViewState��
		/// </summary>
		/// <param name="key">ViewState�ļ�ֵ</param>
		/// <param name="data">��Ҫ���������</param>
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
		/// ���ű��е��ַ������滻��˫���źͻس�
		/// </summary>
		/// <param name="strData">�ַ���</param>
		/// <returns>�滻��Ľ��</returns>
		public static string CheckScriptString(string strData)
		{
			return CheckScriptString(strData, true);
		}

		/// <summary>
		/// ���ű��е��ַ������滻��˫���źͻس�
		/// </summary>
		/// <param name="strData">�ַ���</param>
		/// <param name="changeCRToBR">�Ƿ񽫻س��滻��Html��BR</param>
		/// <returns>�滻��Ľ��</returns>
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
		/// �����ScriptTag�Ľű�
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
		/// ͨ��ScriptManager.RegisterClientScriptBlockע��ͻ�����ʾ��Ϣ�Ľű�
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
		/// ͨ��ScriptManager.RegisterClientScriptBlockע��ͻ��˴�����Ϣ�Ľű�
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

			RegisterClientErrorMessage(ex.Message, detail, Translator.Translate(Define.DefaultCategory, "����"));
		}

		/// <summary>
		/// ͨ��ScriptManager.RegisterClientScriptBlockע��ͻ��˴�����Ϣ�Ľű�
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
		/// �ͻ��˵�����ʾ��
		/// </summary>
		/// <param name="strMessage">��ʾ����Ϣ</param>
		/// <param name="strDetail">��ʾ����ϸ��Ϣ</param>
		/// <param name="strTitle">��ʾ��Title</param>
		public static void ShowClientMessage(string strMessage, string strDetail, string strTitle)
		{
			Page page = GetCurrentPage();

			RegisterClientMessageScript(page);
			RegisterOnLoadScriptBlock(page,
				string.Format("$HGRootNS.ClientMsg.inform(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle)));
		}

		/// <summary>
		/// Response�ͻ��˵�����ʾ��
		/// </summary>
		/// <param name="strMessage">��ʾ����Ϣ</param>
		/// <param name="strDetail">��ʾ����ϸ��Ϣ</param>
		/// <param name="strTitle">��ʾ��Title</param>
		public static void ResponseShowClientMessageScriptBlock(string strMessage, string strDetail, string strTitle)
		{
			ResponseClientMessageCommonScriptBlock();

			string script = string.Format("$HGRootNS.ClientMsg.inform(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle));
			script = DeluxeClientScriptManager.AddScriptTags(script);

			HttpContext.Current.Response.Write(script);
		}

		/// <summary>
		/// �õ��ͻ��˵����Ի���Ľű�
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
		/// �ͻ��˵��������
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

			ShowClientError(ex.Message, detail, Translator.Translate(Define.DefaultCategory, "����"));
		}

		/// <summary>
		/// �ͻ��˵��������
		/// </summary>
		/// <param name="strMessage">�������Ϣ</param>
		/// <param name="strDetail">�������ϸ��Ϣ</param>
		/// <param name="strTitle">�����Title</param>
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
		/// �ͻ��˵��������
		/// </summary>
		/// <param name="ex"></param>
		public static void ResponseShowClientErrorScriptBlock(System.Exception ex)
		{
			ex.NullCheck("ex");

			ResponseShowClientErrorScriptBlock(ex.Message, ex.StackTrace, Translator.Translate(Define.DefaultCategory, "����"));
		}

		/// <summary>
		/// Response�ͻ��˵��������
		/// </summary>
		/// <param name="strMessage">�������Ϣ</param>
		/// <param name="strDetail">�������ϸ��Ϣ</param>
		/// <param name="strTitle">�����Title</param>
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
		/// �ͻ��˵���ȷ�Ͽ�
		/// </summary>
		/// <param name="strMessage">��ʾ����Ϣ</param>
		/// <param name="strDetail">��ʾ����ϸ��Ϣ</param>
		/// <param name="strTitle">��ʾ��Title</param>
		/// <param name="okBtnText">ȷ����ť�ı�</param>
		/// <param name="cancelBtnText">ȡ����ť�ı�</param>
		/// <param name="onOKClientEventHandler">ȷ�Ϻ�ͻ�����Ӧ����</param>
		/// <param name="onCancelClientEventHandler">ȡ����ͻ�����Ӧ����</param>
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
		/// ע��ͻ��˵�����ʾ��ű�
		/// </summary>
		public static void RegisterClientMessageScript()
		{
			RegisterClientMessageScript(GetCurrentPage());
		}
		#endregion ClientMsg

		/// <summary>
		/// ��ȡ��ǰҳ
		/// </summary>
		/// <returns></returns>
		public static Page GetCurrentPage()
		{
			Page page = (Page)HttpContext.Current.Items[_CurrentPageKey];
			if (page == null)
				page = HttpContext.Current.CurrentHandler as Page;

			//ExceptionHelper.TrueThrow(page == null, "��ǰû�д��������ҳ�棡");

			return page;
		}

		/// <summary>
		/// ���õ�ǰҳ
		/// </summary>
		/// <param name="page"></param>
		public static void SetCurrentPage(Page page)
		{
			HttpContext.Current.Items[_CurrentPageKey] = page;
		}

		/// <summary>
		/// �������ڴ�С��λ��
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
		/// �رմ���
		/// </summary>
		public static void CloseWindow()
		{
			RequireWindowCommandScript();
			string script = "$HGRootNS.WindowCommand.executeCommand('close');";
			Page page = GetCurrentPage();
			RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// ֱ��Response���رմ��ڽű���Ȼ�����Response
		/// </summary>
		public static void ResponseCloseWindowScriptBlock()
		{
			ResponseRequireWindowCommandScriptBlock();

			string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.executeCommand('close');");
			HttpContext.Current.Response.Write(script);
			HttpContext.Current.Response.End();
		}

		/// <summary>
		/// ֱ��Response���window.setTimeout(script, ms);
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
		/// ˢ�¸�ҳ��
		/// </summary>
		public static void RefreshParentWindow()
		{
			RequireWindowCommandScript();
			string script = "$HGRootNS.WindowCommand.openerExecuteCommand('refresh');";
			Page page = GetCurrentPage();
			RegisterOnLoadScriptBlock(page, script);
		}

		/// <summary>
		/// ֱ��Response��ˢ�¸�ҳ��ű�
		/// </summary>
		public static void ResponseRefreshParentWindowScriptBlock()
		{
			ResponseRequireWindowCommandScriptBlock();

			string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.openerExecuteCommand('refresh');");
			HttpContext.Current.Response.Write(script);
		}

		/// <summary>
		/// ��ҳ����������scriptType������صĽű�
		/// </summary>
		/// <param name="scriptType">�ű��������</param>
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
		/// �õ�ĳ���Ͷ�Ӧ�Ľű���
		/// </summary>
		/// <param name="scriptType">������Ϣ</param>
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
		/// �õ�ĳ���Ͷ�Ӧ�Ľű�
		/// </summary>
		/// <param name="scriptType">������Ϣ</param>
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
		/// ע����ӦOnLoad�¼��Ľű�
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
		/// ��PageModule��page����
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
		/// ��ҳ�����������չ��Ϣ
		/// </summary>
		public static void LoadConfigPageContent()
		{
			LoadConfigPageContent(false);
		}

		/// <summary>
		/// ��ҳ�����������չ��Ϣ
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
		/// �Ƿ�������ͻ�������쳣��ϸ��Ϣ
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
		/// ��Debugģʽ�£���ֹʹ�û���
		/// </summary>
		public static void SetResponseNoCacheWhenDebug()
		{
			if (IsWebApplicationCompilationDebug())
				HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		/// <summary>
		/// ��ȡ�ͻ��˵�IP
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
		/// ��ǰWebӦ���Ƿ�������Debug״̬��
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
