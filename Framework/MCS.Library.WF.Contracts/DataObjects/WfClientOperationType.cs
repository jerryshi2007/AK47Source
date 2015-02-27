using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.DataObjects
{
    /// <summary>
    /// 日志操作类型
    /// </summary>
    public enum WfClientOperationType
    {
        [EnumItemDescription("添加文件")]
        Add = 0,

        [EnumItemDescription("修改文件")]
        Update = 1,

        [EnumItemDescription("删除文件")]
        Delete = 2,

        [EnumItemDescription("修改意见")]
        ModifyOpinion = 3,

        [EnumItemDescription("异常处理修改")]
        ExceptionOperation = 4,

        [EnumItemDescription("为流转而打开表单")]
        OpenFormForMove = 5,

        [EnumItemDescription("打开表单")]
        OpenForm = 6,

        [EnumItemDescription("其他修改")]
        Other = 8192
    }
}
