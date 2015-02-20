using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.PropertyComparers
{
    public class EnumPropertyComparer : IPropertyComparer
    {
        public bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            string enumType = mapping.Parameters["enumType"];
            if (string.IsNullOrEmpty(enumType))
                throw new System.Configuration.ConfigurationErrorsException("配置EnumPropertyComparer时，必须指定enumType属性");

            var type = Type.GetType(enumType);
            if (type == null)
                throw new System.Configuration.ConfigurationErrorsException("未找到指定的枚举类型 " + enumType);

            object srcValue = srcValues[mapping.SourceProperty];

            string targetString = targetObj.Properties[mapping.TargetProperty].StringValue;

            if (string.IsNullOrEmpty(targetString) == false)
            {
                return srcValue.Equals(int.Parse(targetString));
            }
            else
                return false;
        }
    }
}
