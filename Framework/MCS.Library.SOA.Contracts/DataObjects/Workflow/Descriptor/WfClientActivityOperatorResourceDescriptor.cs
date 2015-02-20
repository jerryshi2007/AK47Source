using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientActivityOperatorResourceDescriptor : WfClientResourceDescriptor
    {
        private string _ActivityKey = string.Empty;
        [DataMember]
        public string ActivityKey
        {
            get { return this._ActivityKey; }
            set { this._ActivityKey = value; }
        }


       
       
    }
}
