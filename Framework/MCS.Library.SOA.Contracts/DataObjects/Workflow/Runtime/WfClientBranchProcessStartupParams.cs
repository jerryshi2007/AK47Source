using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference=true)]
    [Serializable]
    public class WfClientBranchProcessStartupParams
    {
        private WfClientAssigneeCollection _Assignees = null;
        private ClientOguOrganization _Department = null;

        private object _StartupContext = null;

        public WfClientBranchProcessStartupParams()
        {
        }

        public WfClientBranchProcessStartupParams(params ClientOguUser[] users)
        {
            //this.Assignees.Add(users);
        }

        public WfClientBranchProcessStartupParams(IEnumerable<ClientOguUser> users)
        {
            //this.Assignees.Add(users);
        }

        [DataMember]
        public object StartupContext
        {
            get
            {
                if (this._StartupContext == null)
                {
                    this._StartupContext = new object();
                }
                return this._StartupContext;
            }
            set { this._StartupContext = value; }
        }
        /// <summary>
        /// 分支流程的部门
        /// </summary>
        [DataMember]
        public ClientOguOrganization Department
        {
            get
            {
                if (this._Department == null)
                {
                    this._Department = new ClientOguOrganization();
                }
                return this._Department;
            }
            set { _Department = value; }
        }

        /// <summary>
        /// 分支流程的起始点的办理人
        /// </summary>
        [DataMember]
        public WfClientAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfClientAssigneeCollection();

                return this._Assignees;
            }
            set
            {
                this._Assignees = value;
            }
        }
    }

    [CollectionDataContract(IsReference=true)]
    [Serializable]
    public class WfClientBranchProcessStartupParamsCollection : EditableDataObjectCollectionBase<WfClientBranchProcessStartupParams>
    {

    }
}
