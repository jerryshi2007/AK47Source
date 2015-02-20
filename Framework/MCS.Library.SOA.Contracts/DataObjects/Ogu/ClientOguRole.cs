using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [DataContract(IsReference=true)]
    [Serializable]
    public class ClientOguRole : ClientOguObjectBase
    {
        [DataMember]
        public string FullCodeName
        {
            get;
            set;
        }

        [DataMember]
        public string CodeName
        {
            get;
            set;
        }

    }
}
