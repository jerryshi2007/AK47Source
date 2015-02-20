using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using MCS.Library.Globalization;
using MCS.Library.Logging;

namespace MCS.Web.Responsive.Library
{
	public class GlobalizationModule : IHttpModule
	{
		#region IHttpModule Members

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
		}

		#endregion

		#region Private
		private void context_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			CultureInfo translatedCulture = GetCultureInfo();

			Thread.CurrentThread.CurrentUICulture = translatedCulture;

			if (HttpContext.Current.Handler is Page)
			{
				Page page = (Page)HttpContext.Current.Handler;

				page.Init += new EventHandler(page_Init);
			}
		}

		private static CultureInfo GetCultureInfo()
		{
			string language = TranslatorConfigSettings.GetConfig().DefaultCulture.Name;

			HttpRequest request = HttpContext.Current.Request;

			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
			{
				try
				{
					IUserCultureInfoAccessor accessor = UserCultureInfoSettings.GetConfig().UserCultureInfoAccessor;

					if (accessor != null)
						language = accessor.GetCurrentUserLanguageID(HttpContext.Current.User.Identity.Name, language);
				}
				catch (System.Exception ex)
				{
					WriteExceptionToLog(ex);
				}
			}

			CultureInfo translatedCulture = null;

			int cultureID = 2052;

			if (int.TryParse(language, out cultureID))
				translatedCulture = new CultureInfo(cultureID);
			else
				translatedCulture = new CultureInfo(language);

			translatedCulture.DateTimeFormat.DateSeparator = "-";
			translatedCulture.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";
			translatedCulture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
			translatedCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
			translatedCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
			translatedCulture.DateTimeFormat.ShortTimePattern = "HH:mm";

			return translatedCulture;
		}

		private static void WriteExceptionToLog(System.Exception ex)
		{
			Logger logger = LoggerFactory.Create("webApplicationError");
			LogEntity logEntity = new LogEntity(ex);
			HttpContext context = HttpContext.Current;

			logEntity.LogEventType = ApplicationErrorLogSection.GetSection().GetExceptionLogEventType(ex);

			string[] paths = context.Request.ApplicationPath.Split('/');
			logEntity.StackTrace = ex.StackTrace;
			logEntity.Source = paths[paths.Length - 1];
			logEntity.Title = string.Format("{0}应用页面错误", context.Request.ApplicationPath);
			logEntity.ExtendedProperties.Add("RequestUrl", context.Request.Url.AbsoluteUri);
			try
			{
				if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
				{
					logEntity.ExtendedProperties.Add("UserLogOnName", HttpContext.Current.User.Identity.Name);
				}
			}
			catch
			{
			}
			logger.Write(logEntity);
		}

		private static void page_Init(object sender, EventArgs e)
		{
			Page page = (Page)sender;

			page.Culture = Thread.CurrentThread.CurrentUICulture.Name;
		}
		#endregion
	}
}
