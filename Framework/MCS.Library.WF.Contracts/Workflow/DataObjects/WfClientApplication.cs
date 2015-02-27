using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientApplication
    {
        public string CodeName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Sort
        {
            get;
            set;
        }
    }

    [Serializable]
    public class WfClientApplicationCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientApplication>
    {
        public WfClientApplicationCollection()
        {
        }

        protected WfClientApplicationCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(WfClientApplication item)
        {
            return item.CodeName;
        }
    }
}
