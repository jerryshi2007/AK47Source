using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// ҳ����չ��Ϣ���ý�
	/// </summary>
	public class PageContentSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// ��ʽ���ļ�����
		/// </summary>
		[ConfigurationProperty("cssClasses", IsRequired = false)]
		public CssClassesConfigurationElement CssClasses
		{
			get
			{
				return (CssClassesConfigurationElement)this["cssClasses"];
			}
		}

		[ConfigurationProperty("scripts", IsRequired = false)]
		public ScriptsConfigurationElement Scripts
		{
			get
			{
				return (ScriptsConfigurationElement)this["scripts"];
			}
		}

		/// <summary>
		/// ���Զ����ص�ҳ�����ýڼ���
		/// </summary>
		[ConfigurationProperty("notAutoLoadPages", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public NotAutoLoadPageConfigElementCollection NotAutoLoadPages
		{
			get
			{
				return (NotAutoLoadPageConfigElementCollection)this["notAutoLoadPages"];
			}
		}

		/// <summary>
		/// �Զ��������ݵĿؼ�ID
		/// </summary>
		[ConfigurationProperty("autoEncryptControlIDs", DefaultValue = "")]
		public string AutoEncryptControlIDs
		{
			get
			{
				return (string)this["autoEncryptControlIDs"];
			}
		}

		/// <summary>
		/// �Ƿ��Զ�Ϊÿ��ҳ�������չ
		/// </summary>
		[ConfigurationProperty("autoLoad", DefaultValue = false)]
		public bool AutoLoad
		{
			get
			{
				return (bool)this["autoLoad"];
			}
		}
	}

	/// <summary>
	/// ���Զ����ص�ҳ�����ýڼ���
	/// </summary>
	public class NotAutoLoadPageConfigElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new NotAutoLoadPageConfigElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NotAutoLoadPageConfigElement)element).Path;
		}
	}

	/// <summary>
	/// ���Զ����ص�ҳ�����ý�
	/// </summary>
	public class NotAutoLoadPageConfigElement : ConfigurationElement
	{
		/// <summary>
		/// page·��
		/// </summary>
		[ConfigurationProperty("path", DefaultValue = "")]
		public string Path
		{
			get
			{
				return (string)this["path"];
			}
		}
	}
}
