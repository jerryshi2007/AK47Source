using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfAssigneeConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfAssignee assignee = new WfAssignee();

            assignee.AssigneeType = DictionaryHelper.GetValue(dictionary, "AssigneeType", WfAssigneeType.Normal);
            assignee.User = JSONSerializerExecute.Deserialize<OguUser>(dictionary["User"]);
            assignee.Url = DictionaryHelper.GetValue(dictionary, "Url", (string)null);
            assignee.Selected = DictionaryHelper.GetValue(dictionary, "Selected", true);

            if (dictionary.ContainsKey("Delegator"))
                assignee.Delegator = JSONSerializerExecute.Deserialize<OguUser>(dictionary["Delegator"]);

            return assignee;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfAssignee assignee = (WfAssignee)obj;

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            DictionaryHelper.AddNonDefaultValue(dictionary, "AssigneeType", assignee.AssigneeType);
            dictionary.Add("User", assignee.User);
            DictionaryHelper.AddNonDefaultValue(dictionary, "Url", assignee.Url);

            DictionaryHelper.AddNonDefaultValue(dictionary, "Selected", assignee.Selected);

            if (assignee.Delegator != null)
                dictionary.Add("Delegator", assignee.Delegator);

            dictionary["__type"] = obj.GetType().AssemblyQualifiedName;

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(WfAssignee) }; }
        }
    }
}
