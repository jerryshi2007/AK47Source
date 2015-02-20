using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
   [KnownType(typeof(WfClientInitialActivity))]
   [KnownType(typeof(WfClientNormalActivity))]
    [KnownType(typeof(WfClientCompletedActivity))]
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientActivity
    {
        private string _ID = string.Empty;
        private string _ProcessKey = string.Empty;
        private string _TaskTitle = string.Empty;
        private WfClientActivityDescriptor _Descriptor;

        private WfClientActivityContext _Context = null;
        private WfClientActivityStatus _Status = WfClientActivityStatus.NotRunning;
        private WfClientProcess _Process = null;

        private DateTime _StartTime = DateTime.MinValue;
        private DateTime _EndTime = DateTime.MinValue;
        private WfClientAssigneeCollection _Assignees = null;
        private WfClientAssigneeCollection _Candidates = null;
        private ClientOguUser _Operator = null;
        private string _ProcessDescKey = string.Empty;
        private string _ActivityDescKey = string.Empty;
        private ClientDataLoadingType _LoadingType = ClientDataLoadingType.Memory;
        private string _CreatorInstanceID = string.Empty;
      


        public WfClientActivity()
        {
        }

        public WfClientActivity(WfClientActivityDescriptor descriptor)
        {
            _Descriptor = descriptor;

            this._ProcessDescKey = descriptor.Process.Key;
            this._ActivityDescKey = descriptor.Key;
            this._StartTime = DateTime.Now;
        }

        [DataMember]
        public string ActivityDescKey
        {
            get { return _ActivityDescKey; }
            set { _ActivityDescKey = value; }
        }
       

        [DataMember]
        public string ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }
        [DataMember]
        public string ProcessKey
        {
            get { return this._ProcessKey; }
            set { this._ProcessKey = value; }
        }

        [DataMember]
        public WfClientActivityDescriptor XDescriptor
        {
            get {
                if (_Descriptor == null)
                    _Descriptor = new WfClientActivityDescriptor();
                return this._Descriptor; }
            set { _Descriptor = value; }
        }
        [DataMember]
        public WfClientProcess Process
        {
            get {
                if (_Process == null)
                    _Process = new WfClientProcess();
                return _Process; }
            set { _Process = value; }
        }
        [DataMember]
        public WfClientActivityStatus Status
        {
            get
            {
               
                   
                return this._Status;
            }
            set { _Status = value; }
        }
        [DataMember]
        public string TaskTitle
        {
            get { return this._TaskTitle; }
            set { _TaskTitle=value;}
        }
        [DataMember]
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        [DataMember]
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        [DataMember]
        public ClientDataLoadingType LoadingType
        {
            get { return _LoadingType; }
            set { _LoadingType = value; }
        }
        //[DataMember]
        public WfClientActivityContext Context
        {
            get
            {
                if (_Context == null)
                    _Context = new WfClientActivityContext();

                return _Context;
            }
            set { _Context = value; }
        }

        /// <summary>
        /// 该环节的候选人
        /// </summary>
        [DataMember]
        public WfClientAssigneeCollection Candidates
        {
            get
            {
                if (this._Candidates == null)
                    this._Candidates = new WfClientAssigneeCollection();

                return this._Candidates;
            }
            set { _Candidates = value; }
        }
        [DataMember]
        public WfClientAssigneeCollection Assignees
        {
            get
            {
                if (_Assignees == null)
                    _Assignees = new WfClientAssigneeCollection();

                return _Assignees;
            }
            set { _Assignees=value; }
        }
        [DataMember]
        public ClientOguUser Operator
        {
            get
            {
                if (_Operator == null)
                    _Operator = new ClientOguUser();
                return _Operator;
            }
            set
            {
                this._Operator = value;
            }
        }
        [DataMember]
        public bool CanMoveTo
        {
            get;
            set;
        }
        [DataMember]
        public string CreatorInstanceID
        {
            get
            {
                return this._CreatorInstanceID;
            }
            set
            {
                this._CreatorInstanceID = value;
            }
        }

        private WfClientBranchProcessGroupCollection _BranchProcessGroups = null;
        [DataMember]
        public WfClientBranchProcessGroupCollection BranchProcessGroups
        {
            get
            {
                if (this._BranchProcessGroups == null)
                    this._BranchProcessGroups = new WfClientBranchProcessGroupCollection();

                return this._BranchProcessGroups;
            }
            set { this._BranchProcessGroups = value; }
        }
        [DataMember]
        public WfClientActivityType ActivityType { get; set; }



      
       
    }
}
       