using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientUserOperationLog
    {
        WfClientOperationType _OperationType = WfClientOperationType.Update;

        public Int64 ID
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

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

        public string ProcessID
        {
            get;
            set;
        }

        public string ActivityID
        {
            get;
            set;
        }

        public string ActivityName
        {
            get;
            set;
        }

        public WfClientUser Operator
        {
            get;
            set;
        }

        public WfClientOrganization TopDepartment
        {
            get;
            set;
        }

        public DateTime? OperationDateTime
        {
            get;
            set;
        }

        public WfClientOperationType OperationType
        {
            get
            {
                return this._OperationType;
            }
            set
            {
                this._OperationType = value;
            }
        }

        public string OperationName
        {
            get;
            set;
        }

        public string OperationDescription
        {
            get;
            set;
        }

        public string HttpContextString
        {
            get;
            set;
        }

        public WfClientUser RealUser
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string CorrelationID
        {
            get;
            set;
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientUserOperationLogCollection : EditableDataObjectCollectionBase<WfClientUserOperationLog>
    {
    }
}
