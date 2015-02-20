using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	public class WfMoveToSelectedResultConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfMoveToSelectedResult result = new WfMoveToSelectedResult();

			result.OperationType = DictionaryHelper.GetValue(dictionary, "OperationType", WfControlOperationType.MoveTo);
			result.BlockingType = DictionaryHelper.GetValue(dictionary, "BlockingType", WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			result.SequenceType = DictionaryHelper.GetValue(dictionary, "SequenceType", WfBranchProcessExecuteSequence.Parallel);
            result.RejectMode = DictionaryHelper.GetValue(dictionary, "RejectMode", WfRejectMode.SelectRejectStep);

			result.TargetActivityDescriptor = JSONSerializerExecute.Deserialize<IWfActivityDescriptor>(dictionary["TargetActivityDescriptor"]);
			result.FromTransitionDescriptor = JSONSerializerExecute.Deserialize<IWfTransitionDescriptor>(dictionary["FromTransitionDescriptor"]);
			result.BranchTemplate = JSONSerializerExecute.Deserialize<IWfBranchProcessTemplateDescriptor>(dictionary["BranchTemplate"]);

			WfAssigneeCollection assignees = JSONSerializerExecute.Deserialize<WfAssigneeCollection>(dictionary["Assignees"]);
			result.Assignees.CopyFrom(assignees);

			WfAssigneeCollection circulators = JSONSerializerExecute.Deserialize<WfAssigneeCollection>(dictionary["Circulators"]);
			result.Circulators.CopyFrom(circulators);

			return result;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfMoveToSelectedResult result = (WfMoveToSelectedResult)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.Add("OperationType", result.OperationType);
			dictionary.Add("TargetActivityDescriptor", result.TargetActivityDescriptor);
			dictionary.Add("FromTransitionDescriptor", result.FromTransitionDescriptor);
			dictionary.Add("BranchTemplate", result.BranchTemplate);
			dictionary.Add("BlockingType", result.BlockingType);
			dictionary.Add("SequenceType", result.SequenceType);
			dictionary.Add("Assignees", result.Assignees);
			dictionary.Add("Circulators", result.Circulators);
            dictionary.Add("RejectMode", result.RejectMode);
			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfMoveToSelectedResult) }; }
		}
	}
}
