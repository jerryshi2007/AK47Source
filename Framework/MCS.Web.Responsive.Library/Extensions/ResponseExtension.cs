using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 为HttpResponse添加的扩展方法类
	/// </summary>
	public static class ResponseExtension
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

		#region Response Script
		/// <summary>
		/// 直接Response出关闭窗口脚本，然后结束Response
		/// </summary>
		/// <param name="response">HttpResponse对象</param>
		public static void WriteCloseWindowScriptBlock(this HttpResponse response)
		{
			if (response != null)
			{
				HttpContextExtension.ResponseRequireWindowCommandScriptBlock();

				string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.executeCommand(\"close\");");

				response.Write(script);
				response.End();
			}
		}

		/// <summary>
		/// 直接Response出刷新父页面脚本
		/// </summary>
		/// <param name="response">HttpResponse对象</param>
		public static void WriteRefreshParentWindowScriptBlock(this HttpResponse response)
		{
			if (response != null)
			{
				HttpContextExtension.ResponseRequireWindowCommandScriptBlock();

				string script = DeluxeClientScriptManager.AddScriptTags("$HGRootNS.WindowCommand.openerExecuteCommand(\"refresh\");");

				response.Write(script);
			}
		}

		/// <summary>
		/// 直接Response输出window.setTimeout(script, ms);
		/// </summary>
		/// <param name="response">HttpResponse对象</param>
		/// <param name="script"></param>
		/// <param name="ms"></param>
		public static void WriteTimeoutScriptBlock(this HttpResponse response, string script, int ms)
		{
			if (response != null)
			{
				string allScript = string.Format("<script type=\"text/javascript\">\n window.setTimeout(new Function(\"{0}\"), {1});\n</script>",
					script, ms);

				response.Write(allScript);
			}
		}
		#endregion

		#region Response MsgBox
		/// <summary>
		/// 客户端弹出错误框
		/// </summary>
		/// <param name="response">HttpResponse对象</param>
		/// <param name="ex"></param>
		public static void WriteShowClientErrorScriptBlock(this HttpResponse response, System.Exception ex)
		{
			ex.NullCheck("ex");

			WriteShowClientErrorScriptBlock(response, ex.Message, ex.StackTrace, Translator.Translate(Define.DefaultCategory, "错误"));
		}

		/// <summary>
		/// Response客户端弹出错误框
		/// </summary>
		/// <param name="response">HttpResponse对象</param>
		/// <param name="strMessage">错误框消息</param>
		/// <param name="strDetail">错误框详细信息</param>
		/// <param name="strTitle">错误框Title</param>
		public static void WriteShowClientErrorScriptBlock(this HttpResponse response, string strMessage, string strDetail, string strTitle)
		{
			if (response != null)
			{
				HttpContextExtension.ResponseClientMessageCommonScriptBlock();

				if (WebAppSettings.AllowResponseExceptionStackTrace() == false)
					strDetail = string.Empty;

				string script = ScriptHelper.GetShowClientErrorScript(strMessage, strDetail, strTitle);

				script = DeluxeClientScriptManager.AddScriptTags(script);

				WebApplicationExceptionExtension.TryWriteAppLog(strMessage, strDetail);

				response.Write(script);
			}
		}
		#endregion
	}
}
