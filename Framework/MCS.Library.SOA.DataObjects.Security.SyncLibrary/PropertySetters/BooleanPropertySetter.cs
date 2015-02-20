using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertySetters
{
    public class BooleanPropertySetter : IPropertySetter
    {
        public void SetValue(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            var srcValue = srcValues[mapping.SourceProperty] ?? Common.EmptyString;

            if (typeof(bool).IsAssignableFrom(srcValue.GetType()))
            {
                targetObj.Properties[mapping.TargetProperty].StringValue = ((bool)srcValue).ToString();
            }
            else if (srcValue is string)
            {
                targetObj.Properties[mapping.TargetProperty].StringValue = (string)srcValue;
            }
            else
            {
                //其他情况如null，DbNull等，以及不知如何转换的
                targetObj.Properties[mapping.TargetProperty].StringValue = "False";
            }
        }
    }
}
