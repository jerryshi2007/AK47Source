using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientActivityAssigneesResourceDescriptor : WfClientResourceDescriptor
    {
        private string _ActivityKey = string.Empty;
       

        public WfClientActivityAssigneesResourceDescriptor()
        { }
        [DataMember]
        public string ActivityKey
        {
            get { return this._ActivityKey; }
            set { this._ActivityKey = value; }
        }
       
    }
}
