using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class SchemaPropertyDefineConfigurationElement : PropertyDefineConfigurationElement
	{
		/// <summary>
		/// 该属性属于的标签
		/// </summary>
		[ConfigurationProperty("tab", IsRequired = false)]
		public string Tab
		{
			get
			{
				return (string)this["tab"];
			}
		}

		[ConfigurationProperty("snapshotMode", IsRequired = false, DefaultValue = SnapshotModeDefinition.None)]
		public SnapshotModeDefinition SnapshotMode
		{
			get
			{
				return (SnapshotModeDefinition)this["snapshotMode"];
			}
		}

		[ConfigurationProperty("snapshotFieldName", IsRequired = false, DefaultValue = "")]
		public string SnapshotFieldName
		{
			get
			{
				return (string)this["snapshotFieldName"];
			}
		}

		[ConfigurationProperty("batchMode", IsRequired = false, DefaultValue = PropertyBatchMode.Normal)]
		public PropertyBatchMode BatchMode
		{
			get
			{
				return (PropertyBatchMode)this["batchMode"];
			}
		}
	}

	public class SchemaPropertyDefineConfigurationElementCollection : NamedConfigurationElementCollection<SchemaPropertyDefineConfigurationElement>
	{
	}
}
