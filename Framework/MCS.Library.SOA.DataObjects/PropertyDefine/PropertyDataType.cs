using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 参照TypeCode枚举值
    /// </summary>
    public enum PropertyDataType
    {
        [EnumItemDescription("对象", 1)]
        DataObject = 1,
        [EnumItemDescription("布尔", 3)]
        Boolean = 3,
        [EnumItemDescription("整型", 9)]
        Integer = 9,
        [EnumItemDescription("浮点", 15)]
        Decimal = 15,
        [EnumItemDescription("时间", 16)]
        DateTime = 16,
        [EnumItemDescription("文本", 18)]
        String = 18,
        [EnumItemDescription("枚举", 20)]
        Enum = 20
    }
}
