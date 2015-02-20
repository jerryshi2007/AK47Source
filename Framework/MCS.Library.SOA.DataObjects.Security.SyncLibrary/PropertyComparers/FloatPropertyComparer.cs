using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertyComparers
{
    public class FloatPropertyComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            var srcValue = srcValues[mapping.SourceProperty] ?? Common.EmptyString;
            var targetString = targetObj.Properties[mapping.TargetProperty].StringValue;

            if (typeof(float).IsAssignableFrom(srcValue.GetType()))
            {
                if (string.IsNullOrEmpty(targetString) == false)
                {
                    float left = (float)srcValue;
                    float right = float.Parse(targetString);

                    if (string.IsNullOrEmpty(mapping.Parameters["precision"]))
                    {
                        return left.Equals(right); //完全精度匹配（几乎不可能实现）
                    }
                    else
                    {
                        int precision = int.Parse(mapping.Parameters["precision"]);
                        double delta = Math.Abs(left - right) * Math.Pow(10, precision); //通常两个数相差在delta范围内算相等
                        return delta < 1;
                    }
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
