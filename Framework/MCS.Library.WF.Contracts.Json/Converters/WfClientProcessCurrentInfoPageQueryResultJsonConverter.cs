using MCS.Library.WF.Contracts.Json.Converters.Query;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientProcessCurrentInfoPageQueryResultJsonConverter :
        ClientPageQueryResultJsonConverterBase<WfClientProcessCurrentInfoPageQueryResult, WfClientProcessCurrentInfo, WfClientProcessCurrentInfoCollection>
    {
        public override WfClientProcessCurrentInfoPageQueryResult CreateInstance(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new WfClientProcessCurrentInfoPageQueryResult();
        }
    }
}
