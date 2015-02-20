using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [Serializable]
    [CollectionDataContract(IsReference= true)]
    public class WfClientApplicationRumtimeParameters : Dictionary<string, object>
    {
        private Dictionary<string, object> _Dictionary = null;
        public Dictionary<string, object> Dictionary
        {
            get
            {
                if (this._Dictionary == null)
                {
                    this._Dictionary = new Dictionary<string, object>();
                }
                return this._Dictionary;
            }
            set
            {
                this._Dictionary = value;
            }
        }
    }
}
