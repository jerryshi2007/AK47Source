using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [DataContract(IsReference = true)]
    public class ClientOguUser : ClientOguObjectBase, IClientUser
    {
        [DataMember]
        public string Email
        {
            get;
            set;
        }

        [DataMember]
        public bool IsSideline
        {
            get;
            set;
        }

        [DataMember]
        public string LogOnName
        {
            get;
            set;
        }

        [DataMember]
        public string Occupation
        {
            get;
            set;
        }

        [DataMember]
        public ClientUserRankType Rank
        {
            get;
            set;
        }
    }
}
