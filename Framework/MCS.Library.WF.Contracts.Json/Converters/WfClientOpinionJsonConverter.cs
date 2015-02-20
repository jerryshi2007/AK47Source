using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
	public class WfClientOpinionJsonConverter : JavaScriptConverter
	{
		private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientOpinion) };

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfClientOpinion opinion = new WfClientOpinion();

			opinion.ID = dictionary.GetValue("id", string.Empty);
			opinion.ResourceID = dictionary.GetValue("resourceID", string.Empty);
			opinion.ActivityID = dictionary.GetValue("activityID", string.Empty);
			opinion.ProcessID = dictionary.GetValue("processID", string.Empty);
			opinion.Content = dictionary.GetValue("content", string.Empty);
			opinion.OpinionType = dictionary.GetValue("opinionType", string.Empty);
			opinion.LevelName = dictionary.GetValue("levelName", string.Empty);
			opinion.LevelDesp = dictionary.GetValue("levelDesp", string.Empty);
			opinion.IssueTime = dictionary.GetValue("issueTime", DateTime.MinValue);
			opinion.AppendTime = dictionary.GetValue("appendTime", DateTime.MinValue);
			opinion.IssuePersonID = dictionary.GetValue("issuePersonID", string.Empty);
			opinion.IssuePersonName = dictionary.GetValue("issuePersonName", string.Empty);
			opinion.AppendPersonID = dictionary.GetValue("appendPersonID", string.Empty);
			opinion.AppendPersonName = dictionary.GetValue("appendPersonName", string.Empty);
			opinion.ExtraData = dictionary.GetValue("extraData", string.Empty);

			return opinion;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfClientOpinion opinion = (WfClientOpinion)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.AddNonDefaultValue("id", opinion.ID);
			dictionary.AddNonDefaultValue("resourceID", opinion.ResourceID);
			dictionary.AddNonDefaultValue("activityID", opinion.ActivityID);
			dictionary.AddNonDefaultValue("processID", opinion.ProcessID);
			dictionary.AddNonDefaultValue("content", opinion.Content);
			dictionary.AddNonDefaultValue("opinionType", opinion.OpinionType);
			dictionary.AddNonDefaultValue("levelName", opinion.LevelName);
			dictionary.AddNonDefaultValue("levelDesp", opinion.LevelDesp);
			dictionary.AddNonDefaultValue("issueTime", opinion.IssueTime);
			dictionary.AddNonDefaultValue("appendTime", opinion.AppendTime);
			dictionary.AddNonDefaultValue("issuePersonID", opinion.IssuePersonID);
			dictionary.AddNonDefaultValue("issuePersonName", opinion.IssuePersonName);
			dictionary.AddNonDefaultValue("appendPersonID", opinion.AppendPersonID);
			dictionary.AddNonDefaultValue("appendPersonName", opinion.AppendPersonName);
			dictionary.AddNonDefaultValue("extraData", opinion.ExtraData);

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
