using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    /// <summary>
    /// 流转目标的描述
    /// </summary>
    public class WFMoveToTargetParameter
    {
        private List<WfClientAssignee> _Candidates = null;
        public string ActionResult { get; set; }
        public string ActivityKey
        {
            get;
            set;
        }

        public string TransitionKey
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许从候选人中选择执行人
        /// </summary>
        public bool IsSelectCandidates
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许允许分派给多个执行人
        /// </summary>
        public bool IsAssignToMultiUsers
        {
            get;
            set;
        }

        public List<WfClientAssignee> Candidates
        {
            get
            {
                if (this._Candidates == null)
                    this._Candidates = new List<WfClientAssignee>();

                return this._Candidates;
            }
            set
            {
                this._Candidates = value;
            }
        }
    }
}
