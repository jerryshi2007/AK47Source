using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientProcessCurrentInfoJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessCurrentInfo) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessCurrentInfo processInfo = new WfClientProcessCurrentInfo();

            processInfo.InstanceID = dictionary.GetValue("instanceID", string.Empty);
            processInfo.ResourceID = dictionary.GetValue("resourceID", string.Empty);

            processInfo.ApplicationName = dictionary.GetValue("applicationName", string.Empty);
            processInfo.ProgramName = dictionary.GetValue("programName", string.Empty);
            processInfo.ProcessName = dictionary.GetValue("processName", string.Empty);
            processInfo.DescriptorKey = dictionary.GetValue("descriptorKey", string.Empty);
            processInfo.OwnerActivityID = dictionary.GetValue("ownerActivityID", string.Empty);
            processInfo.OwnerTemplateKey = dictionary.GetValue("ownerTemplateKey", string.Empty);
            processInfo.CurrentActivityID = dictionary.GetValue("currentActivityID", string.Empty);

            processInfo.Sequence = dictionary.GetValue("sequence", 0);
            processInfo.Committed = dictionary.GetValue("committed", true);
            processInfo.CreateTime = dictionary.GetValue("createTime", DateTime.MinValue);
            processInfo.StartTime = dictionary.GetValue("startTime", DateTime.MinValue);
            processInfo.EndTime = dictionary.GetValue("endTime", DateTime.MinValue);

            processInfo.Creator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("creator", (object)null));
            processInfo.Department = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("department", (object)null));
            processInfo.Status = dictionary.GetValue("status", WfClientProcessStatus.NotRunning);
            processInfo.UpdateTag = dictionary.GetValue("updateTag", 0);

            return processInfo;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientProcessCurrentInfo processInfo = (WfClientProcessCurrentInfo)obj;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("instanceID", processInfo.InstanceID);
            dictionary.AddNonDefaultValue("resourceID", processInfo.ResourceID);

            dictionary.AddNonDefaultValue("applicationName", processInfo.ApplicationName);
            dictionary.AddNonDefaultValue("programName", processInfo.ProgramName);
            dictionary.AddNonDefaultValue("processName", processInfo.ProcessName);
            dictionary.AddNonDefaultValue("descriptorKey", processInfo.DescriptorKey);
            dictionary.AddNonDefaultValue("ownerActivityID", processInfo.OwnerActivityID);
            dictionary.AddNonDefaultValue("ownerTemplateKey", processInfo.OwnerTemplateKey);
            dictionary.AddNonDefaultValue("currentActivityID", processInfo.CurrentActivityID);

            dictionary.AddNonDefaultValue("sequence", processInfo.Sequence);
            dictionary.AddNonDefaultValue("committed", processInfo.Committed);
            dictionary.AddNonDefaultValue("createTime", processInfo.CreateTime);
            dictionary.AddNonDefaultValue("startTime", processInfo.StartTime);
            dictionary.AddNonDefaultValue("endTime", processInfo.EndTime);

            dictionary.AddNonDefaultValue("creator", processInfo.Creator);
            dictionary.AddNonDefaultValue("department", processInfo.Department);
            dictionary.AddNonDefaultValue("status", processInfo.Status);
            dictionary.AddNonDefaultValue("updateTag", processInfo.UpdateTag);

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
