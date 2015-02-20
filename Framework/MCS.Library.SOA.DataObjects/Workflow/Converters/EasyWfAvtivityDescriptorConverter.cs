using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class EasyWfActivityDescriptorConverter : EasyWfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfActivityDescriptor actDesp = (WfActivityDescriptor)base.Deserialize(dictionary, type, serializer);

			//actDesp.ActivityType = DictionaryHelper.GetValue(dictionary, "ActivityType", WfActivityType.NormalActivity);

			if (true == DictionaryHelper.GetValue(dictionary, "AllowEmptyCandidates", false))
			{
				actDesp.Properties.SetValue("AllowEmptyCandidates", true);
			}

			if (true == DictionaryHelper.GetValue(dictionary, "AllowInvalidCandidates", false))
			{
				if (actDesp.Properties.ContainsKey("AllowInvalidCandidates"))
					actDesp.Properties.SetValue("AllowInvalidCandidates", true);
			}

			actDesp.Url = DictionaryHelper.GetValue(dictionary, "Url", string.Empty);
			actDesp.CodeName = DictionaryHelper.GetValue(dictionary, "CodeName", string.Empty);

			WfVariableDescriptorCollection variables = JSONSerializerExecute.Deserialize<WfVariableDescriptorCollection>(dictionary["Variables"]);
			actDesp.Variables.Clear();
			actDesp.Variables.CopyFrom(variables);

			if (dictionary.ContainsKey("Condition"))
			{
				actDesp.Condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(dictionary["Condition"]);
				actDesp.Condition.Owner = actDesp;
			}

			if (dictionary.ContainsKey("BranchProcessTemplates"))
			{
				WfBranchProcessTemplateCollection templates = JSONSerializerExecute.Deserialize<WfBranchProcessTemplateCollection>(dictionary["BranchProcessTemplates"]);
				actDesp.BranchProcessTemplates.Clear();
				actDesp.BranchProcessTemplates.CopyFrom(templates);
			}

			if (dictionary.ContainsKey("Resources"))
			{
				WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["Resources"]);
				actDesp.Resources.Clear();
				actDesp.Resources.CopyFrom(resource);
			}

			if (dictionary.ContainsKey("RelativeLinks"))
			{
				WfRelativeLinkDescriptorCollection relativeLinks = JSONSerializerExecute.Deserialize<WfRelativeLinkDescriptorCollection>(dictionary["RelativeLinks"]);

				actDesp.RelativeLinks.Clear();
				actDesp.RelativeLinks.CopyFrom(relativeLinks);
			}

			if (dictionary.ContainsKey("EnterEventReceivers"))
			{
				WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["EnterEventReceivers"]);
				actDesp.EnterEventReceivers.Clear();
				actDesp.EnterEventReceivers.CopyFrom(resource);
			}

			if (dictionary.ContainsKey("LeaveEventReceivers"))
			{
				WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["LeaveEventReceivers"]);
				actDesp.LeaveEventReceivers.Clear();
				actDesp.LeaveEventReceivers.CopyFrom(resource);
			}

			if (dictionary.ContainsKey("InternalRelativeUsers"))
			{
				WfResourceDescriptorCollection interRelUser = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["InternalRelativeUsers"]);
				actDesp.InternalRelativeUsers.Clear();
				actDesp.InternalRelativeUsers.CopyFrom(interRelUser);
			}

			if (dictionary.ContainsKey("ExternalUsers"))
			{
				WfExternalUserCollection externalUser = JSONSerializerExecute.Deserialize<WfExternalUserCollection>(dictionary["ExternalUsers"]);
				actDesp.ExternalUsers.Clear();
				actDesp.ExternalUsers.CopyFrom(externalUser);
			}

			if (dictionary.ContainsKey("EnterEventExecuteServices"))
			{
				WfServiceOperationDefinitionCollection svcOperationDef =
					JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["EnterEventExecuteServices"]);
				actDesp.EnterEventExecuteServices.Clear();
				actDesp.EnterEventExecuteServices.CopyFrom(svcOperationDef);
			}

			if (dictionary.ContainsKey("ExternalUsers"))
			{
				WfServiceOperationDefinitionCollection svcOperationDef =
					JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["LeaveEventExecuteServices"]);
				actDesp.LeaveEventExecuteServices.Clear();
				actDesp.LeaveEventExecuteServices.CopyFrom(svcOperationDef);
			}

			return actDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfActivityDescriptor actDesp = (WfActivityDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "AllowEmptyCandidates", actDesp.Properties.GetValue("AllowEmptyCandidates", false));
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "AllowInvalidCandidates", actDesp.Properties.GetValue("AllowInvalidCandidates", false));

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ActivityType", actDesp.ActivityType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "CodeName", actDesp.CodeName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Url", actDesp.Url);

			dictionary.Add("Variables", actDesp.Variables);
			dictionary.Add("Condition", actDesp.Condition);
			dictionary.Add("BranchProcessTemplates", actDesp.BranchProcessTemplates);
			dictionary.Add("Resources", actDesp.Resources);
			dictionary.Add("RelativeLinks", actDesp.RelativeLinks);
			dictionary.Add("EnterEventReceivers", actDesp.EnterEventReceivers);
			dictionary.Add("LeaveEventReceivers", actDesp.LeaveEventReceivers);
			dictionary.Add("InternalRelativeUsers", actDesp.InternalRelativeUsers);
			dictionary.Add("ExternalUsers", actDesp.ExternalUsers);
			dictionary.Add("EnterEventExecuteServices", actDesp.EnterEventExecuteServices);
			dictionary.Add("LeaveEventExecuteServices", actDesp.LeaveEventExecuteServices);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfActivityDescriptor), typeof(IWfActivityDescriptor) };
			}
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfActivityType activityType = DictionaryHelper.GetValue(dictionary, "ActivityType", WfActivityType.NormalActivity);

			return new WfActivityDescriptor(key, activityType);
		}
	}
}
