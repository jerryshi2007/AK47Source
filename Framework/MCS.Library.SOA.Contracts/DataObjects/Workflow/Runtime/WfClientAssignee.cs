using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
	[Serializable]
	public class WfClientAssignee
	{
        private ClientOguUser _User;

        [DataMember]
        public WfClientAssigneeType AssigneeType { get; set; }

        [DataMember]
        public ClientOguUser User
        {
            get
            {
                if (_User == null)
                    _User = new ClientOguUser();
                return this._User;
            }
            set
            {
                this._User = value;
            }
        }
	}

	[CollectionDataContract(IsReference=true)]
    public class WfClientAssigneeCollection : EditableDataObjectCollectionBase<WfClientAssignee>
	{

	}
}
