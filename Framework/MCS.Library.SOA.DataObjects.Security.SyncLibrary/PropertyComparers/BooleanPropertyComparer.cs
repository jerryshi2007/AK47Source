using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertyComparers
{
    public class BooleanPropertyComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            var srcValue = srcValues[mapping.SourceProperty] ?? Common.EmptyString;
            var targetString = targetObj.Properties[mapping.TargetProperty].StringValue;

            if (typeof(bool).IsAssignableFrom(srcValue.GetType()))
            {
                if (string.IsNullOrEmpty(targetString) == false)
                {
                    return ((bool)srcValue).Equals(bool.Parse(targetString));
                }
                else
                    return false; // 左边对象有值，而右边对象没值
            }
            else if (srcValue is string)
            {
                return string.Equals((string)srcValue, (string)targetString, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return mapping.Parameters["sourceDefaultValue"] == targetString; //源对象为null或其他情形
            }
        }
    }
}
