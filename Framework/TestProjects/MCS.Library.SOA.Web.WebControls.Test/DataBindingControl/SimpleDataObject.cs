using System;
using System.Collections.Generic;
using System.Web;
using MCS.Library.Validation;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
    public class SimpleDataObject
    {
        [DateTimeEmptyValidator(MessageTemplate = "TimeInput不能为空")]
        public DateTime TimeInput
        {
            get;
            set;
        }

        //[DateTimeEmptyValidator(MessageTemplate = "DateInput不能为空")]
        //[DateTimeRangeValidator("2011-11-17", "2011-11-18", MessageTemplate = "日期范围不正确")]
        public DateTime DateInput
        {
            get;
            set;
        }

        [IntegerRangeValidator(1, 20, MessageTemplate = "请输入1-20之间的整数")]
        [StringByteLengthValidator(2, 5, MessageTemplate = "字节范围2-5")]
        [RegexValidator(@"^\d{3,4}$", MessageTemplate = "格式不正确")]
        public string IntegerInput
        {
            get;
            set;
        }

        //[NotNullValidator(MessageTemplate = "不能为空")]
        [ObjectNullValidator(MessageTemplate = "不能为空")]
        public float? NullableFloat
        {
            get;
            set;
        }

        //[EnumDefaultValueValidator(MessageTemplate = "不能默认值")]
        public DataType SimpleDataType { get; set; }

        //[IOguObjectNullValidator(MessageTemplate = "人员不能为空")]
        public IOguObject User { get; set; }

        // [EnumDefaultValueValidator(MessageTemplate = "不能默认值")]
        public SimpleUserCollection Users
        {
            get;
            set;
        }
    }

    public enum DataType
    {
        [EnumItemDescription("空")]
        Non = 0,
        [EnumItemDescription("整型")]
        Int = 1,
        [EnumItemDescription("串")]
        String = 2,
      
    }
}
