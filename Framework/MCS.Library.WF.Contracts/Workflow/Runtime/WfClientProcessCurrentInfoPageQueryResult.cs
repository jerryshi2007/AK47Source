using MCS.Library.WF.Contracts.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [Serializable]
    [DataContract]
    public class WfClientProcessCurrentInfoPageQueryResult : ClientPageQueryResultBase<WfClientProcessCurrentInfo, WfClientProcessCurrentInfoCollection>
    {
    }
}
