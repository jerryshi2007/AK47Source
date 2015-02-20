using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public class StringPropertySetter : IPropertySetter
    {
        public void SetValue(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            string srcValue = srcValues[mapping.SourceProperty] as string ?? string.Empty;
            targetObj.Properties[mapping.TargetProperty].StringValue = srcValue;
        }
    }
}
