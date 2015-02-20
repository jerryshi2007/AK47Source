using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	public abstract class SCPropertyAccessorBase
	{
		private SchemaNameAndPropertyName _SchemaAndPropertyName;

		public SCPropertyAccessorBase(SchemaNameAndPropertyName snpn)
		{
			this._SchemaAndPropertyName = snpn;
		}

		public SchemaNameAndPropertyName SchemaAndPropertyName
		{
			get
			{
				return this._SchemaAndPropertyName;
			}
		}

		public abstract object GetValue(ISCConditionCalculatingContext context, ParamObjectCollection arrParams);
	}
}
