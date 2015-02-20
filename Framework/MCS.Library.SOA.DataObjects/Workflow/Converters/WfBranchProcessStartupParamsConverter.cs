using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfBranchProcessStartupParamsConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfBranchProcessStartupParams data = new WfBranchProcessStartupParams();

			data.ResourceID = dictionary.GetValue("ResourceID", string.Empty);
			data.DefaultTaskTitle = dictionary.GetValue("DefaultTaskTitle", string.Empty);
			data.Assignees = JSONSerializerExecute.Deserialize<WfAssigneeCollection>(dictionary["Assignees"]);
			data.Department = JSONSerializerExecute.Deserialize<OguOrganization>(dictionary["Department"]);
			data.ApplicationRuntimeParameters = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(dictionary["ApplicationRuntimeParameters"]);

			foreach (KeyValuePair<string, object> kp in (Dictionary<string, object>)dictionary["RelativeParams"])
				data.RelativeParams[kp.Key] = (string)kp.Value;

			data.StartupContext = dictionary["StartupContext"];

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfBranchProcessStartupParams data = (WfBranchProcessStartupParams)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.AddNonDefaultValue("ResourceID", data.ResourceID);
			dictionary.AddNonDefaultValue("DefaultTaskTitle", data.DefaultTaskTitle);
			dictionary["Assignees"] = data.Assignees;
			dictionary["Department"] = data.Department;
			dictionary["ApplicationRuntimeParameters"] = data.ApplicationRuntimeParameters;

			Dictionary<string, string> relativeParams = new Dictionary<string, string>();

			foreach (string key in data.RelativeParams)
				relativeParams[key] = data.RelativeParams[key];

			dictionary["RelativeParams"] = relativeParams;
			dictionary["StartupContext"] = data.StartupContext;

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfBranchProcessStartupParams) }; }
		}
	}
}
