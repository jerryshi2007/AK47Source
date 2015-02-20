using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [Serializable]
    [DataContract]
    public abstract class WfClientResourceDescriptor
    {
    }

    [Serializable]
    [DataContract]
    public class WfClientResourceDescriptorCollection : EditableDataObjectCollectionBase<WfClientResourceDescriptor>
    {
    }
}
