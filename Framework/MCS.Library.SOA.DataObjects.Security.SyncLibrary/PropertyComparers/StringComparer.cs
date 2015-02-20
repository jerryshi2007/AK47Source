using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    /// <summary>
    /// 字符串的比较器
    /// </summary>
    public class StringComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            string srcValue = (srcValues[mapping.SourceProperty] as string ?? string.Empty);
            string targetValue = (string)targetObj.Properties[mapping.TargetProperty].StringValue ?? string.Empty;

            return srcValue.Equals(targetValue);
        }
    }
}
