using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using System.Collections.Specialized;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfProcessStartupParams
    {
        private bool _AutoCommit = false;

        public WfProcessStartupParams(WfServiceStartupProcessParams serviceOP_Paramas, string ProcessDespKey)
        {
            this.ProcessDescriptor = WfProcessDescriptorManager.GetDescriptor(ProcessDespKey);
            this.ProcessDescriptor.Url = serviceOP_Paramas.DefaultURL;
            this.Assignees.Add(serviceOP_Paramas.Assignees);
            this.AutoStartInitialActivity = serviceOP_Paramas.AutoStartInitialActivity;
            this.Creator = serviceOP_Paramas.Creator;
            this.DefaultTaskTitle = serviceOP_Paramas.DefaultTaskTitle;
            this.Department = serviceOP_Paramas.Department;
            this.ResourceID = serviceOP_Paramas.ResourceID;
            this.RelativeID = serviceOP_Paramas.RelativeID;
            this.RelativeURL = serviceOP_Paramas.RelativeURL;
            this.OwnerActivityID = serviceOP_Paramas.OwnerActivityID;
            this.OwnerTemplateKey = serviceOP_Paramas.OwnerTemplateKey;
            this.CheckStartProcessUserPermission = false;
            this.RuntimeProcessName = serviceOP_Paramas.RuntimeProcessName;
            this.AutoCommit = serviceOP_Paramas.AutoCommit;
            this.DefaultUrl = serviceOP_Paramas.DefaultUrl;
        }

        public WfProcessStartupParams()
        { }

        public IWfProcessDescriptor ProcessDescriptor { get; set; }

        private Dictionary<string, object> _ApplicationRuntimeParameters = null;

        public Dictionary<string, object> ApplicationRuntimeParameters
        {
            get
            {
                if (this._ApplicationRuntimeParameters == null)
                    this._ApplicationRuntimeParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return this._ApplicationRuntimeParameters;
            }
        }

        /// <summary>
        /// 流程启动时设置流程Committed的属性
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return this._AutoCommit;
            }
            set
            {
                this._AutoCommit = value;
            }
        }

        private IUser _Creator = null;

        public IUser Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        private IOrganization _Department = null;

        public IOrganization Department
        {
            get
            {
                return this._Department;
            }
            set
            {
                this._Department = (IOrganization)OguUser.CreateWrapperObject(value);
            }
        }

        private bool _AutoStartInitialActivity = true;

        /// <summary>
        /// 是否自动启动第一个活动
        /// </summary>
        public bool AutoStartInitialActivity
        {
            get
            {
                return this._AutoStartInitialActivity;
            }
            set
            {
                this._AutoStartInitialActivity = value;
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

        public string DefaultUrl
        {
            get;
            set;
        }

        public string RuntimeProcessName
        {
            get;
            set;
        }

        public string RelativeID
        {
            get;
            set;
        }

        public string RelativeURL
        {
            get;
            set;
        }

        private WfAssigneeCollection _Assignees = new WfAssigneeCollection();

        public WfAssigneeCollection Assignees
        {
            get
            {
                return this._Assignees;
            }
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

        private bool _CheckStartProcessUserPermission = true;

        public bool CheckStartProcessUserPermission
        {
            get
            {
                return this._CheckStartProcessUserPermission;
            }
            set
            {
                this._CheckStartProcessUserPermission = value;
            }
        }


        #region 创建分支流程时需要的参数，内部使用
        internal WfBranchProcessStartupParams BranchStartupParams
        {
            get;
            set;
        }

        internal IWfBranchProcessGroup Group
        {
            get;
            set;
        }

        internal string OwnerActivityID
        {
            get;
            set;
        }

        internal string OwnerTemplateKey
        {
            get;
            set;
        }

        internal int Sequence
        {
            get;
            set;
        }
        #endregion
    }

    [Serializable]
    public class WfServiceStartupProcessParams
    {
        private bool _AutoCommit = false;

        /// <summary>
        /// 流程启动时设置流程Committed的属性
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return this._AutoCommit;
            }
            set
            {
                this._AutoCommit = value;
            }
        }

        private IUser _Creator = null;

        public IUser Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        private IOrganization _Department = null;

        public IOrganization Department
        {
            get
            {
                return this._Department;
            }
            set
            {
                this._Department = (IOrganization)OguUser.CreateWrapperObject(value);
            }
        }

        public string DefaultURL
        {
            get;
            set;
        }

        private bool _AutoStartInitialActivity = true;

        /// <summary>
        /// 是否自动启动第一个活动
        /// </summary>
        public bool AutoStartInitialActivity
        {
            get { return _AutoStartInitialActivity; }
            set { _AutoStartInitialActivity = value; }
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

        public string DefaultUrl
        {
            get;
            set;
        }

        public string RelativeID
        {
            get;
            set;
        }

        public string RelativeURL
        {
            get;
            set;
        }

        private IEnumerable<IUser> _Assignees = null;

        public IEnumerable<IUser> Assignees
        {
            get { return _Assignees; }
            set { _Assignees = value; }
        }

        public string OwnerActivityID
        {
            get;
            set;
        }

        public string OwnerTemplateKey
        {
            get;
            set;
        }

        public string RuntimeProcessName
        {
            get;
            set;
        }

        internal int Sequence
        {
            get;
            set;
        }

        public IDictionary<object, object> RelativeParams
        {
            get;
            set;
        }

    }
}
