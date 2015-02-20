using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientBranchProcessStartupParams
    {
        private WfClientAssigneeCollection _Assignees = null;
        private WfClientOrganization _Department = null;
        private Dictionary<string, object> _ApplicationRuntimeParameters = null;

        private object _StartupContext = null;

        public WfClientBranchProcessStartupParams()
        {
        }

        /// <summary>
        /// 增加一组用户
        /// </summary>
        /// <param name="users"></param>
        public WfClientBranchProcessStartupParams(params WfClientUser[] users)
        {
            this.Assignees.Add(users);
        }

        /// <summary>
        /// 增加一组用户
        /// </summary>
        /// <param name="users"></param>
        public WfClientBranchProcessStartupParams(IEnumerable<WfClientUser> users)
        {
            this.Assignees.Add(users);
        }

        public Dictionary<string, object> ApplicationRuntimeParameters
        {
            get
            {
                if (this._ApplicationRuntimeParameters == null)
                    this._ApplicationRuntimeParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ApplicationRuntimeParameters;
            }
        }

        public string ResourceID
        {
            get;
            set;
        }

        public string DefaultTaskTitle
        {
            get;
            set;
        }

        private NameValueCollection _RelativeParams = null;

        public NameValueCollection RelativeParams
        {
            get
            {
                if (this._RelativeParams == null)
                    this._RelativeParams = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

                return this._RelativeParams;
            }
            set
            {
                this._RelativeParams = value;
            }
        }

        public object StartupContext
        {
            get
            {
                return this._StartupContext;
            }
            set
            {
                this._StartupContext = value;
            }
        }

        /// <summary>
        /// 分支流程的部门
        /// </summary>
        public WfClientOrganization Department
        {
            get
            {
                return this._Department;
            }
            set
            {
                this._Department = value;
            }
        }

        /// <summary>
        /// 分支流程的起始点的办理人
        /// </summary>
        public WfClientAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfClientAssigneeCollection();

                return this._Assignees;
            }
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientBranchProcessStartupParamsCollection : EditableDataObjectCollectionBase<WfClientBranchProcessStartupParams>
    {
        /// <summary>
        /// 根据一组用户创建分支流程启动参数
        /// </summary>
        /// <param name="users"></param>
        public void Add(params WfClientUser[] users)
        {
            Add((IEnumerable<WfClientUser>)users);
        }

        /// <summary>
        /// 根据一组用户创建分支流程启动参数
        /// </summary>
        /// <param name="users"></param>
        public void Add(IEnumerable<WfClientUser> users)
        {
            if (users != null)
            {
                foreach (WfClientUser user in users)
                {
                    this.Add(new WfClientBranchProcessStartupParams(user));
                }
            }
        }
    }
}
