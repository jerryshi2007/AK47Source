using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
	public class WfClientTransitionDescriptor : WfClientKeyedDescriptorBase
	{
        private string _FromActivityKey;

        [DataMember]
        public string FromActivityKey
        {
            get { return _FromActivityKey; }
            set { _FromActivityKey = value; }
        }
        private string _ToActivityKey;

        [DataMember]
        public string ToActivityKey
        {
            get { return _ToActivityKey; }
            set { _ToActivityKey = value; }
        }
        private WfClientActivityDescriptor _FromActivity;
        private WfClientActivityDescriptor _ToActivity;
        private WfClientVariableDescriptorCollection _Variables = null;
        private WfClientConditionDescriptor _Condition = null;

 
        [DataMember]
		public WfClientActivityDescriptor ToActivity
		{
            get
            {
                if (this._ToActivity == null)
                    this._ToActivity = new WfClientActivityDescriptor();

                return _ToActivity;
            }
            set
            {
                _ToActivity = value;
            }
		}
        [DataMember]
		public WfClientActivityDescriptor FromActivity
		{
            get
            {
                if (this._FromActivity == null)
                    this._FromActivity = new WfClientActivityDescriptor();

                return _FromActivity;
            }
            set
            {
                _FromActivity= value;
            }
		}
        [DataMember]
		public WfClientVariableDescriptorCollection Variables
		{
            get
            {
                if (this._Variables == null)
                    this._Variables = new WfClientVariableDescriptorCollection(this);

                return _Variables;
            }
            set 
            {
                _Variables = value;
            }

		}       
        [DataMember]
        public WfClientConditionDescriptor Condition
        {
            get
            {
                if (this._Condition == null)
                    this._Condition = new WfClientConditionDescriptor(this);

                return _Condition;
            }
            set
            {
                _Condition = value;
            }
        }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public bool DefaultSelect { get; set; }


       
	}

    [CollectionDataContract(IsReference=true)]
	public class WfClientTransitionDescriptorCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientTransitionDescriptor>
	{
        internal WfClientTransitionDescriptorCollection(WfClientKeyedDescriptorBase owner)
            : base(owner)
        { }
        public WfClientTransitionDescriptorCollection()
        { }
       
		protected override string GetKeyForItem(WfClientTransitionDescriptor item)
		{
			return item.Key;
		}
	}

   
}
