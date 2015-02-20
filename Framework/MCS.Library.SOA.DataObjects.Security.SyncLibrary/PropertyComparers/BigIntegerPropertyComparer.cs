using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertyComparers
{
    public class BigIntegerPropertyComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            var srcValue = srcValues[mapping.SourceProperty] ?? Common.EmptyString;
            var targetString = targetObj.Properties[mapping.TargetProperty].StringValue;

            if (typeof(long).IsAssignableFrom(srcValue.GetType()))
            {
                if (string.IsNullOrEmpty(targetString) == false)
                {
                    return ((long)srcValue).Equals(long.Parse(targetString));
                }
                else
                    return false; // 左边对象有值，而右边对象没值
            }
            else if (srcValue is string)
            {
                return ((string)srcValue) == targetString;
            }
            else
            {
                return mapping.Parameters["sourceDefaultValue"] == targetString; //源对象为null或其他情形
            }
        }
    }
}
