using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 属性数据的持久化器的配置节
	/// </summary>
	public sealed class PropertyPersisterSettings : ConfigurationSection
	{
		public static PropertyPersisterSettings GetConfig()
		{
			PropertyPersisterSettings settings = (PropertyPersisterSettings)ConfigurationBroker.GetSection("persisterGroupSettings");

			if (settings == null)
				settings = new PropertyPersisterSettings();

			return settings;
		}

		private PropertyPersisterSettings()
		{
		}

		[ConfigurationProperty("persisters", IsRequired = false)]
		public TypeConfigurationCollection Persisters
		{
			get
			{
				return (TypeConfigurationCollection)this["persisters"];
			}
		}

		/*	[ConfigurationProperty("groups", IsRequired = false)]
		  public PropertyPersisterGroupConfigurationElement Groups
		  {
			  get
			  {
				  return (PropertyPersisterGroupConfigurationElement)this["groups"];
			  }
		  } */
	}

	/*
public class PropertyPersisterGroupConfigurationElement : NamedConfigurationElement
{
	[ConfigurationProperty("persisters", IsRequired = false)]
	public TypeConfigurationCollection Persisters
	{
		get
		{
			return (TypeConfigurationCollection)this["persisters"];
		}
	}
}


public class PropertyPersisterGroupConfigurationElementCollection : NamedConfigurationElementCollection<PropertyPersisterConfigurationElement>
{
}

public class PropertyPersisterConfigurationElement : NamedConfigurationElement
{
	protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
	{
		return true;
	}
} */
}
