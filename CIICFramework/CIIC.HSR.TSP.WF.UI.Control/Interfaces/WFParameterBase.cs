using CIIC.HSR.TSP.WF.Bizlet.Impl;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    /// <summary>
    /// 流程相关公用参数
    /// </summary>
    public class WFParameterBase
    {
        /// <summary>
        /// 租户编码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 行为的结果，如同意，不同意，确认等。
        /// 形成审批历史中的审批结果。
        /// </summary>
        public string ActionResult { get; set; }

        /// <summary>
        /// 审批意见ID。
        /// </summary>
        public string ClientOpinionId { get; set; }

        /// <summary>
        /// 审批意见文本。
        /// </summary>
        public string ClientOpinionComments{ get; set; }

        /// <summary>
        /// 审核意见控件ID
		/// </summary>
        public string CommentsControlId { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 节点Id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// 请求Url
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// 调用前客户端事件句柄
        /// </summary>
        public string BeforeClick { get; set; }

        /// <summary>
        /// 调用后客户端事件句柄
        /// </summary>
        public string AfterClick { get; set; }

        /// <summary>
        /// 是否允许从候选人中选择执行人
        /// </summary>
        public bool IsSelectCandidates { get; set; }
        
        /// <summary>
        /// 是否允许允许分派给多个执行人
        /// </summary>
        public bool IsAssignToMultiUsers { get; set; }

        /// <summary>
        /// 邮件参数收集器，在发待办时使用，
        /// 不产生待办的控件忽略
        /// </summary>
        public MailCollector EMailCollector
        {
            get;
            set;
        }

        /// <summary>
        /// 上线文，如当前操作人等
        /// </summary>
        public WfClientRuntimeContext RuntimeContext
        {
            get;
            set;
        }
    }
}
