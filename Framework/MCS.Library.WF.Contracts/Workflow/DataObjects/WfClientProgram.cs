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
    public class WfClientProgram
    {
        public string ApplicationCodeName
        {
            get;
            set;
        }

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
    public class WfClientProgramInApplicationCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientProgram>
    {
        public WfClientProgramInApplicationCollection()
        {
        }

        protected WfClientProgramInApplicationCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(WfClientProgram item)
        {
            return item.CodeName;
        }
    }
}
