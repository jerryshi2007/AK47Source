using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Ogu
{
    [Serializable]
    [DataContract]
    public class WfClientOrganization : WfClientOguObjectBase
    {
        public WfClientOrganization()
            : base(ClientOguSchemaType.Organizations)
        {
        }

        public WfClientOrganization(string id)
            : base(id, ClientOguSchemaType.Organizations)
        {
        }

        public WfClientOrganization(string id, string name)
            : base(id, name, ClientOguSchemaType.Organizations)
        {
        }
    }
}
