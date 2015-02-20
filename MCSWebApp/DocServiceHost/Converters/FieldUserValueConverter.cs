using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
    public class FieldUserValueConverter :DataTypeConverterGenericBase<FieldUserValue, DCTUser>
    {
        protected override void GenericConvert(FieldUserValue srcObject, DCTUser targetObject)
        {
            if (targetObject == null)
                targetObject = new DCTUser();
            targetObject.PricinpalType = DCTPrincipalType.User;
            targetObject.ID = srcObject.LookupId;
            targetObject.LoginName = srcObject.LookupValue;
        }
    }
}