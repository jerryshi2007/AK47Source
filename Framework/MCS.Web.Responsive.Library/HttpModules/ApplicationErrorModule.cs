using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Logging;
using MCS.Web.Responsive.Library;

[assembly: WebResource("MCS.Web.Responsive.Library.HttpModules.ApplicationErrorModuleTemplate_stopLogo.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.Library.HttpModules.ApplicationErrorModuleTemplate_stopLogo-English.gif", "image/gif")]

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 
	/// </summary>
	public class ApplicationErrorModule : IHttpModule
	{
		private const int ApplicationErrorEventID = 65001;

		/// <summary>
		/// 
		/// </summary>
		public static readonly object ExceptionItemKey = new object();

		void IHttpModule.Init(HttpApplication context)
		{
			context.Error += new EventHandler(context_Error);
			context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
		}

		void IHttpModule.Dispose()
		{
		}

		private void context_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			if (HttpContext.Current.Handler is Page)
			{
				Page page = (Page)HttpContext.Current.Handler;

				page.PreRender += new EventHandler(page_PreRender);
			}
		}

		private void page_PreRender(object sender, EventArgs e)
		{
			Page page = (Page)sender;

			page.ClientScript.RegisterClientScriptBlock(this.GetType(),
				"notifyMailAddress",
				string.Format("var mseeageNotifyMailAddress = \"{0}\";",
					ScriptHelper.CheckScriptString(ApplicationErrorLogSection.GetSection().NotifyMailAddress)),
				true);
		}

		private void context_Error(object sender, EventArgs e)
		{
			var app = (HttpApplication)sender;
			HttpContext context = app.Context;

			if (!(context.CurrentHandler is Page ||
				context.Request.FilePath.EndsWith("aspx", StringComparison.OrdinalIgnoreCase) ||
				context.Request.FilePath.EndsWith("ashx", StringComparison.OrdinalIgnoreCase)))
				return;



			ChangeResponseStatus(context);

			Page page = context.CurrentHandler as Page;

			if (page == null || page.IsCallback == false)
			{
				string errorPageUrl = string.Empty;

				if (context.Error != null)
				{
					ScriptManager sm = null;

					if (page != null)
						sm = ScriptManager.GetCurrent(page);

					if (sm == null || sm.IsInAsyncPostBack == false)
					{
						ProcessException(context);
					}
				}
			}
		}

		/// <summary>
		/// 根据异常，调整Response的Status和StatusDescription
		/// </summary>
		private static void ChangeResponseStatus(HttpContext context)
		{
			if (context.Error != null)
			{
				context.Response.StatusCode = ApplicationErrorLogSection.GetSection().HttpStatusCode;
				//HttpContext.Current.Response.SubStatusCode = 1024;

				//HttpContext.Current.Response.StatusDescription = HttpContext.Current.Error.Message;
			}
		}

		private static void ProcessException(HttpContext context)
		{
			if (context.Error != null)
			{
				string detail = EnvironmentHelper.GetEnvironmentInfo() + "\r\n" +
					context.Error.GetAllStackTrace();

				Exception realEx = ExceptionHelper.GetRealException(context.Error);

				context.ClearError();

				realEx.TryWriteAppLog(detail);

				ResponseErrorMessage(context, realEx.Message, detail);
			}
		}

		private static void ResponseErrorMessage(HttpContext context, string strErrorMsg, string strStackTrace)
		{
			string errorFormat = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
							"MCS.Web.Responsive.Library.HttpModules.ApplicationErrorModuleTemplate.htm");

			Page page = context.CurrentHandler is Page ? (Page)context.CurrentHandler : new Page();
			string imageUrl = ApplicationErrorLogSection.GetSection().LogoImage;

			if (imageUrl.IsNullOrEmpty())
				imageUrl = page.ClientScript.GetWebResourceUrl(typeof(ApplicationErrorModule),
					"MCS.Web.Responsive.Library.HttpModules." +
					Translator.Translate(Define.DefaultCategory, "ApplicationErrorModuleTemplate_stopLogo.gif"));

			string goBackBtnDisplay = context.Request.UrlReferrer != null ? "inline" : "none";
			string closePromptValue = context.Request.UrlReferrer != null ? "true" : "false";

			string[] strArray = errorFormat.Split('$');
			for (int i = 0; i < strArray.Length; i++)
			{
				switch (strArray[i])
				{
					case "globalStyle":
						if (ApplicationErrorLogSection.GetSection().GlobalStyle.IsNotEmpty())
							strArray[i] = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />",
								ApplicationErrorLogSection.GetSection().GlobalStyle);
						else
							strArray[i] = string.Empty;
						break;
					case "imageUrl":
						strArray[i] = imageUrl;
						break;
					case "goBackBtnDisplay":
						strArray[i] = goBackBtnDisplay;
						break;
					case "closePromptValue":
						strArray[i] = closePromptValue;
						break;
					case "copyHint":
						strArray[i] = HttpUtility.HtmlAttributeEncode(Translator.Translate(Define.DefaultCategory, "复制信息"));
						break;
					case "copyCompletedPrompt":
						strArray[i] = HttpUtility.HtmlAttributeEncode(Translator.Translate(Define.DefaultCategory, "复制信息完成"));
						break;
					case "errorMessage":
						strArray[i] = HttpUtility.HtmlEncode(strErrorMsg);
						break;
					case "errorStackTrace":
						string errorMessage = string.Empty;

						if (WebAppSettings.AllowResponseExceptionStackTrace())
							errorMessage += "\r\n" + strStackTrace;

						strArray[i] = errorMessage.Replace("\r\n", "<br/>");
						break;
					case "clickToDetail":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "点击此处展开详细信息......");
						break;
					case "clickToCloseDetail":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "点击此处关闭详细信息......");
						break;
					case "ConfirmClose":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "您确认要关闭窗口吗？");
						break;
					case "errorMessageHead":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "错误提示");
						break;
					case "returnBtn":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "返回");
						break;
					case "closeBtn":
						strArray[i] = Translator.Translate(Define.DefaultCategory, "关闭");
						break;
					case "notifyMailAddress":
						string mailAddress = ApplicationErrorLogSection.GetSection().NotifyMailAddress;

						if (mailAddress.IsNotEmpty())
							strArray[i] = HttpUtility.HtmlAttributeEncode(string.Format(@"mailto://{0}", mailAddress));
						else
							strArray[i] = "#";

						break;
				}
			}

			context.Response.Clear();
			context.Response.Write(string.Join("", strArray));
			context.Response.Flush();
			context.Response.End();
		}
	}
}
