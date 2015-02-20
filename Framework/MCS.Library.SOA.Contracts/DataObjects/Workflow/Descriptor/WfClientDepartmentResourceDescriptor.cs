using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference=true)]
    public class WfClientDepartmentResourceDescriptor : WfClientResourceDescriptor
    {
        private ClientOguOrganization _Department = null;

		public WfClientDepartmentResourceDescriptor() {}

        public WfClientDepartmentResourceDescriptor(ClientOguOrganization org)
		{
            this._Department =org;
		}
        [DataMember]
		public ClientOguOrganization Department
		{
			get { return this._Department; }
            set { this._Department = value; }
		}
    }
}
