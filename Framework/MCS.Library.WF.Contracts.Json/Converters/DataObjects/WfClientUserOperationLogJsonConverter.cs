using MCS.Library.Core;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientUserOperationLogJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientUserOperationLog) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientUserOperationLog log = new WfClientUserOperationLog();

            log.ID = dictionary.GetValue("id", 0);

            log.ResourceID = dictionary.GetValue("resourceID", string.Empty);
            log.ProcessID = dictionary.GetValue("processID", string.Empty);
            log.ActivityID = dictionary.GetValue("activityID", string.Empty);
            log.ActivityName = dictionary.GetValue("activityName", string.Empty);

            log.Subject = dictionary.GetValue("subject", string.Empty);

            log.ApplicationName = dictionary.GetValue("applicationName", string.Empty);
            log.ProgramName = dictionary.GetValue("programName", string.Empty);

            log.OperationName = dictionary.GetValue("operationName", string.Empty);
            log.OperationDescription = dictionary.GetValue("operationDescription", string.Empty);
            log.OperationType = dictionary.GetValue("operationType", WfClientOperationType.Update);
            log.OperationDateTime = dictionary.GetValue("operationDateTime", new Nullable<DateTime>());

            log.CorrelationID = dictionary.GetValue("correlationID", string.Empty);
            log.HttpContextString = dictionary.GetValue("httpContextString", string.Empty);

            log.Operator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("operator", (object)null));
            log.RealUser = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("realUser", (object)null));
            log.TopDepartment = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("topDepartment", (object)null));

            return log;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientUserOperationLog log = (WfClientUserOperationLog)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("id", log.ID);

            dictionary.AddNonDefaultValue("resourceID", log.ResourceID);
            dictionary.AddNonDefaultValue("processID", log.ProcessID);
            dictionary.AddNonDefaultValue("activityID", log.ActivityID);
            dictionary.AddNonDefaultValue("activityName", log.ActivityName);

            dictionary.AddNonDefaultValue("subject", log.Subject);

            dictionary.AddNonDefaultValue("applicationName", log.ApplicationName);
            dictionary.AddNonDefaultValue("programName", log.ProgramName);

            dictionary.AddNonDefaultValue("operationName", log.OperationName);
            dictionary.AddNonDefaultValue("operationDescription", log.OperationDescription);
            dictionary.AddNonDefaultValue("operationType", log.OperationType);
            dictionary.AddNonDefaultValue("operationDateTime", log.OperationDateTime);

            dictionary.AddNonDefaultValue("correlationID", log.CorrelationID);
            dictionary.AddNonDefaultValue("httpContextString", log.HttpContextString);

            dictionary["operator"] = log.Operator;
            dictionary["realUser"] = log.RealUser;
            dictionary["topDepartment"] = log.TopDepartment;

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
