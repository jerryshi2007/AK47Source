using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Logging;
using System.Web;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library
{
	internal static class WebApplicationExceptionExtension
	{
		private const int ApplicationErrorEventID = 65001;

		public static bool TryWriteAppLog(LogEntity logEntity)
		{
			bool result = false;

			if (logEntity != null)
			{
				try
				{
					Logger logger = LoggerFactory.Create("webApplicationError");

					logger.Write(logEntity);
					result = true;
				}
				catch
				{
				}
			}

			return result;
		}

		public static bool TryWriteAppLog(string message, string detail)
		{
			LogEntity logEntity = new LogEntity(message);

			logEntity.LogEventType = TraceEventType.Error;

			logEntity.EventID = ApplicationErrorEventID;
			logEntity.StackTrace = detail;

			HttpContext context = HttpContext.Current;

			try
			{
				if (context != null)
				{
					string[] paths = context.Request.ApplicationPath.Split('/');
					logEntity.Source = paths[paths.Length - 1];
					logEntity.Title = string.Format("{0}应用页面错误", context.Request.ApplicationPath);
					logEntity.ExtendedProperties.Add("RequestUrl", context.Request.Url.AbsoluteUri);
					logEntity.ExtendedProperties.Add("UserHostAddress", context.Request.UserHostAddress);

					if (HttpContext.Current.User != null)
					{
						logEntity.ExtendedProperties.Add("UserLogOnName", HttpContext.Current.User.Identity.Name);
					}
				}
			}
			catch
			{
			}

			return TryWriteAppLog(logEntity);
		}

		public static bool TryWriteAppLog(this Exception ex)
		{
			bool result = false;

			if (ex != null)
			{
				string detail = string.Empty;

				try
				{
					detail = EnvironmentHelper.GetEnvironmentInfo();
				}
				catch (System.Exception)
				{
				}

				if (detail.IsNotEmpty())
					detail += "\r\n";

				detail += GetAllStackTrace(ex);

				result = TryWriteAppLog(ex, detail);
			}

			return result;
		}

		public static bool TryWriteAppLog(this Exception ex, string detail)
		{
			bool result = false;

			if (ex != null)
			{
				LogEntity logEntity = new LogEntity(ex);

				logEntity.LogEventType = ApplicationErrorLogSection.GetSection().GetExceptionLogEventType(ex);

				logEntity.EventID = ApplicationErrorEventID;
				logEntity.StackTrace = detail;

				HttpContext context = HttpContext.Current;

				try
				{
					if (context != null)
					{
						string[] paths = context.Request.ApplicationPath.Split('/');

						logEntity.Source = paths[paths.Length - 1];

						logEntity.Title = string.Format("{0}应用页面错误", context.Request.ApplicationPath);
						logEntity.ExtendedProperties.Add("RequestUrl", context.Request.Url.AbsoluteUri);
						logEntity.ExtendedProperties.Add("UserHostAddress", context.Request.UserHostAddress);

						if (HttpContext.Current.User != null)
							logEntity.ExtendedProperties.Add("UserLogOnName", HttpContext.Current.User.Identity.Name);
					}
				}
				catch
				{
				}

				result = TryWriteAppLog(logEntity);
			}

			return result;
		}

		public static string GetAllStackTrace(this Exception ex)
		{
			StringBuilder strB = new StringBuilder();

			for (Exception innerEx = ex; innerEx != null; innerEx = innerEx.InnerException)
			{
				if (strB.Length > 0)
					strB.Append("\n");

				strB.AppendFormat("-------------{0}：{1}-----------\n", innerEx.GetType().Name, innerEx.Message);
				strB.Append(innerEx.StackTrace);
			}

			return strB.ToString();
		}
	}
}
