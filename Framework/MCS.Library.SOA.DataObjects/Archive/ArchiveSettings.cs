using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 归档相关的配置信息
	/// </summary>
	public sealed class ArchiveSettings : ConfigurationSection
	{
		/// <summary>
		/// 得到归档相关的配置信息
		/// </summary>
		/// <returns></returns>
		public static ArchiveSettings GetConfig()
		{
			ArchiveSettings result = (ArchiveSettings)ConfigurationBroker.GetSection("archiveSettings");

			if (result == null)
				result = new ArchiveSettings();

			return result;
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 打开表单时，是否自动定向到归档库
		/// </summary>
		[ConfigurationProperty("autoRedirectToArchiveDB", IsRequired = false, DefaultValue = false)]
		public bool AutoRedirectToArchiveDB
		{
			get
			{
				return (bool)this["autoRedirectToArchiveDB"];
			}
		}

		/// <summary>
		/// 是否复制附件
		/// </summary>
		[ConfigurationProperty("copyMaterials", IsRequired = false, DefaultValue = true)]
		public bool CopyMaterials
		{
			get
			{
				return (bool)this["copyMaterials"];
			}
		}

		/// <summary>
		/// 目标附件的路径配置名称
		/// </summary>
		[ConfigurationProperty("deleteOriginalMaterials", IsRequired = false, DefaultValue = false)]
		public bool DeleteOriginalMaterials
		{
			get
			{
				return (bool)this["deleteOriginalMaterials"];
			}
		}

		/// <summary>
		/// 是否删除原始数据
		/// </summary>
		[ConfigurationProperty("deleteOriginalData", IsRequired = false, DefaultValue = true)]
		public bool DeleteOriginalData
		{
			get
			{
				return (bool)this["deleteOriginalData"];
			}
		}

		/// <summary>
		/// 得到执行器的工厂类
		/// </summary>
		/// <returns></returns>
		public IArchiveExecutorFactory GetFactory()
		{
			TypeConfigurationElement elem = Factories["archiveExecutor"];

			ExceptionHelper.FalseThrow(elem != null, "没有在配置节archiveSettings中配置archiveExecutor的插件");

			return (IArchiveExecutorFactory)elem.CreateInstance();
		}

		/// <summary>
		/// 数据库连接的映射
		/// </summary>
		[ConfigurationProperty("connectionMappings", IsRequired = false)]
		public ArchiveConnectionMappingElementCollection ConnectionMappings
		{
			get
			{
				return (ArchiveConnectionMappingElementCollection)this["connectionMappings"];
			}
		}

		/// <summary>
		/// 应用程序的路径映射
		/// </summary>
		[ConfigurationProperty("appPathMappings", IsRequired = false)]
		public ArchiveAppPathMappingElementCollection AppPathMappings
		{
			get
			{
				return (ArchiveAppPathMappingElementCollection)this["appPathMappings"];
			}
		}

		[ConfigurationProperty("factories", IsRequired = false)]
		private TypeConfigurationCollection Factories
		{
			get
			{
				return (TypeConfigurationCollection)this["factories"];
			}
		}
	}

	public sealed class ArchiveConnectionMappingElementCollection : NamedConfigurationElementCollection<ArchiveConnectionMappingElement>
	{
		public ArchiveConnectionMappingContextCollection CreateConnectionMappingContexts()
		{
			ArchiveConnectionMappingContextCollection result = new ArchiveConnectionMappingContextCollection();

			foreach (ArchiveConnectionMappingElement elem in this)
			{
				DbConnectionMappingContext context = DbConnectionMappingContext.CreateMapping(elem.SourceConnection, elem.DestinationConnection);

				result.Add(context);
			}

			return result;
		}
	}

	public sealed class ArchiveConnectionMappingElement : NamedConfigurationElement
	{
		/// <summary>
		/// 源路经
		/// </summary>
		[ConfigurationProperty("source", IsRequired = true)]
		public string SourceConnection
		{
			get
			{
				return (string)this["source"];
			}
		}

		/// <summary>
		/// 目标路径
		/// </summary>
		[ConfigurationProperty("destination", IsRequired = true)]
		public string DestinationConnection
		{
			get
			{
				return (string)this["destination"];
			}
		}
	}

	/// <summary>
	/// 应用程序附件的根路径映射集合
	/// </summary>
	public sealed class ArchiveAppPathMappingElementCollection : NamedConfigurationElementCollection<ArchiveAppPathMappingElement>
	{
		public ArchiveAppPathMappingContextCollection CreateAppPathMappingContexts()
		{
			ArchiveAppPathMappingContextCollection result = new ArchiveAppPathMappingContextCollection();

			foreach (ArchiveAppPathMappingElement elem in this)
			{
				AppPathMappingContext context = AppPathMappingContext.CreateMapping(elem.SourcePath, elem.DestinationPath);

				result.Add(context);
			}

			return result;
		}
	}

	/// <summary>
	/// 应用程序附件的根路径映射
	/// </summary>
	public sealed class ArchiveAppPathMappingElement : NamedConfigurationElement
	{
		/// <summary>
		/// 源路经
		/// </summary>
		[ConfigurationProperty("source", IsRequired = true)]
		public string SourcePath
		{
			get
			{
				return (string)this["source"];
			}
		}

		/// <summary>
		/// 目标路径
		/// </summary>
		[ConfigurationProperty("destination", IsRequired = true)]
		public string DestinationPath
		{
			get
			{
				return (string)this["destination"];
			}
		}
	}
}
