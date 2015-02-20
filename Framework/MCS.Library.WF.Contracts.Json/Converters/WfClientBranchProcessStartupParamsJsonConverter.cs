using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientBranchProcessStartupParamsJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientBranchProcessStartupParams) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientBranchProcessStartupParams startupParams = new WfClientBranchProcessStartupParams();

            startupParams.DefaultTaskTitle = dictionary.GetValue("defaultTaskTitle", string.Empty);
            startupParams.ResourceID = dictionary.GetValue("resourceID", string.Empty);
            startupParams.Department = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("department", (object)null));

            JSONSerializerExecute.FillDeserializedCollection(dictionary["assignees"], startupParams.Assignees);

            Dictionary<string, object> appParams = (Dictionary<string, object>)dictionary["applicationRuntimeParameters"];

            appParams.ForEach(kp => startupParams.ApplicationRuntimeParameters.Add(kp.Key, kp.Value));

            FillDictionaryToNameValueCollection((IDictionary<string, object>)dictionary["relativeParams"], startupParams.RelativeParams);

            startupParams.StartupContext = dictionary.GetValue("startupContext", string.Empty);

            return startupParams;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientBranchProcessStartupParams startupParams = (WfClientBranchProcessStartupParams)obj;

            dictionary.AddNonDefaultValue("defaultTaskTitle", startupParams.DefaultTaskTitle);
            dictionary.AddNonDefaultValue("resourceID", startupParams.ResourceID);

            dictionary.AddNonDefaultValue("department", startupParams.Department);

            dictionary["assignees"] = startupParams.Assignees;
            dictionary["applicationRuntimeParameters"] = startupParams.ApplicationRuntimeParameters;
            dictionary["relativeParams"] = NameValueCollectionToDictionary(startupParams.RelativeParams);
            dictionary["startupContext"] = startupParams.StartupContext;

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }

        private static IDictionary<string, object> NameValueCollectionToDictionary(NameValueCollection nvc)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (string key in nvc)
                dictionary[key] = nvc[key];

            return dictionary;
        }

        private static void FillDictionaryToNameValueCollection(IDictionary<string, object> dictionary, NameValueCollection nvc)
        {
            foreach(KeyValuePair<string, object> kp in dictionary)
                nvc[kp.Key] = kp.Value.ToString();
        }
    }
}
