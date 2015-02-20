using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class DataFieldDefine
	{
		public string DefaultValue { get; set; }

		public DataFieldDefine()
		{
		}

		public DataFieldDefine(string name, PropertyDataType dataType)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			this.LoadPropertiesFromDataType(dataType);
			this.Name = name;
		}

		public DataFieldDefine(PropertyDataType dataType)
		{
			this.LoadPropertiesFromDataType(dataType);
			this._DataType = dataType;
		}

		private PropertyDataType _DataType = PropertyDataType.String;

		public PropertyDataType DataType
		{
			get { return _DataType; }
			set { _DataType = value; }
		}

		public string Name
		{
			get
			{
				return Properties.GetValue("Name", string.Empty);
			}
			set
			{
				Properties.SetValue("Name", value);
			}
		}
		private PropertyValueCollection _Properties = null;

		public PropertyValueCollection Properties
		{
			get
			{
				if (this._Properties == null)
					this._Properties = new PropertyValueCollection();

				return this._Properties;
			}
		}

		private void LoadPropertiesFromDataType(PropertyDataType dataType)
		{
			string propConfigKey = dataType.ToString() + "FieldProperties";

			PropertyDefineCollection propDefinitions = new PropertyDefineCollection();

			propDefinitions.LoadPropertiesFromConfiguration(propConfigKey);

			this.Properties.InitFromPropertyDefineCollection(propDefinitions);
		}
	}
}
