#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	XmlLogFormatter.cs
// Remark	��	Xml��ʽ������LogFormatter�������࣬����ʵ��Xml��ʽ����
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// 1.1          ccic\zhangtiejun    20081226        ����log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
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
	/// Xml��ʽ����
	/// </summary>
	/// <remarks>
	/// LogFormatter�������࣬����ʵ��Xml��ʽ��
	/// </remarks>
	public sealed class XmlLogFormatter : LogFormatter
	{
		private const string DefaultValue = "";

		private XmlLogFormatter()
		{
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="name">Formatter����</param>
		/// <remarks>
		/// �������ƣ�����XmlLogFormatter����
		/// </remarks>
		public XmlLogFormatter(string name)
			: base(name)
		{
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="element">���ýڶ���</param>
		/// <remarks>
		/// ����������Ϣ������TextLogFormatter����
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
		/// lang="cs" region="Get Formatter Test" tittle="��ȡFormatter����"></code>
		/// </remarks>
		public XmlLogFormatter(LoggerFormatterConfigurationElement element)
			: base(element.Name)
		{
		}

		/// <summary>
		/// ��LogEntity�����ʽ����XML��
		/// </summary>
		/// <param name="log">LogEntity����</param>
		/// <returns>��ʽ���õ�XML��</returns>
		/// <remarks>
		/// ���ط�����ʵ�ָ�ʽ��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
		/// lang="cs" region="Format Test" tittle="��ȡFormatter����"></code>
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
