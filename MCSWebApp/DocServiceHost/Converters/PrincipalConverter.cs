using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
	public class PrincipalConverter : DataTypeConverterGenericBase<Principal, DCTPrincipal>
	{
		protected override void GenericConvert(Principal srcObject, DCTPrincipal targetObject)
		{
			targetObject.ID = srcObject.Id;
			targetObject.Title = srcObject.Title;
			targetObject.PricinpalType = (DCTPrincipalType)srcObject.PrincipalType;
            if (srcObject.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.User)
            {
                (targetObject as DCTUser).LoginName = srcObject.LoginName;
            }
		}
	}

    public class GroupConverter : DataTypeConverterGenericBase<Group, DCTGroup>
    {
        protected override void GenericConvert(Group srcObject, DCTGroup targetObject)
        {
            DCTConverterHelper.Convert<Principal, DCTPrincipal>(srcObject, targetObject);
        }
    }

}