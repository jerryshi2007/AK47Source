using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientDepartmentResourceDescriptor : WfClientResourceDescriptor
    {
        public WfClientDepartmentResourceDescriptor()
        {
        }

        public WfClientDepartmentResourceDescriptor(WfClientOrganization department)
        {
            this.Department = department;
        }

        public WfClientOrganization Department
        {
            get;
            set;
        }
    }
}
