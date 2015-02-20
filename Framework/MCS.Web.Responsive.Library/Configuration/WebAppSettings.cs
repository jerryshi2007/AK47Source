using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace MCS.Web.Responsive.Library
{
	public static class WebAppSettings
	{
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
			string result = string.Empty;

			ContentTypesSection section = ConfigSectionFactory.GetContentTypesSection();

			foreach (ContentTypeConfigElement elt in section.ContentTypes)
			{
				if (StringInCollection(fileExtesionName, elt.FileExtensionNames, true))
				{
					result = elt.ContentType;
					break;
				}
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
	}
}
