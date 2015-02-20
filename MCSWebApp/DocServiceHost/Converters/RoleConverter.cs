using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Microsoft.SharePoint.Client;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Converters
{
    public class RoleConverter : DataTypeConverterGenericBase<RoleDefinition,DCTRoleDefinition>
    {
        protected override void GenericConvert(RoleDefinition srcObject, DCTRoleDefinition targetObject)
        {
            targetObject.ID = srcObject.Id;
            targetObject.Name = srcObject.Name;
            targetObject.Description = srcObject.Description;
        }
    }
}