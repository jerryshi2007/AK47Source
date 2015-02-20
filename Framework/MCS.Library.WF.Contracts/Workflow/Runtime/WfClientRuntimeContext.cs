using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    /// <summary>
    /// 流转等操作时的额外信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientRuntimeContext
    {
        private bool _AutoCalculate = false;
        private Dictionary<string, object> _ApplicationRuntimeParameters = null;
        private Dictionary<string, object> _ProcessContext = null;
        private bool _AutoPersist = true;
        public WfClientRuntimeContext()
        {
        }

        public WfClientRuntimeContext(WfClientUser opUser)
        {
            this.Operator = opUser;
        }

        /// <summary>
        /// 是否根据流程上下文参数自动计算流程环节和候选人
        /// </summary>
        public bool AutoCalculate
        {
            get
            {
                return this._AutoCalculate;
            }
            set
            {
                this._AutoCalculate = value;
            }
        }

        /// <summary>
        /// 流程上下文参数
        /// </summary>
        public Dictionary<string, object> ApplicationRuntimeParameters
        {
            get
            {
                if (this._ApplicationRuntimeParameters == null)
                    this._ApplicationRuntimeParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ApplicationRuntimeParameters;
            }
        }

        public Dictionary<string, object> ProcessContext
        {
            get
            {
                if (this._ProcessContext == null)
                    this._ProcessContext = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ProcessContext;
            }
        }

        /// <summary>
        /// 当前操作人
        /// </summary>
        public WfClientUser Operator
        {
            get;
            set;
        }

        /// <summary>
        /// 当前需要保存的意见
        /// </summary>
        public WfClientOpinion Opinion
        {
            get;
            set;
        }

        /// <summary>
        /// 待办的标题
        /// </summary>
        public string TaskTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 通知的标题
        /// </summary>
        public string NotifyTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 流程是否持久化（默认是True）
        /// </summary>
        public bool AutoPersist
        {
            get
            {
                return this._AutoPersist;
            }
            set
            {
                this._AutoPersist = value;
            }
        }
    }
}
