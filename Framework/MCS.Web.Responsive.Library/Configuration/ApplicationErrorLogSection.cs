using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 应用错误日志配置
	/// </summary>
	public class ApplicationErrorLogSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// 获取Section实例
		/// </summary>
		/// <returns></returns>
		public static ApplicationErrorLogSection GetSection()
		{
			ApplicationErrorLogSection result = (ApplicationErrorLogSection)ConfigurationBroker.GetSection("resAppicationErrorLog");

			if (result == null)
				result = new ApplicationErrorLogSection();

			return result;
		}

		private ApplicationErrorLogSection()
		{
		}

		/// <summary>
		/// 异常和日志对应配置
		/// </summary>
		[ConfigurationProperty("exceptionLogs", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ExceptionLogElementColletion ExceptionLogs
		{
			get
			{
				return (ExceptionLogElementColletion)this["exceptionLogs"];
			}
		}

		/// <summary>
		/// Logo的图片地址
		/// </summary>
		[ConfigurationProperty("logoImage", DefaultValue = "", IsRequired = false)]
		public string LogoImage
		{
			get
			{
				return (string)this["logoImage"];
			}
		}

		/// <summary>
		/// 全局样式文件地址
		/// </summary>
		[ConfigurationProperty("globalStyle", DefaultValue = "", IsRequired = false)]
		public string GlobalStyle
		{
			get
			{
				return (string)this["globalStyle"];
			}
		}

		/// <summary>
		/// 默认事件类型
		/// </summary>
		[ConfigurationProperty("defaultLogEventType")]
		public TraceEventType DefaultLogEventType
		{
			get
			{
				return (TraceEventType)this["defaultLogEventType"];
			}
		}

		/// <summary>
		/// 显示错误信息时的，显示的邮件通知人
		/// </summary>
		[ConfigurationProperty("notifyMailAddress", DefaultValue = "", IsRequired = false)]
		public string NotifyMailAddress
		{
			get
			{
				return (string)this["notifyMailAddress"];
			}
		}

		/// <summary>
		/// 用何种形式输出堆栈信息
		/// </summary>
		[ConfigurationProperty("outputStackTrace", DefaultValue = OutputStackTraceMode.ByCompilationMode, IsRequired = false)]
		public OutputStackTraceMode OutputStackTrace
		{
			get
			{
				return (OutputStackTraceMode)this["outputStackTrace"];
			}
		}

		/// <summary>
		/// 用何种形式输出堆栈信息
		/// </summary>
		[ConfigurationProperty("httpStatusCode", DefaultValue = 200, IsRequired = false)]
		public int HttpStatusCode
		{
			get
			{
				return (int)this["httpStatusCode"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public TraceEventType GetExceptionLogEventType(Exception ex)
		{
			Type type = ex.GetType();
			string name = type.FullName;
			ExceptionLogElement elt = ExceptionLogs.GetElement(name);
			if (elt == null)
			{
				name = type.Name;
				elt = ExceptionLogs.GetElement(name);
			}

			TraceEventType eventType = elt == null ? this.DefaultLogEventType : elt.LogEventType;

			return eventType;
		}
	}

	/// <summary>
	/// 输出的堆栈信息的选项
	/// </summary>
	public enum OutputStackTraceMode
	{
		/// <summary>
		/// 禁止输出
		/// </summary>
		False = 0,

		/// <summary>
		/// 允许输出
		/// </summary>
		True = 1,

		/// <summary>
		/// 由Web.config中的system.web/compilation的debug属性决定
		/// </summary>
		ByCompilationMode = 2,
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExceptionLogElementColletion : ConfigurationElementCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ExceptionLogElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ExceptionLogElement)element).Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ExceptionLogElement GetElement(string name)
		{
			return (ExceptionLogElement)this.BaseGet(name);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class ExceptionLogElement : NamedConfigurationElement
	{
		/// <summary>
		/// 事件类型
		/// </summary>
		[ConfigurationProperty("logEventType")]
		public TraceEventType LogEventType
		{
			get
			{
				return (TraceEventType)this["logEventType"];
			}
		}
	}
}
