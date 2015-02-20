using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Logging
{
	/// <summary>
	/// 
	/// </summary>
	public class LoggerConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// Logger是否可用的标志
		/// </summary>
		[ConfigurationProperty("enable")]
		public bool Enabled
		{
			get
			{
				return (bool)this["enable"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal LogFilterPipeline Filters
		{
			get
			{
				return LogFilterFactory.GetFilterPipeLine(this.FiltersElements);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal List<FormattedTraceListenerBase> Listeners
		{
			get
			{
				return TraceListenerFactory.GetListeners(this.ListenerElements);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("Filters")]
		public LoggerFilterConfigurationElementCollection FiltersElements
		{
			get
			{
				return (LoggerFilterConfigurationElementCollection)this["Filters"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("Listeners")]
		public LoggerListenerConfigurationElementCollection ListenerElements
		{
			get
			{
				return (LoggerListenerConfigurationElementCollection)this["Listeners"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LoggerConfigurationElementCollection : NamedConfigurationElementCollection<LoggerConfigurationElement>
	{
	}
}
