#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	XmlLogFormatter.cs
// Remark	：	Xml格式化器，LogFormatter的派生类，具体实现Xml格式化。
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// 1.1          ccic\zhangtiejun    20081226        增加log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Logging
{
	/// <summary>
	/// Xml格式化器
	/// </summary>
	/// <remarks>
	/// LogFormatter的派生类，具体实现Xml格式化
	/// </remarks>
	public sealed class XmlLogFormatter : LogFormatter
	{
		private const string DefaultValue = "";

		private XmlLogFormatter()
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">Formatter名称</param>
		/// <remarks>
		/// 根据名称，创建XmlLogFormatter对象
		/// </remarks>
		public XmlLogFormatter(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="element">配置节对象</param>
		/// <remarks>
		/// 根据配置信息，创建TextLogFormatter对象
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
		/// lang="cs" region="Get Formatter Test" tittle="获取Formatter对象"></code>
		/// </remarks>
		public XmlLogFormatter(LoggerFormatterConfigurationElement element)
			: base(element.Name)
		{
		}

		/// <summary>
		/// 将LogEntity对象格式化成XML串
		/// </summary>
		/// <param name="log">LogEntity对象</param>
		/// <returns>格式化好的XML串</returns>
		/// <remarks>
		/// 重载方法，实现格式化
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
		/// lang="cs" region="Format Test" tittle="获取Formatter对象"></code>
		/// </remarks>
		public override string Format(LogEntity log)
		{
			XElement root = new XElement("Root");

			Format(log, root);

			return root.ToString();
		}

		private void Format(object obj, XElement parent)
		{
			if (obj == null)
				return;

			XElement objElement = parent.AddChildElement("Object");

			objElement.SetAttributeValue("name", obj.GetType().Name);

			if (Type.GetTypeCode(obj.GetType()) == TypeCode.Object)
			{
				foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
				{
					XElement propertyElement = objElement.AddChildElement("Property");

					propertyElement.SetAttributeValue("name", propertyInfo.Name);

					if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && Type.GetTypeCode(propertyInfo.PropertyType) == TypeCode.Object)
					{
						IEnumerable values = (IEnumerable)propertyInfo.GetValue(obj, null);
						if (values != null)
						{
							foreach (object value in values)
							{
								Format(value, propertyElement);
							}
						}
					}
					else
					{
						propertyElement.Value = ConvertToString(propertyInfo, obj);
					}
				}
			}
		}

		private static string ConvertToString(PropertyInfo propertyInfo, object obj)
		{
			object value = propertyInfo.GetValue(obj, null);
			string result = XmlLogFormatter.DefaultValue;

			if (value != null)
			{
				if (value is DateTime)
					result = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
				else
					result = value.ToString();
			}

			return result;
		}
	}
}
