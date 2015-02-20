using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfProcessDescriptorConverter : WfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			//WfProcessDescriptor processDesp = (WfProcessDescriptor)base.Deserialize(dictionary, type, serializer);
			#region "base Deserialize"
			string key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
			WfProcessDescriptor processDesp = (WfProcessDescriptor)CreateInstance(key, dictionary, type, serializer);
			processDesp.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty); ;
			processDesp.Enabled = DictionaryHelper.GetValue(dictionary, "Enabled", false);
			processDesp.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

			Dictionary<string, object> processProperties = new Dictionary<string, object>();
			Dictionary<string, Type> constKey = new Dictionary<string, Type>();
			constKey.Add("RelativeLinks", typeof(WfRelativeLinkDescriptorCollection));
			constKey.Add("CancelEventReceivers", typeof(WfResourceDescriptorCollection));
			constKey.Add("InternalRelativeUsers", typeof(WfResourceDescriptorCollection));
			constKey.Add("ExternalUsers", typeof(WfExternalUserCollection));
			constKey.Add("Variables", typeof(WfVariableDescriptorCollection));
			constKey.Add("ParametersNeedToBeCollected", typeof(WfParameterNeedToBeCollected));

			constKey.Add("CancelBeforeExecuteServices", typeof(WfServiceOperationDefinitionCollection));
			constKey.Add("CancelAfterExecuteServices", typeof(WfServiceOperationDefinitionCollection));

			if (dictionary.ContainsKey("Properties"))
			{
				PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);
				properties.Remove(p => string.Compare(p.Definition.Name, "ImportWfMatrix") == 0);
				processDesp.Properties.Clear();
				foreach (PropertyValue pv in properties)
				{
					if (constKey.ContainsKey(pv.Definition.Name))
					{
						var objValue = JSONSerializerExecute.DeserializeObject(pv.StringValue, constKey[pv.Definition.Name]);
						processProperties.Add(pv.Definition.Name, objValue);
					}
					else
					{
						processDesp.Properties.Add(pv);
					}
				}
			}
			#endregion

			processDesp.GraphDescription = DictionaryHelper.GetValue(dictionary, "GraphDescription", string.Empty);

			WfActivityDescriptorCollection activities = JSONSerializerExecute.Deserialize<WfActivityDescriptorCollection>(dictionary["Activities"]);
			processDesp.Activities.Clear();
			processDesp.Activities.CopyFrom(activities);

			ClearAllProperties(processDesp);
			SetProcessProperties(processDesp, processProperties, dictionary);

			ToTransitionsDescriptorCollection transitions = JSONSerializerExecute.Deserialize<ToTransitionsDescriptorCollection>(dictionary["Transitions"]);

			foreach (WfTransitionDescriptor tranDesp in transitions)
			{
				WfActivityDescriptor fromActDesc = (WfActivityDescriptor)processDesp.Activities[tranDesp.FromActivityKey];
				WfActivityDescriptor toActDesc = (WfActivityDescriptor)processDesp.Activities[tranDesp.ToActivityKey];

				if (fromActDesc != null && toActDesc != null)
					fromActDesc.ToTransitions.AddTransition(toActDesc, tranDesp);
			}

			return processDesp;
		}

		private void SetProcessProperties(WfProcessDescriptor processDesp, Dictionary<string, object> processProperties, IDictionary<string, object> dictionary)
		{
			if (processProperties.ContainsKey("RelativeLinks"))
			{
				processDesp.RelativeLinks.CopyFrom((WfRelativeLinkDescriptorCollection)processProperties["RelativeLinks"]);
			}
			else if (dictionary.ContainsKey("RelativeLinks"))
			{
				WfRelativeLinkDescriptorCollection relativeLinks = JSONSerializerExecute.Deserialize<WfRelativeLinkDescriptorCollection>(dictionary["Variables"]);
				processDesp.RelativeLinks.CopyFrom(relativeLinks);
			}

			if (processProperties.ContainsKey("CancelEventReceivers"))
			{
				processDesp.CancelEventReceivers.CopyFrom((WfResourceDescriptorCollection)processProperties["CancelEventReceivers"]);
			}
			else if (dictionary.ContainsKey("CancelEventReceivers"))
			{
				WfResourceDescriptorCollection relativeLinks = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["CancelEventReceivers"]);
				processDesp.CancelEventReceivers.CopyFrom(relativeLinks);
			}

			if (processProperties.ContainsKey("CancelBeforeExecuteServices"))
			{
				processDesp.CancelBeforeExecuteServices.CopyFrom((WfServiceOperationDefinitionCollection)processProperties["CancelBeforeExecuteServices"]);
			}
			else if (dictionary.ContainsKey("CancelBeforeExecuteServices"))
			{
				WfServiceOperationDefinitionCollection canceBeforeExecuteServices = JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["CancelBeforeExecuteServices"]);
				processDesp.CancelBeforeExecuteServices.CopyFrom(canceBeforeExecuteServices);
			}

			if (processProperties.ContainsKey("CancelAfterExecuteServices"))
			{
				processDesp.CancelAfterExecuteServices.CopyFrom((WfServiceOperationDefinitionCollection)processProperties["CancelAfterExecuteServices"]);
			}
			else if (dictionary.ContainsKey("CancelAfterExecuteServices"))
			{
				WfServiceOperationDefinitionCollection canceAfterExecuteServices = JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["CancelAfterExecuteServices"]);
				processDesp.CancelAfterExecuteServices.CopyFrom(canceAfterExecuteServices);
			}

			if (processProperties.ContainsKey("InternalRelativeUsers"))
			{
				processDesp.InternalRelativeUsers.CopyFrom((WfResourceDescriptorCollection)processProperties["InternalRelativeUsers"]);
			}
			else if (dictionary.ContainsKey("InternalRelativeUsers"))
			{
				WfResourceDescriptorCollection interRelUser = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["InternalRelativeUsers"]);
				processDesp.InternalRelativeUsers.CopyFrom(interRelUser);
			}

			if (processProperties.ContainsKey("ExternalUsers"))
			{
				processDesp.ExternalUsers.CopyFrom((WfExternalUserCollection)processProperties["ExternalUsers"]);
			}
			else if (dictionary.ContainsKey("ExternalUsers"))
			{
				WfExternalUserCollection externalUser = JSONSerializerExecute.Deserialize<WfExternalUserCollection>(dictionary["ExternalUsers"]);
				processDesp.ExternalUsers.CopyFrom(externalUser);
			}

			if (processProperties.ContainsKey("Variables"))
			{
				processDesp.Variables.CopyFrom((WfVariableDescriptorCollection)processProperties["Variables"]);
			}
			else if (dictionary.ContainsKey("Variables"))
			{
				WfVariableDescriptorCollection externalUser = JSONSerializerExecute.Deserialize<WfVariableDescriptorCollection>(dictionary["Variables"]);
				processDesp.Variables.CopyFrom(externalUser);
			}

			if (processProperties.ContainsKey("ParametersNeedToBeCollected"))
			{
				processDesp.ParametersNeedToBeCollected.CopyFrom((WfParameterNeedToBeCollected)processProperties["ParametersNeedToBeCollected"]);
			}
			else if (dictionary.ContainsKey("ParametersNeedToBeCollected"))
			{
				WfParameterNeedToBeCollected parametersNeedToBeCollected = JSONSerializerExecute.Deserialize<WfParameterNeedToBeCollected>(dictionary["ParametersNeedToBeCollected"]);
				processDesp.ParametersNeedToBeCollected.CopyFrom(parametersNeedToBeCollected);
			}
		}

		private void ClearAllProperties(WfProcessDescriptor processDesp)
		{
			processDesp.Variables.Clear();
			processDesp.Variables.Clear();
			processDesp.RelativeLinks.Clear();
			processDesp.CancelEventReceivers.Clear();
			processDesp.InternalRelativeUsers.Clear();
			processDesp.ExternalUsers.Clear();
			processDesp.ParametersNeedToBeCollected.Clear();
			processDesp.CancelAfterExecuteServices.Clear();
			processDesp.CancelBeforeExecuteServices.Clear();
		}

		public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfProcessDescriptor processDesp = (WfProcessDescriptor)obj;

			Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Version", processDesp.Version);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ApplicationName", processDesp.ApplicationName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ProgramName", processDesp.ProgramName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Url", processDesp.Url);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "GraphDescription", processDesp.GraphDescription);

			dictionary.Add("Activities", processDesp.Activities);

			SetPropertiesValue(processDesp, "Variables", processDesp.Variables);
			SetPropertiesValue(processDesp, "RelativeLinks", processDesp.RelativeLinks);
			SetPropertiesValue(processDesp, "CancelEventReceivers", processDesp.CancelEventReceivers);
			SetPropertiesValue(processDesp, "InternalRelativeUsers", processDesp.InternalRelativeUsers);
			SetPropertiesValue(processDesp, "ExternalUsers", processDesp.ExternalUsers);
			SetPropertiesValue(processDesp, "ParametersNeedToBeCollected", processDesp.ParametersNeedToBeCollected);
			SetPropertiesValue(processDesp, "CancelBeforeExecuteServices", processDesp.CancelBeforeExecuteServices);
			SetPropertiesValue(processDesp, "CancelAfterExecuteServices", processDesp.CancelAfterExecuteServices);

			ToTransitionsDescriptorCollection transitions = new ToTransitionsDescriptorCollection();

			foreach (WfActivityDescriptor actDesp in processDesp.Activities)
			{
				transitions.CopyFrom(actDesp.ToTransitions);
			}

			dictionary.Add("Transitions", transitions);

			return dictionary;
		}

		private void SetPropertiesValue(WfProcessDescriptor processDesp, string propertyName, object input)
		{
			if (processDesp.Properties.ContainsKey(propertyName))
			{
				string itemvalue = JSONSerializerExecute.Serialize(input);
				processDesp.Properties.SetValue<string>(propertyName, itemvalue);
			}
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfProcessDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfProcessDescriptor) };
			}
		}
	}
}
