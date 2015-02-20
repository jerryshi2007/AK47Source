using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	public class SchemaMappingInfo
	{
		private SetterObjectMappingConfigurationElementCollection _ModifyOperations = new SetterObjectMappingConfigurationElementCollection();
		private ComparerPropertyMappingConfigurationElementCollection _ComparedProperties = new ComparerPropertyMappingConfigurationElementCollection();
		private SetterPropertyMappingConfigurationElementCollection _ModifiedProperties = new SetterPropertyMappingConfigurationElementCollection();

		public string Name
		{
			get;
			set;
		}

		public string ComparerName
		{
			get;
			set;
		}

		public string Prefix
		{
			get;
			set;
		}

		public SetterObjectMappingConfigurationElementCollection ModifyOperations
		{
			get
			{
				return this._ModifyOperations;
			}
		}

		public ComparerPropertyMappingConfigurationElementCollection ComparedProperties
		{
			get
			{
				return this._ComparedProperties;
			}
		}

		public SetterPropertyMappingConfigurationElementCollection ModifiedProperties
		{
			get
			{
				return this._ModifiedProperties;
			}
		}
	}
}
