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
    public class WfClientGroup : WfClientOguObjectBase
    {
        public WfClientGroup()
            : base(ClientOguSchemaType.Groups)
        {
        }

        public WfClientGroup(string id)
            : base(id, ClientOguSchemaType.Groups)
        {
        }

        public WfClientGroup(string id, string name)
            : base(id, name, ClientOguSchemaType.Groups)
        {
        }
    }
}
