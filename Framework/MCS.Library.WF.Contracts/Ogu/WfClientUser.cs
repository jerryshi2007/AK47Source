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
    public class WfClientUser : WfClientOguObjectBase
    {
        public WfClientUser()
            : base(ClientOguSchemaType.Users)
        {
        }

        public WfClientUser(string id)
            : base(id, ClientOguSchemaType.Users)
        {
        }

        public WfClientUser(string id, string name)
            : base(id, name, ClientOguSchemaType.Users)
        {
        }
    }
}
