using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientDelegationJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientDelegation) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientDelegation delegation = new WfClientDelegation();

            delegation.SourceUserID = dictionary.GetValue("sourceUserID", string.Empty);
            delegation.SourceUserName = dictionary.GetValue("sourceUserName", string.Empty);
            delegation.DestinationUserID = dictionary.GetValue("destinationUserID", string.Empty);
            delegation.DestinationUserName = dictionary.GetValue("destinationUserName", string.Empty);

            delegation.StartTime = dictionary.GetValue("startTime", DateTime.MinValue);
            delegation.StartTime = delegation.StartTime.ToLocalTime();

            delegation.EndTime = dictionary.GetValue("endTime", DateTime.MinValue);
            delegation.EndTime = delegation.EndTime.ToLocalTime();

            delegation.Enabled = dictionary.GetValue("enabled", false);

            delegation.ApplicationName = dictionary.GetValue("applicationName", string.Empty);
            delegation.ProgramName = dictionary.GetValue("programName", string.Empty);
            delegation.TenantCode = dictionary.GetValue("tenantCode", string.Empty);

            return delegation;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientDelegation opinion = (WfClientDelegation)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("sourceUserID", opinion.SourceUserID);
            dictionary.AddNonDefaultValue("sourceUserName", opinion.SourceUserName);
            dictionary.AddNonDefaultValue("destinationUserID", opinion.DestinationUserID);
            dictionary.AddNonDefaultValue("destinationUserName", opinion.DestinationUserName);
            dictionary.AddNonDefaultValue("startTime", opinion.StartTime);
            dictionary.AddNonDefaultValue("endTime", opinion.EndTime);
            dictionary.AddNonDefaultValue("enabled", opinion.Enabled);

            dictionary.AddNonDefaultValue("applicationName", opinion.ApplicationName);
            dictionary.AddNonDefaultValue("programName", opinion.ProgramName);
            dictionary.AddNonDefaultValue("tenantCode", opinion.TenantCode);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }
}
