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
    public abstract class WfClientProcessInfoJsonConverterBase : JavaScriptConverter
    {
        private static readonly Dictionary<string, object> _EmptyDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessInfoBase processInfo = this.CreateInstance(dictionary, type, serializer);

            processInfo.ID = dictionary.GetValue("id", string.Empty);
            processInfo.ProcessDescriptorKey = dictionary.GetValue("processDescriptorKey", string.Empty);
            processInfo.CurrentActivityKey = dictionary.GetValue("currentActivityKey", string.Empty);
            processInfo.RuntimeProcessName = dictionary.GetValue("runtimeProcessName", string.Empty);
            processInfo.Committed = dictionary.GetValue("committed", true);
            processInfo.ProcessDescriptorKey = dictionary.GetValue("processDescriptorKey", string.Empty);
            processInfo.Creator = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("creator", (object)null));
            processInfo.OwnerDepartment = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("ownerDepartment", (object)null));
            processInfo.RelativeID = dictionary.GetValue("relativeID", string.Empty);
            processInfo.ResourceID = dictionary.GetValue("resourceID", string.Empty);
            processInfo.RelativeUrl = dictionary.GetValue("relativeUrl", string.Empty);
            processInfo.SearchID = dictionary.GetValue("searchID", string.Empty);
            processInfo.OwnerActivityID = dictionary.GetValue("ownerActivityID", string.Empty);
            processInfo.OwnerTemplateKey = dictionary.GetValue("ownerTemplateKey", string.Empty);
            processInfo.StartTime = dictionary.GetValue("startTime", DateTime.MinValue);
            processInfo.EndTime = dictionary.GetValue("endTime", DateTime.MinValue);
            processInfo.Status = dictionary.GetValue("status", WfClientProcessStatus.NotRunning);
            processInfo.AuthorizationInfo = dictionary.GetValue("authorizationInfo", processInfo.AuthorizationInfo);
            processInfo.CanWithdraw = dictionary.GetValue("canWithdraw", false);
            processInfo.CurrentOpinion = JSONSerializerExecute.Deserialize<WfClientOpinion>(dictionary.GetValue("currentOpinion", (object)null));
            processInfo.UpdateTag = dictionary.GetValue("updateTag", -1);

            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "processContext", processInfo.ProcessContext);
            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "applicationRuntimeParameters", processInfo.ApplicationRuntimeParameters);
            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("mainStreamActivityDescriptors", (object)null), processInfo.MainStreamActivityDescriptors);

            return processInfo;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientProcessInfoBase processInfo = (WfClientProcessInfoBase)obj;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("id", processInfo.ID);
            dictionary.AddNonDefaultValue("processDescriptorKey", processInfo.ProcessDescriptorKey);
            dictionary.AddNonDefaultValue("currentActivityKey", processInfo.CurrentActivityKey);
            dictionary.AddNonDefaultValue("runtimeProcessName", processInfo.RuntimeProcessName);
            dictionary.AddNonDefaultValue("committed", processInfo.Committed);
            dictionary.AddNonDefaultValue("creator", processInfo.Creator);
            dictionary.AddNonDefaultValue("ownerDepartment", processInfo.OwnerDepartment);
            dictionary.AddNonDefaultValue("relativeID", processInfo.RelativeID);
            dictionary.AddNonDefaultValue("resourceID", processInfo.ResourceID);
            dictionary.AddNonDefaultValue("relativeUrl", processInfo.RelativeUrl);
            dictionary.AddNonDefaultValue("searchID", processInfo.SearchID);
            dictionary.AddNonDefaultValue("ownerActivityID", processInfo.OwnerActivityID);
            dictionary.AddNonDefaultValue("ownerTemplateKey", processInfo.OwnerTemplateKey);
            dictionary.AddNonDefaultValue("startTime", processInfo.StartTime);
            dictionary.AddNonDefaultValue("endTime", processInfo.EndTime);
            dictionary.AddNonDefaultValue("status", processInfo.Status);
            dictionary.AddNonDefaultValue("authorizationInfo", processInfo.AuthorizationInfo);
            dictionary.AddNonDefaultValue("processContext", processInfo.ProcessContext);
            dictionary.AddNonDefaultValue("applicationRuntimeParameters", processInfo.ApplicationRuntimeParameters);
            dictionary.AddNonDefaultValue("canWithdraw", processInfo.CanWithdraw);
            dictionary.AddNonDefaultValue("currentOpinion", processInfo.CurrentOpinion);
            dictionary.AddNonDefaultValue("mainStreamActivityDescriptors", processInfo.MainStreamActivityDescriptors);
            dictionary.AddNonDefaultValue("updateTag", processInfo.UpdateTag);

            return dictionary;
        }

        protected abstract WfClientProcessInfoBase CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
    }
}
