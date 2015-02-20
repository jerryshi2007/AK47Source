using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	[Serializable]
	[XElementSerializable]
	public class SchemaPropertyDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SchemaPropertyDefine>
	{
		public SchemaPropertyDefineCollection()
		{
		}

		protected SchemaPropertyDefineCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public SchemaPropertyDefineCollection(SchemaPropertyDefineConfigurationElementCollection propertiesConfig)
		{
			LoadFromConfiguration(propertiesConfig);
		}

		protected override string GetKeyForItem(SchemaPropertyDefine item)
		{
			return item.Name;
		}

		public void LoadFromConfiguration(SchemaPropertyDefineConfigurationElementCollection propertiesConfig)
		{
			this.Clear();

			AppendPropertiesFromConfiguration(propertiesConfig);
		}

		public void AppendPropertiesFromConfiguration(SchemaPropertyDefineConfigurationElementCollection propertiesConfig)
		{
			if (propertiesConfig != null)
			{
				foreach (SchemaPropertyDefineConfigurationElement propDefineElement in propertiesConfig)
				{
					if (this.ContainsKey(propDefineElement.Name))
					{
						if (propDefineElement.AllowOverride)
						{
							this.Remove(pd => pd.Name == propDefineElement.Name);
							this.Add(new SchemaPropertyDefine(propDefineElement));
						}
					}
					else
						this.Add(new SchemaPropertyDefine(propDefineElement));
				}
			}
		}

		public void MergePropertiesDefine(SchemaPropertyDefineCollection propertiesConfig)
		{
			if (propertiesConfig != null)
			{
				foreach (SchemaPropertyDefine propertyDefine in propertiesConfig)
				{
					if (this.ContainsKey(propertyDefine.Name))
					{
						if (propertyDefine.AllowOverride)
						{
							this.Remove(pd => pd.Name == propertyDefine.Name);
							this.Add(propertyDefine);
						}
					}
					else
						this.Add(propertyDefine);
				}
			}
		}
	}
}
