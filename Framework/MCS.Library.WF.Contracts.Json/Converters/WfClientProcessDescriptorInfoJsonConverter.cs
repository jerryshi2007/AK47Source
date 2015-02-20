using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.WF.Contracts.Json.Converters.Query;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientProcessDescriptorInfoJsonConverter : JavaScriptConverter
    {
       private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessDescriptorInfo) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessDescriptorInfo processInfo = new WfClientProcessDescriptorInfo();

            processInfo.ProcessKey = dictionary.GetValue("processKey", string.Empty);
            processInfo.ProcessName = dictionary.GetValue("processName", string.Empty);

            processInfo.ApplicationName = dictionary.GetValue("applicationName", string.Empty);
            processInfo.ProgramName = dictionary.GetValue("programName", string.Empty);


            processInfo.Enabled = dictionary.GetValue("enabled", true);
            processInfo.Data = dictionary.GetValue("data", string.Empty);          
           
            processInfo.CreateTime = dictionary.GetValue("createTime", DateTime.MinValue);
            processInfo.ModifyTime = dictionary.GetValue("modifyTime", DateTime.MinValue);
            processInfo.ImportTime = dictionary.GetValue("importTime", DateTime.MinValue);   

            processInfo.Creator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("creator", (object)null));
            processInfo.Modifier = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("modifier", (object)null));
            processInfo.ImportUser = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("importUser", (object)null));
           
            return processInfo;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientProcessDescriptorInfo processInfo = (WfClientProcessDescriptorInfo)obj;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("processKey", processInfo.ProcessKey);
            dictionary.AddNonDefaultValue("processName", processInfo.ProcessName);
            dictionary.AddNonDefaultValue("applicationName", processInfo.ApplicationName);
            dictionary.AddNonDefaultValue("programName", processInfo.ProgramName);

  
            dictionary.AddNonDefaultValue("data", processInfo.Data);
            dictionary.AddNonDefaultValue("enabled", processInfo.Enabled);

            dictionary.AddNonDefaultValue("createTime", processInfo.CreateTime);
            dictionary.AddNonDefaultValue("modifyTime", processInfo.ModifyTime);
            dictionary.AddNonDefaultValue("importTime", processInfo.ImportTime);

            dictionary.AddNonDefaultValue("creator", processInfo.Creator);
            dictionary.AddNonDefaultValue("modifier", processInfo.Modifier);
            dictionary.AddNonDefaultValue("importUser", processInfo.ImportUser); 

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
