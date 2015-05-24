using MCS.Library.Core;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientProcessQueryConditionJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessQueryCondition) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessQueryCondition condition = new WfClientProcessQueryCondition();

            condition.ApplicationName = DictionaryHelper.GetValue(dictionary, "applicationName", string.Empty);
            condition.ProgramName = DictionaryHelper.GetValue(dictionary, "programName", string.Empty);

            condition.BeginStartTime = DictionaryHelper.GetValue(dictionary, "beginStartTime", DateTime.MinValue);
            condition.EndStartTime = DictionaryHelper.GetValue(dictionary, "endStartTime", DateTime.MinValue);

            condition.DepartmentName = DictionaryHelper.GetValue(dictionary, "departmentName", string.Empty);
            condition.ProcessName = DictionaryHelper.GetValue(dictionary, "processName", string.Empty);

            condition.AssigneesSelectType = DictionaryHelper.GetValue(dictionary, "assigneesSelectType", WfClientAssigneesFilterType.CurrentActivity);
            condition.AssigneesUserName = DictionaryHelper.GetValue(dictionary, "assigneesUserName", string.Empty);

            condition.AssigneeExceptionFilterType = DictionaryHelper.GetValue(dictionary, "assigneeExceptionFilterType", WfClientAssigneeExceptionFilterType.All);

            condition.ProcessStatus = DictionaryHelper.GetValue(dictionary, "processStatus", string.Empty);

            condition.ProcessCreatorID = DictionaryHelper.GetValue(dictionary, "processCreatorID", string.Empty);
            condition.ProcessCreatorName = DictionaryHelper.GetValue(dictionary, "processCreatorName", string.Empty);

            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("currentAssignees", (ArrayList)null), condition.CurrentAssignees);

            return condition;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientProcessQueryCondition condition = (WfClientProcessQueryCondition)obj;

            DictionaryHelper.AddNonDefaultValue(dictionary, "applicationName", condition.ApplicationName);
            DictionaryHelper.AddNonDefaultValue(dictionary, "programName", condition.ProgramName);

            DictionaryHelper.AddNonDefaultValue(dictionary, "beginStartTime", condition.BeginStartTime);
            DictionaryHelper.AddNonDefaultValue(dictionary, "endStartTime", condition.EndStartTime);

            DictionaryHelper.AddNonDefaultValue(dictionary, "departmentName", condition.DepartmentName);
            DictionaryHelper.AddNonDefaultValue(dictionary, "processName", condition.ProcessName);

            DictionaryHelper.AddNonDefaultValue(dictionary, "assigneesSelectType", condition.AssigneesSelectType);
            DictionaryHelper.AddNonDefaultValue(dictionary, "assigneesUserName", condition.AssigneesUserName);

            DictionaryHelper.AddNonDefaultValue(dictionary, "assigneeExceptionFilterType", condition.AssigneeExceptionFilterType);

            DictionaryHelper.AddNonDefaultValue(dictionary, "processStatus", condition.ProcessStatus);

            DictionaryHelper.AddNonDefaultValue(dictionary, "processCreatorID", condition.ProcessCreatorID);
            DictionaryHelper.AddNonDefaultValue(dictionary, "processCreatorName", condition.ProcessCreatorName);

            DictionaryHelper.AddNonDefaultValue(dictionary, "currentAssignees", condition.CurrentAssignees);

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
