using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public class MailArgKeys
    {
        /// <summary>
        /// 流程大分类
        /// </summary>
        public static readonly string ApplicationName = "ApplicationName";
        /// <summary>
        /// 流程小分类
        /// </summary>
        public static readonly string ProgramName = "ProgramName";
        /// <summary>
        /// 任务标题
        /// </summary>
        public static readonly string TaskTitle = "TaskTitle";
        /// <summary>
        /// 任务的业务地址
        /// </summary>
        public static readonly string Url = "Url";
        /// <summary>
        /// 接收人姓名
        /// </summary>
        public static readonly string ReceiverName = "ReceiverName";
        /// <summary>
        /// 待办时间
        /// </summary>
        public static readonly string DeliverTime = "DeliverTime";
        /// <summary>
        /// 提交人
        /// </summary>
        public static readonly string CreatorName = "CreatorName";
    }
}
