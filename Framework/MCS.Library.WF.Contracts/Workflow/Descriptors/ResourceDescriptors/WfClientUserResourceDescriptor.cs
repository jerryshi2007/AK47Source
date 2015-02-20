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
    public class WfClientUserResourceDescriptor : WfClientResourceDescriptor
    {
        public WfClientUserResourceDescriptor()
        {
        }

        public WfClientUserResourceDescriptor(WfClientUser user)
        {
            this.User = user;
        }

        public WfClientUser User
        {
            get;
            set;
        }

        public bool IsSameUser(WfClientUser user)
        {
            bool result = false;

            if (this.User != null && user != null)
                result = string.Compare(this.User.ID, user.ID, true) == 0;

            return result;
        }
    }
}
