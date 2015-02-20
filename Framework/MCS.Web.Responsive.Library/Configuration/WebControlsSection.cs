using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Compilation;
using System.Web.UI;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// WebControl���������Ϣ
	/// </summary>
	/// <remarks>WebControl���������Ϣ</remarks>
	public sealed class WebControlsSection : DeluxeConfigurationSection
	{
		private static readonly ConfigurationProperty _S_PropWebControls =
			new ConfigurationProperty(null, typeof(WebControlConfigElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection _S_Properties = BuildProperties();

		private static ConfigurationPropertyCollection BuildProperties()
		{
			ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
			//properties.Add(_S_PropDefaultCssUrl);
			properties.Add(_S_PropWebControls);

			return properties;
		}


		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return _S_Properties;
			}
		}

		/// <summary>
		/// WebControl������Ϣ����
		/// </summary>
		/// <remarks>WebControl������Ϣ����</remarks>
		public WebControlConfigElementCollection Controls
		{
			get
			{
				return (WebControlConfigElementCollection)this[_S_PropWebControls];
			}
		}

		/// <summary>
		/// ��ȡĳ���Ϳؼ�������CssUrl
		/// </summary>
		/// <param name="controlType">�ؼ����</param>
		/// <returns>CssUrl</returns>
		/// <remarks>��ȡĳ���Ϳؼ�������CssUrl</remarks>
		public string GetConfigCssUrl(Type controlType)
		{
			string url = string.Empty;
			WebControlConfigElement elt = (WebControlConfigElement)this.Controls[controlType];
			if (elt == null)
				url = string.Empty;
			else
			{
				url = elt.CssUrl;
			}
			return url;
		}

		/// <summary>
		/// ��ȡĳ���Ϳؼ�������CssUrl
		/// </summary>
		/// <param name="control">�ؼ�ʵ��</param>
		/// <returns>CssUrl</returns>
		/// <remarks>��ȡĳ���Ϳؼ�������CssUrl</remarks>
		public string GetConfigCssUrl(Control control)
		{
			return GetConfigCssUrl(control.GetType());
		}
	}

	/// <summary>
	/// WebControl���ýڵ㼯��
	/// </summary>
	public sealed class WebControlConfigElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// creates a new WebControlConfigElement
		/// </summary>
		/// <returns>new WebControlConfigElement</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new WebControlConfigElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element 
		/// </summary>
		/// <param name="element">The ConfigurationElement to return the key for.</param>
		/// <returns>An Object that acts as the key for the specified ConfigurationElement.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((WebControlConfigElement)element).Type;
		}

		/// <summary>
		/// ͨ���ؼ����ͣ�������Ӧ���ýڵ�
		/// </summary>
		/// <param name="controlType">�ؼ�����</param>
		/// <returns>���ýڵ�</returns>
		public WebControlConfigElement this[Type controlType]
		{
			get
			{
				return (WebControlConfigElement)base.BaseGet(controlType);
			}
		}

		/// <summary>
		/// ͨ���ؼ�ʵ����������Ӧ���ýڵ�
		/// </summary>
		/// <param name="control">�ؼ�ʵ��</param>
		/// <returns>���ýڵ�</returns>
		public WebControlConfigElement this[Control control]
		{
			get
			{
				return this[control.GetType()];
			}
		}
	}

	/// <summary>
	/// �ؼ����ýڵ���Ϣ
	/// </summary>
	public sealed class WebControlConfigElement : ConfigurationElement
	{
		private static readonly ConfigurationProperty _S_Type =
		   new ConfigurationProperty("type", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty _S_CssUrl =
			new ConfigurationProperty("cssUrl", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
		//private static readonly ConfigurationProperty _S_UseSysCssUrl =
		//    new ConfigurationProperty("useSysCssUrl", typeof(bool), false, ConfigurationPropertyOptions.None);

		private static ConfigurationPropertyCollection _S_Properties = BuildProperties();

		private static ConfigurationPropertyCollection BuildProperties()
		{
			ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
			properties.Add(_S_Type);
			properties.Add(_S_CssUrl);
			//properties.Add(_S_UseSysCssUrl);

			return properties;
		}

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return _S_Properties;
			}
		}

		private Type _Type;

		/// <summary>
		/// �ؼ�����
		/// </summary>
		public Type Type
		{
			get
			{
				if (this._Type == null)
				{
					string strType = this.StrType;
					Type t = BuildManager.GetType(strType, false);
					if (t == null)
					{
						throw new ArgumentException(string.Format(Resources.DeluxeWebResource.E_UnknownType, new object[] { strType }));
					}
					if (!typeof(Control).IsAssignableFrom(t))
					{
						throw new ArgumentException(string.Format(Resources.DeluxeWebResource.E_NotControl, new object[] { strType }));
					}

					this._Type = t;
				}
				return _Type;
			}
		}

		/// <summary>
		/// �ؼ�����
		/// </summary>
		internal string StrType
		{
			get
			{
				return (string)this[_S_Type];
			}
		}

		/// <summary>
		/// �ؼ���Ӧ��CssUrl
		/// </summary>
		public string CssUrl
		{
			get
			{
				return (string)this[_S_CssUrl];
			}
		}
	}
}
