using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [Serializable]
    [DataContract(IsReference=true)]
    [KnownType(typeof(WfClientAssignee))]
    public class WfClientProcessStartupParams
    {
        private WfClientProcessDescriptor _ProcessDescriptor = null;
        private ClientOguUser _Creator = null;
        private ClientOguOrganization _Department = null;
        private string _ResourceID = string.Empty;
        private string _DefaultTaskTitle = string.Empty;
        private WfClientAssigneeCollection _Assignees = null;
        private NameValueCollection _RelativeParams = null;

        [DataMember]
        public WfClientProcessDescriptor ProcessDescriptor
        {
            get
            {
                if (this._ProcessDescriptor == null)
                {
                    this._ProcessDescriptor = new WfClientProcessDescriptor();
                }
                return this._ProcessDescriptor;
            }
            set { _ProcessDescriptor = value; }
        }
     
        [DataMember]
        public ClientOguUser Creator
        {
            get
            {
                if (this._Creator == null)
                {
                    this._Creator = new ClientOguUser();
                }
                return this._Creator;
            }
            set
            {
                this._Creator = value;
            }
        }
 
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
            set
            {
                this._Department = value;
            }
        }

        /// <summary>
        /// 是否自动启动第一个活动
        /// </summary>
        [DataMember]
        public bool AutoStartInitialActivity
        {
            get;
            set;
        }

         [DataMember]
        public string ResourceID
        {
            get { return this._ResourceID; }
            set { this._ResourceID = value; }
        }

         [DataMember]
         public string DefaultTaskTitle
        {
            get { return this._DefaultTaskTitle; }
            set { this._DefaultTaskTitle = value; }
        }
         
         [DataMember]
         public WfClientAssigneeCollection Assignees
         {
             get
             {
                 if (this._Assignees == null)
                 {
                     this._Assignees = new WfClientAssigneeCollection();
                 }
                 return this._Assignees;
             }
             set { this._Assignees = value; }
         }

        //[DataMember]
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

        #region 创建分支流程时需要的参数，内部使用
        internal WfClientBranchProcessStartupParams BranchStartupParams
        {
            get;
            set;
        }

        internal WfClientBranchProcessGroup Group
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
}
