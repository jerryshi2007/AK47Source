using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
	public class UserConverter : DataTypeConverterGenericBase<User, DCTUser>
	{
		protected override void GenericConvert(User srcObject, DCTUser targetObject)
		{
			DCTConverterHelper.Convert<Principal, DCTPrincipal>(srcObject, targetObject);

			targetObject.LoginName = srcObject.LoginName;
		}
	}
}