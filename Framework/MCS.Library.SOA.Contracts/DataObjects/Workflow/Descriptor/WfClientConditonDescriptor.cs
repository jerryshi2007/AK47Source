using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
	public class WfClientConditionDescriptor
	{
        private WfClientKeyedDescriptorBase _Owner;
        private string _Expression = string.Empty;

        internal WfClientConditionDescriptor(WfClientKeyedDescriptorBase owner)
        {
            _Owner = owner;
        }
        public WfClientConditionDescriptor() { }

        [DataMember]
        public WfClientKeyedDescriptorBase Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        [DataMember]
        public string Expression
        {
            get { return this._Expression; }
            set { this._Expression = value; }
        }

       
	}
	
	

}
