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
        public string ApplicationName
        {
            get;
            set;
        }

        public string ProgramName
        {
            get;
            set;
        }

        public string ProcessName
        {
            get;
            set;
        }

        public string DepartmentName
        {
            get;
            set;
        }

        public DateTime BeginStartTime
        {
            get;
            set;
        }

        public DateTime EndStartTime
        {
            get;
            set;
        }

        public string ProcessStatus
        {
            get;
            set;
        }

        private WfClientAssigneeExceptionFilterType _AssigneeExceptionFilterType = WfClientAssigneeExceptionFilterType.All;

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

        public string AssigneesUserName
        {
            get;
            set;
        }

        private List<WfClientUser> _CurrentAssignees = null;

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
