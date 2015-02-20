using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [CollectionDataContract(IsReference = true)]
    
    public class WfClientActivityCollection : EditableKeyedDataObjectCollectionBase<string, WfClientActivity>
    {
       
        protected override string GetKeyForItem(WfClientActivity item)
        {
            return item.ID;
        }
    }

    [CollectionDataContract(IsReference = true)]
    [Serializable]
    public class WfClientActionCollection : EditableDataObjectCollectionBase<ClientUserTaskActionBase>
    {

    }
}
