using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Json.Converters.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientUserOperationLogPageQueryResultJsonConverter :
        ClientPageQueryResultJsonConverterBase<WfClientUserOperationLogPageQueryResult, WfClientUserOperationLog, WfClientUserOperationLogCollection>
    {
        public override WfClientUserOperationLogPageQueryResult CreateInstance(IDictionary<string, object> dictionary, System.Web.Script.Serialization.JavaScriptSerializer serializer)
        {
            return new WfClientUserOperationLogPageQueryResult();
        }
    }
}
