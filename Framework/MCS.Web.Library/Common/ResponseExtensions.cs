using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MCS.Web.Library
{
	/// <summary>
	/// 为HttpResponse添加的扩展方法类
	/// </summary>
	public static class ResponseExtensions
	{
		/// <summary>
		/// 为Http Head中的ContentDisposition项的文件名进行编码。真对于不同的浏览器编码方式不同
		/// </summary>
		/// <param name="response"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string EncodeFileNameInContentDisposition(this HttpResponse response, string fileName)
		{
			string result = fileName;

			if (response != null)
			{
				HttpRequest request = HttpContext.Current.Request;

				if (request.Browser.IsBrowser("IE"))
					result = HttpUtility.UrlEncode(fileName);
			}
			else
				result = HttpUtility.UrlEncode(fileName);

			return result;
		}
	}
}
