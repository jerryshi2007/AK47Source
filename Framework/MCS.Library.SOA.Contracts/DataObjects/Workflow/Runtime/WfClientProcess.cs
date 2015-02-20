using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using MCS.Library.Core;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
	[DataContract(IsReference=true)]
	public class WfClientProcess
	{
        private string _ID;
        private WfClientProcessDescriptor _Descriptor = null;
        private WfClientProcessStatus _Status = WfClientProcessStatus.NotRunning;
        private ClientDataLoadingType _LoadingType = ClientDataLoadingType.Memory;
        private WfClientProcessContext _Context = null;
        private WfClientApplicationRumtimeParameters _ApplicationRumtimeParameters = null;
        private WfClientActivityCollection _Activities = null;
        private DateTime _StartTime = DateTime.MinValue;
        private DateTime _EndTime = DateTime.MinValue;
       

        private WfClientActivity _InitialActivity = null;
        private WfClientActivity _CurrentActivity = null;
        private WfClientActivity _CompletedActivity = null;

        private ClientOguUser _Creator = null;
        private ClientOguOrganization _OwnerDepartment = null;

        private string _OwnerActivityID = null;
        private string _OwnerTemplateKey = null;
        private int _Sequence = -1;
        private WfClientBranchProcessStartupParams _BranchStartupParams = null;
        private NameValueCollection _RelativeParams = null;
        private string _ResourceID = string.Empty;
        private string _DefaultTaskTitle = string.Empty;

		[DataMember]
		public string ID
		{
            get { return this._ID; }
            set { this._ID = value; }
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
		public WfClientProcessDescriptor Descriptor
		{
            get
            {
                if (this._Descriptor == null)
                    this._Descriptor = new WfClientProcessDescriptor();
                return this._Descriptor;
            }
            set
            {
                this._Descriptor = value;
            }
		}

        [DataMember]
        public WfClientProcessStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }

        [DataMember]
        public DateTime StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
            }
        }

        [DataMember]
        public DateTime EndTime
        {
            get
            {
                return this._EndTime;
            }
            set
            {
                this._EndTime = value;
            }
        }

        /// <summary>
        /// 更新标记
        /// </summary>
        [DataMember]
        public int UpdateTag
        {
            get;
            set;
        }

        [DataMember]
        public ClientOguUser Creator
        {
            get
            {
                if (this._Creator == null)
                    this._Creator = new ClientOguUser();
                return this._Creator;
            }
            set
            {
                this._Creator = value;
            }
        }

        [DataMember]
        public ClientOguOrganization OwnerDepartment
        {
            get
            {
                if (this._OwnerDepartment == null)
                {
                    this._OwnerDepartment = new ClientOguOrganization();
                }
                return this._OwnerDepartment;
            }
            set
            {
                this._OwnerDepartment = value;
            }
        }

        [DataMember]
        public WfClientProcessContext Context
        {
            get
            {
                if (this._Context == null)
                    this._Context = new WfClientProcessContext();

                return this._Context;
            }
            set
            {
                this._Context = value;
            }
        }

        [DataMember]
        public WfClientApplicationRumtimeParameters ApplicationRumtimeParameters
        {
            get
            {
                if (this._ApplicationRumtimeParameters == null)
                    this._ApplicationRumtimeParameters = new WfClientApplicationRumtimeParameters();

                return this._ApplicationRumtimeParameters;
            }
            set
            {
                this._ApplicationRumtimeParameters = value;
            }
        }

        [DataMember]
        public ClientDataLoadingType LoadingType
        {
            get
            {
                return this._LoadingType;
            }
            set
            {
                this._LoadingType = value;
            }
        }

        [DataMember]
        public WfClientActivityCollection Activities
        {
            get
            {
                if (this._Activities == null)
                    this._Activities = new WfClientActivityCollection();

                return this._Activities;
            }
            set
            {
                this._Activities = value;
            }
        }

       [DataMember]
        public WfClientBranchProcessStartupParams BranchStartupParams
        {
            get {
                if (this._BranchStartupParams == null)
                {
                    this._BranchStartupParams = new WfClientBranchProcessStartupParams();
                }
                return this._BranchStartupParams; }
            set { this._BranchStartupParams = value; }
        }

        private WfClientActivityCollection _ElapsedActivities = new WfClientActivityCollection();

        /// <summary>
        /// 顺序返回流程中已执行过的一组活动节点集合
        /// </summary>
        [DataMember]
        public WfClientActivityCollection ElapsedActivities
        {
            get
            {
                if (this._ElapsedActivities == null)
                {
                    this._ElapsedActivities = new WfClientActivityCollection();
                }
                return this._ElapsedActivities;
            }
            set
            {
                this._ElapsedActivities = value;
            }
        }

        [DataMember]
        public WfClientActivity InitialActivity
        {
            get
            {
                if (this._InitialActivity == null)
                {
                    //this._InitialActivity = new WfClientActivity();
                }
                return this._InitialActivity;
            }
            set
            {
                this._InitialActivity = value;
            }
        }

        [DataMember]
        public WfClientActivity CurrentActivity
        {
            get
            {
               
                return this._CurrentActivity;
            }
            set
            {
                this._CurrentActivity = value;
            }
        }

        [DataMember]
        public WfClientActivity CompletedActivity
        {
            get
            {
               
                return this._CompletedActivity;
            }
            set
            {
                this._CompletedActivity = value;
            }
        }

        [DataMember]
        public int Sequence
        {
            get
            {
                return this._Sequence;
            }
            set
            {
                this._Sequence = value;
            }
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

    

       
        #region 此属性不需要
        //[DataMember]
        //public bool HasParentProcess
        //{
        //    get
        //    {
        //        return string.IsNullOrEmpty(this.OwnerActivityID) && string.IsNullOrEmpty(this.OwnerTemplateKey);
        //    }
        //    set
        //    {

        //    }
        //}
       // [DataMember]
       // public WfClientBranchProcessGroup EntryInfo
       // {
       //     get
       //     {
       //         if (this._EntryInfo == null)
       //         {
       //             this._EntryInfo = new WfClientBranchProcessGroup();
       //         }
       //         return this._EntryInfo;
       //     }
       //     set
       //     {
       //         this._EntryInfo = value;
       //     }
       // }

       //// [DataMember]
       // public WfClientProcess RootProcess
       // {
       //     get
       //     {
       //         if (this._RootProcess == null)
       //         {
       //             this._RootProcess = new WfClientProcess();
       //         }
       //         return this._RootProcess;
       //     }
       //     set
       //     {
       //         this._RootProcess = value;
       //     }
       // }

       //// [DataMember]
       // public WfClientProcess SameResourceRootProcess
       // {
       //     get
       //     {
       //         return this._SameResourceRootProcess;
       //     }
       //     set
       //     {
       //         this._SameResourceRootProcess = value;
       //     }
       // }

       //// [DataMember]
       // public WfClientProcess ApprovalRootProcess
       // {
       //     get
       //     {
       //         return this._ApprovalRootProcess;
       //     }
       //     set
       //     {
       //         this._ApprovalRootProcess = value;
       //     }
       // }
        #endregion

       
      

        [DataMember]
        public string OwnerActivityID
        {
            get { return this._OwnerActivityID; }
            set { this._OwnerActivityID = value; }
        }

        [DataMember]
        public string OwnerTemplateKey
        {
            get { return this._OwnerTemplateKey; }
            set { this._OwnerTemplateKey = value; }
        }
	}

	[CollectionDataContract(IsReference=true)]
    [Serializable]
	public class WfClientProcessCollection : EditableKeyedDataObjectCollectionBase<string, WfClientProcess>
	{
		protected override string GetKeyForItem(WfClientProcess item)
		{
			return item.ID;
		}
	}

}
