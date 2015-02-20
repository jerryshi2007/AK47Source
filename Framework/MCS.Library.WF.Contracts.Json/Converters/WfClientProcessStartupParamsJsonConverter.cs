using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientProcessStartupParamsJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessStartupParams) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessStartupParams startupParams = new WfClientProcessStartupParams();

            startupParams.AutoStartInitialActivity = dictionary.GetValue("autoStartInitialActivity", true);
            startupParams.AutoCommit = dictionary.GetValue("autoCommit", false);
            startupParams.CheckStartProcessUserPermission = dictionary.GetValue("checkStartProcessUserPermission", true);
            startupParams.DefaultTaskTitle = dictionary.GetValue("defaultTaskTitle", string.Empty);
            startupParams.DefaultUrl = dictionary.GetValue("defaultUrl", string.Empty);
            startupParams.ProcessDescriptorKey = dictionary.GetValue("processDescriptorKey", string.Empty);
            startupParams.RelativeID = dictionary.GetValue("relativeID", string.Empty);
            startupParams.RelativeURL = dictionary.GetValue("relativeURL", string.Empty);
            startupParams.ResourceID = dictionary.GetValue("resourceID", string.Empty);
            startupParams.RuntimeProcessName = dictionary.GetValue("runtimeProcessName", string.Empty);
            startupParams.AutoPersist = dictionary.GetValue("autoPersist", true);

            startupParams.Creator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("creator", (object)null));
            startupParams.Department = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("department", (object)null));

            startupParams.Opinion = JSONSerializerExecute.Deserialize<WfClientOpinion>(dictionary.GetValue("opinion", (object)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("assignees", (object)null), startupParams.Assignees);
            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "applicationRuntimeParameters", startupParams.ApplicationRuntimeParameters);
            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "processContext", startupParams.ProcessContext);

            return startupParams;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientProcessStartupParams startupParams = (WfClientProcessStartupParams)obj;

            dictionary.AddNonDefaultValue("autoStartInitialActivity", startupParams.AutoStartInitialActivity);
            dictionary.AddNonDefaultValue("autoCommit", startupParams.AutoCommit);
            dictionary.AddNonDefaultValue("checkStartProcessUserPermission", startupParams.CheckStartProcessUserPermission);
            dictionary.AddNonDefaultValue("defaultTaskTitle", startupParams.DefaultTaskTitle);
            dictionary.AddNonDefaultValue("defaultUrl", startupParams.DefaultUrl);
            dictionary.AddNonDefaultValue("processDescriptorKey", startupParams.ProcessDescriptorKey);
            dictionary.AddNonDefaultValue("relativeID", startupParams.RelativeID);
            dictionary.AddNonDefaultValue("relativeURL", startupParams.RelativeURL);
            dictionary.AddNonDefaultValue("resourceID", startupParams.ResourceID);
            dictionary.AddNonDefaultValue("runtimeProcessName", startupParams.RuntimeProcessName);

            dictionary.AddNonDefaultValue("creator", startupParams.Creator);
            dictionary.AddNonDefaultValue("department", startupParams.Department);
            dictionary.AddNonDefaultValue("autoPersist", startupParams.AutoPersist);

            dictionary["assignees"] = startupParams.Assignees;
            dictionary["applicationRuntimeParameters"] = startupParams.ApplicationRuntimeParameters;
            dictionary["processContext"] = startupParams.ProcessContext;

            dictionary.AddNonDefaultValue("opinion", startupParams.Opinion);

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
