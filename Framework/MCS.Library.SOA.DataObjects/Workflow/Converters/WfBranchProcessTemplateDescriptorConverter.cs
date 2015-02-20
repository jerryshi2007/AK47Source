using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfBranchProcessTemplateDescriptorConverter : WfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			string key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);

			WfBranchProcessTemplateDescriptor branchProcTempDesp = (WfBranchProcessTemplateDescriptor)CreateInstance(key, dictionary, type, serializer);

			branchProcTempDesp.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty); ;
			branchProcTempDesp.Enabled = DictionaryHelper.GetValue(dictionary, "Enabled", false);
			branchProcTempDesp.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

			if (dictionary.ContainsKey("Properties"))
			{
				branchProcTempDesp.Properties.Clear();

				PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);

				foreach (PropertyValue pv in properties)
				{
					switch (pv.Definition.Name)
					{
						case "Resources":
							WfResourceDescriptorCollection resources = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(pv.StringValue);
							branchProcTempDesp.Resources.Clear();
							if (resources != null)
								branchProcTempDesp.Resources.CopyFrom(resources);
							break;
						case "CancelSubProcessNotifier":
							WfResourceDescriptorCollection cancelSubProcessNotifierResources = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(pv.StringValue);
							branchProcTempDesp.CancelSubProcessNotifier.Clear();
							if (cancelSubProcessNotifierResources != null)
								branchProcTempDesp.CancelSubProcessNotifier.CopyFrom(cancelSubProcessNotifierResources);
							break;
						case "Condition":
							branchProcTempDesp.Condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(pv.StringValue);
							branchProcTempDesp.Condition.Owner = branchProcTempDesp;
							break;
						case "OperationDefinition":
							branchProcTempDesp.OperationDefinition = JSONSerializerExecute.Deserialize<WfServiceOperationDefinition>(pv.StringValue);
							break;
						case "BranchProcessKey":
							branchProcTempDesp.Properties.Add(pv);
							break;
						case "RelativeLinks":
							WfRelativeLinkDescriptorCollection links = JSONSerializerExecute.Deserialize<WfRelativeLinkDescriptorCollection>(pv.StringValue);
							branchProcTempDesp.RelativeLinks.Clear();

							if (links != null)
								branchProcTempDesp.RelativeLinks.CopyFrom(links);
							break;
						default:
							branchProcTempDesp.Properties.Add(pv);
							break;
					}
				}
			}

			return branchProcTempDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfBranchProcessTemplateDescriptor branchProcTempDesp = (WfBranchProcessTemplateDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "BranchProcessKey", branchProcTempDesp.BranchProcessKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ExecuteSequence", branchProcTempDesp.ExecuteSequence);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "BlockingType", branchProcTempDesp.BlockingType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OperationDefinition", branchProcTempDesp.OperationDefinition);

			this.SetPropertiesValue(branchProcTempDesp, "Condition", branchProcTempDesp.Condition);
			this.SetPropertiesValue(branchProcTempDesp, "Resources", branchProcTempDesp.Resources);
			this.SetPropertiesValue(branchProcTempDesp, "OperationDefinition", branchProcTempDesp.OperationDefinition);
			this.SetPropertiesValue(branchProcTempDesp, "CancelSubProcessNotifier", branchProcTempDesp.CancelSubProcessNotifier);
			this.SetPropertiesValue(branchProcTempDesp, "RelativeLinks", branchProcTempDesp.RelativeLinks);

			return dictionary;
		}

		private void SetPropertiesValue(WfBranchProcessTemplateDescriptor branchTemplate, string propertyName, object input)
		{
			if (branchTemplate.Properties.ContainsKey(propertyName))
			{
				string itemValue = JSONSerializerExecute.Serialize(input);
				branchTemplate.Properties.SetValue<string>(propertyName, itemValue);
			}
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfBranchProcessTemplateDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfBranchProcessTemplateDescriptor), typeof(IWfBranchProcessTemplateDescriptor) }; }
		}
	}
}
