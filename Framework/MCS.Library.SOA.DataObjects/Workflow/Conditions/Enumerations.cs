using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Conditions
{
    /// <summary>
    /// 流程的筛选类型
    /// </summary>
    public enum WfAssigneeExceptionFilterType
    {
        [EnumItemDescription("全部")]
        All = 0,

        [EnumItemDescription("当前环节人员异常")]
        CurrentActivityError = 1,

        [EnumItemDescription("当前环节和后续环节人员异常")]
        ExsitedActivitiesError = 2
    }

    /// <summary>
    /// 指派人的筛选类型
    /// </summary>
    public enum WfAssigneesFilterType
    {
        [EnumItemDescription("当前环节")]
        CurrentActivity = 0,

        [EnumItemDescription("所有环节")]
        AllActivities = 1
    }
}
