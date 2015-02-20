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
    public class WfClientRole
    {
        public WfClientRole()
        {
        }

        public WfClientRole(string fullCodeName)
        {
            this.FullCodeName = fullCodeName;
        }

        public string FullCodeName
        {
            get;
            set;
        }
    }
}
