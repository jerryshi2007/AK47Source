using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [KnownType(typeof(WfClientTransferParams))]

    [DataContract(IsReference = true)]
    [Serializable]
    public abstract class WfClientTransferParamsBase
    {
        private WfClientProcess _processinstance;

        public WfClientProcess Processinstance
        {
            get { return _processinstance; }
            set { _processinstance = value; }
        }
        [DataMember]
        public string InstanceID
        {
            get;
            set;
        }
        private WfClientActivityDescriptor _NextActivityDescriptor = null;
        private ClientOguUser _Operator = null;
        private WfClientAssigneeCollection _Assignees = null;
        private WfClientTransitionDescriptor _FromTransitionDescriptor = null;

        public WfClientTransferParamsBase(WfClientActivityDescriptor nextActivityDescriptor)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nextActivityDescriptor != null, "nextActivityDescriptor");

            _NextActivityDescriptor = nextActivityDescriptor;
        }

        [DataMember]
        public WfClientActivityDescriptor NextActivityDescriptor
        {
            get
            {
                if (this._NextActivityDescriptor == null)
                {
                    this._NextActivityDescriptor = new WfClientActivityDescriptor();
                }
                return _NextActivityDescriptor;
            }
            set
            {
                _NextActivityDescriptor = value;
            }
        }

        [DataMember]
        public ClientOguUser Operator
        {
            get
            {
                if (this._Operator == null)
                {
                    this._Operator = new ClientOguUser();
                }
                return _Operator;
            }
            set
            {
                _Operator = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public WfClientAssigneeCollection Assignees
        {
            get 
            {
                if (this._Assignees == null)
                    this._Assignees = new WfClientAssigneeCollection();

                return _Assignees;
            }
            set 
            {
                this._Assignees = value;
            }
        }

        /// <summary>
        /// 来自于哪里的线的定义
        /// </summary>
       //[DataMember]
        public WfClientTransitionDescriptor FromTransitionDescriptor
        {
            get
            {
                if (this._FromTransitionDescriptor == null)
                {
                    this._FromTransitionDescriptor = new WfClientTransitionDescriptor();
                }
                return _FromTransitionDescriptor;
            }
            set
            {
                _FromTransitionDescriptor = value;
            }
        }
    }

    /// <summary>
    /// 普通流程的流转参数
    /// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientTransferParams : WfClientTransferParamsBase
    {
        private WfClientBranchProcessTransferParamsCollection _BranchTransferParams = null;
        [DataMember]
        public WfClientBranchProcessTransferParamsCollection BranchTransferParams
        {
            get
            {
                if (this._BranchTransferParams == null)
                {
                    this._BranchTransferParams = new WfClientBranchProcessTransferParamsCollection();
                }
                return this._BranchTransferParams;
            }
            set
            {
                this._BranchTransferParams = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextActivityDescriptor"></param>
        public WfClientTransferParams(WfClientActivityDescriptor nextActivityDescriptor)
            : base(nextActivityDescriptor)
        {
        }
    }

    /// <summary>
    /// 分支流程的流转参数
    /// </summary>
    //[Serializable]
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientBranchProcessTransferParams
    {
        private WfClientBranchProcessTemplateDescriptor _Template = null;
        private WfClientBranchProcessStartupParamsCollection _BranchParams = null;

        public WfClientBranchProcessTransferParams()
        {
        }

        public WfClientBranchProcessTransferParams(WfClientBranchProcessTemplateDescriptor template)
        {
            //template.NullCheck("template");

            //this._Template = template;

            //OguDataCollection<IClientUser> users = template.Resources.ToUsers();

            //if (template.ExecuteSequence == WfBranchProcessExecuteSequence.SerialInSameProcess && users.Count > 0)
            //{
            //    this.BranchParams.Add(users[0]);
            //}
            //else
            //{
            //    WfClientProcessDescriptor processDesp = template.GetBranchProcessDescriptor();

            //    if (processDesp.InitialActivity != null)
            //    {
            //        OguDataCollection<IUser> usersInInitialActivity = processDesp.InitialActivity.Resources.ToUsers();

            //        if (usersInInitialActivity.Count > 0)
            //            users = usersInInitialActivity;
            //    }

            //    this.BranchParams.Add(users);
            //}
        }

        public WfClientBranchProcessTransferParams(WfClientBranchProcessTemplateDescriptor template, IEnumerable<ClientOguUser> users)
        {
            template.NullCheck("template");
            users.NullCheck("users");
            this._Template = template;
            WfClientBranchProcessStartupParams branch = new WfClientBranchProcessStartupParams(users);
            this.BranchParams.Add(branch);
        }

        /// <summary>
        /// 分支流程的具体参数
        /// </summary>
        [DataMember]
        public WfClientBranchProcessStartupParamsCollection BranchParams
        {
            get
            {
                if (this._BranchParams == null)
                {
                    this._BranchParams = new WfClientBranchProcessStartupParamsCollection();
                }
                return this._BranchParams; 
            }
            set
            {
                this._BranchParams = value;
            }
        }
        [DataMember]
        public WfClientBranchProcessTemplateDescriptor Template
        {
            get 
            {
                if (this._Template == null)
                {
                    this._Template = new WfClientBranchProcessTemplateDescriptor();
                }
                return this._Template;
            }
            set { this._Template = value; }
        }
    }
   
    //[Serializable]
    [CollectionDataContract(IsReference = true)]
    [Serializable]
    public class WfClientBranchProcessTransferParamsCollection : EditableKeyedDataObjectCollectionBase<string, WfClientBranchProcessTransferParams>
    {
        protected override string GetKeyForItem(WfClientBranchProcessTransferParams branchProcessTransferParams)
        {
            branchProcessTransferParams.NullCheck("branchProcessTransferParams");
            branchProcessTransferParams.Template.NullCheck("branchProcessTransferParams.Template");

            //return branchProcessTransferParams.Template.Key;
            return "";
        }

        public void InitFromTemplates(IEnumerable<WfClientBranchProcessTemplateDescriptor> templates)
        {
            this.Clear();

            foreach (WfClientBranchProcessTemplateDescriptor template in templates)
            {
                this.Add(new WfClientBranchProcessTransferParams(template));
            }
        }
    }
}
