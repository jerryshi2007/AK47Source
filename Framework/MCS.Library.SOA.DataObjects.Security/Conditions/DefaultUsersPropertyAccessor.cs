using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Expression;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	public class DefaultUsersPropertyAccessor : SCPropertyAccessorBase
	{
		public DefaultUsersPropertyAccessor(SchemaNameAndPropertyName snpn) :
			base(snpn)
		{
		}

		public override object GetValue(ISCConditionCalculatingContext context, ParamObjectCollection arrParams)
		{
			object result = context.CurrentObject.Properties.GetValue<string>(this.SchemaAndPropertyName.PropertyName, null);

			if (result == null)
				result = string.Empty;

			return result;
		}
	}
}
