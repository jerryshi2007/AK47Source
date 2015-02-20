using MCS.Library.WF.Contracts.Json.Converters.Query;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientProcessDescriptorInfoPageQueryResultJsonConverter :
      ClientPageQueryResultJsonConverterBase<WfClientProcessDescriptorInfoPageQueryResult, WfClientProcessDescriptorInfo, WfClientProcessDescriptorInfoCollection>
    {
        public override WfClientProcessDescriptorInfoPageQueryResult CreateInstance(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new WfClientProcessDescriptorInfoPageQueryResult();
        }
    }
}
