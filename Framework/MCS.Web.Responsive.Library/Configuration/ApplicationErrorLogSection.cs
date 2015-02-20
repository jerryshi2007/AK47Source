using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// Ӧ�ô�����־����
	/// </summary>
	public class ApplicationErrorLogSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// ��ȡSectionʵ��
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
		/// �쳣����־��Ӧ����
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
		/// Logo��ͼƬ��ַ
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
		/// ȫ����ʽ�ļ���ַ
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
		/// Ĭ���¼�����
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
		/// ��ʾ������Ϣʱ�ģ���ʾ���ʼ�֪ͨ��
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
		/// �ú�����ʽ�����ջ��Ϣ
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
		/// �ú�����ʽ�����ջ��Ϣ
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
	/// ����Ķ�ջ��Ϣ��ѡ��
	/// </summary>
	public enum OutputStackTraceMode
	{
		/// <summary>
		/// ��ֹ���
		/// </summary>
		False = 0,

		/// <summary>
		/// �������
		/// </summary>
		True = 1,

		/// <summary>
		/// ��Web.config�е�system.web/compilation��debug���Ծ���
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
		/// �¼�����
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
