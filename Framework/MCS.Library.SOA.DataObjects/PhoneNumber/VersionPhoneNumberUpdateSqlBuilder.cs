using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	public class VersionPhoneNumberUpdateSqlBuilder : VersionStrategyUpdateSqlBuilder<PhoneNumber>
	{
		public static readonly VersionPhoneNumberUpdateSqlBuilder Instance = new VersionPhoneNumberUpdateSqlBuilder();

		/*protected override InsertSqlClauseBuilder PrepareInsertSqlBuilder(PhoneNumber obj,ORMappingItemCollection mapping)
		{
			PhoneNumber phonenumber = (PhoneNumber)obj;
 
			if (OguBase.IsNullOrEmpty(schemaObj.Creator))
			{
				if (DeluxePrincipal.IsAuthenticated)
					schemaObj.Creator = DeluxeIdentity.CurrentUser;
			}时间的问题
 
			InsertSqlClauseBuilder builder = base.PrepareInsertSqlBuilder(obj, mapping);
				builder.AppendItem("Data", obj.ToString());
 
			return builder;
		}*/

	}
}
