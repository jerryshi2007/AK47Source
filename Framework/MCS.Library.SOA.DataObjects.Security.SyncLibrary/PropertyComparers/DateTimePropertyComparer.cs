using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertyComparers
{
    public class DateTimePropertyComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            var srcValue = srcValues[mapping.SourceProperty] ?? Common.EmptyString;
            var targetString = targetObj.Properties[mapping.TargetProperty].StringValue;

            if (typeof(DateTime).IsAssignableFrom(srcValue.GetType()))
            {
                if (string.IsNullOrEmpty(targetString) == false)
                {
                    DateTime left = (DateTime)srcValue;
                    DateTime right = DateTime.Parse(targetString);

                    return left.Equals(right); //完全精度匹配（几乎不可能实现）

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
