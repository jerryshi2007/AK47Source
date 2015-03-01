using MCS.Library.Data.Mapping;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.DataObjects
{
    [Serializable]
    public class WfClientProcessQueryCondition
    {
        [ConditionMapping("APPLICATION_NAME", "LIKE")]
        public string ApplicationName
        {
            get;
            set;
        }

        [ConditionMapping("PROCESS_NAME", "LIKE")]
        public string ProcessName
        {
            get;
            set;
        }

        [ConditionMapping("DEPARTMENT_NAME", "LIKE")]
        public string DepartmentName
        {
            get;
            set;
        }

        [ConditionMapping("START_TIME", ">=")]
        public DateTime BeginStartTime
        {
            get;
            set;
        }

        [ConditionMapping("START_TIME", "<")]
        public DateTime EndStartTime
        {
            get;
            set;
        }

        [ConditionMapping("STATUS")]
        public string ProcessStatus
        {
            get;
            set;
        }

        [NoMapping]
        private WfClientAssigneeExceptionFilterType _AssigneeExceptionFilterType = WfClientAssigneeExceptionFilterType.All;

        [NoMapping]
        public WfClientAssigneeExceptionFilterType AssigneeExceptionFilterType
        {
            get
            {
                return this._AssigneeExceptionFilterType;
            }
            set
            {
                this._AssigneeExceptionFilterType = value;
            }
        }

        private WfClientAssigneesFilterType _AssigneesSelectType = WfClientAssigneesFilterType.CurrentActivity;

        [NoMapping]
        public WfClientAssigneesFilterType AssigneesSelectType
        {
            get
            {
                return this._AssigneesSelectType;
            }
            set
            {
                this._AssigneesSelectType = value;
            }
        }

        [NoMapping]
        public string AssigneesUserName
        {
            get;
            set;
        }

        private List<WfClientUser> _CurrentAssignees = null;

        [NoMapping]
        public List<WfClientUser> CurrentAssignees
        {
            get
            {
                if (this._CurrentAssignees == null)
                    this._CurrentAssignees = new List<WfClientUser>();

                return this._CurrentAssignees;
            }
        }
    }
}
