using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using System.ComponentModel;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfActivityDescriptorConverter : WfDescriptorConverterBase
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            #region "base Deserialize"
            string key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
            WfActivityDescriptor actDesp = (WfActivityDescriptor)CreateInstance(key, dictionary, type, serializer);
            actDesp.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty); ;
            actDesp.Enabled = DictionaryHelper.GetValue(dictionary, "Enabled", false);
            actDesp.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

            Dictionary<string, object> activityProperties = new Dictionary<string, object>();

            Dictionary<string, Type> constKey = new Dictionary<string, Type>();
            constKey.Add("Variables", typeof(WfVariableDescriptorCollection));
            constKey.Add("Condition", typeof(WfConditionDescriptor));
            constKey.Add("BranchProcessTemplates", typeof(WfBranchProcessTemplateCollection));
            constKey.Add("Resources", typeof(WfResourceDescriptorCollection));
            constKey.Add("RelativeLinks", typeof(WfRelativeLinkDescriptorCollection));
            constKey.Add("EnterEventReceivers", typeof(WfResourceDescriptorCollection));
            constKey.Add("LeaveEventReceivers", typeof(WfResourceDescriptorCollection));
            constKey.Add("InternalRelativeUsers", typeof(WfResourceDescriptorCollection));
            constKey.Add("ExternalUsers", typeof(WfExternalUserCollection));
            constKey.Add("EnterEventExecuteServices", typeof(WfServiceOperationDefinitionCollection));
            constKey.Add("LeaveEventExecuteServices", typeof(WfServiceOperationDefinitionCollection));
            constKey.Add("ParametersNeedToBeCollected", typeof(WfParameterNeedToBeCollected));

            if (dictionary.ContainsKey("Properties"))
            {
                PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);
                properties.Remove(p => string.Compare(p.Definition.Name, "ImportWfMatrix") == 0);
                //properties.Remove(p => string.Compare(p.Definition.Name, "BranchProcessTemplates") == 0);
                actDesp.Properties.Clear();
                foreach (PropertyValue pv in properties)
                {
                    if (constKey.ContainsKey(pv.Definition.Name))
                    {
                        var objValue = JSONSerializerExecute.DeserializeObject(pv.StringValue, constKey[pv.Definition.Name]);
                        activityProperties.Add(pv.Definition.Name, objValue);
                    }
                    else
                    {
                        actDesp.Properties.Add(pv);
                    }
                }
            }
            #endregion
            actDesp.ActivityType = DictionaryHelper.GetValue(dictionary, "ActivityType", WfActivityType.NormalActivity);

            ClearAllProperties(actDesp);
            SetActivityProperties(actDesp, activityProperties, dictionary);

            return actDesp;
        }

        private void ClearAllProperties(WfActivityDescriptor actDesp)
        {
            actDesp.Variables.Clear();
            actDesp.Resources.Clear();
            actDesp.ExternalUsers.Clear();
            actDesp.RelativeLinks.Clear();
            actDesp.EnterEventReceivers.Clear();
            actDesp.LeaveEventReceivers.Clear();
            actDesp.InternalRelativeUsers.Clear();
            actDesp.BranchProcessTemplates.Clear();
            actDesp.EnterEventExecuteServices.Clear();
            actDesp.LeaveEventExecuteServices.Clear();
            actDesp.ParametersNeedToBeCollected.Clear();
        }

        private void SetActivityProperties(WfActivityDescriptor actDesp, Dictionary<string, object> activityProperties, IDictionary<string, object> dictionary)
        {
            if (activityProperties.ContainsKey("Variables"))
            {
                var item = (WfVariableDescriptorCollection)activityProperties["Variables"];
                if (item != null)
                {
                    actDesp.Variables.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("Variables"))
            {
                WfVariableDescriptorCollection variables = JSONSerializerExecute.Deserialize<WfVariableDescriptorCollection>(dictionary["Variables"]);
                actDesp.Variables.CopyFrom(variables);
            }

            if (activityProperties.ContainsKey("Condition"))
            {
                var item = (WfConditionDescriptor)activityProperties["Condition"];
                if (item != null)
                {
                    actDesp.Condition = (WfConditionDescriptor)activityProperties["Condition"];
                }
                else
                {
                    actDesp.Condition.Owner = actDesp;
                }
            }
            else if (dictionary.ContainsKey("Condition"))
            {
                actDesp.Condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(dictionary["Condition"]);
                actDesp.Condition.Owner = actDesp;
            }

            if (activityProperties.ContainsKey("BranchProcessTemplates"))
            {
                var item = (WfBranchProcessTemplateCollection)activityProperties["BranchProcessTemplates"];
                if (item != null)
                {
                    actDesp.BranchProcessTemplates.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("BranchProcessTemplates"))
            {
                WfBranchProcessTemplateCollection templates = JSONSerializerExecute.Deserialize<WfBranchProcessTemplateCollection>(dictionary["BranchProcessTemplates"]);
                actDesp.BranchProcessTemplates.CopyFrom(templates);
            }

            if (activityProperties.ContainsKey("Resources"))
            {
                var item = (WfResourceDescriptorCollection)activityProperties["Resources"];
                if (item != null)
                {
                    actDesp.Resources.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("Resources"))
            {
                WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["Resources"]);
                actDesp.Resources.CopyFrom(resource);
            }

            if (activityProperties.ContainsKey("RelativeLinks"))
            {
                var item = (WfRelativeLinkDescriptorCollection)activityProperties["RelativeLinks"];
                if (item != null)
                {
                    actDesp.RelativeLinks.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("RelativeLinks"))
            {
                WfRelativeLinkDescriptorCollection relativeLinks = JSONSerializerExecute.Deserialize<WfRelativeLinkDescriptorCollection>(dictionary["RelativeLinks"]);
                actDesp.RelativeLinks.CopyFrom(relativeLinks);
            }

            if (activityProperties.ContainsKey("EnterEventReceivers"))
            {
                var item = (WfResourceDescriptorCollection)activityProperties["EnterEventReceivers"];
                if (item != null)
                {
                    actDesp.EnterEventReceivers.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("EnterEventReceivers"))
            {
                WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["EnterEventReceivers"]);
                actDesp.EnterEventReceivers.CopyFrom(resource);
            }

            if (activityProperties.ContainsKey("LeaveEventReceivers"))
            {
                var item = (WfResourceDescriptorCollection)activityProperties["LeaveEventReceivers"];
                if (item != null)
                {
                    actDesp.LeaveEventReceivers.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("LeaveEventReceivers"))
            {
                WfResourceDescriptorCollection resource = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["LeaveEventReceivers"]);
                actDesp.LeaveEventReceivers.CopyFrom(resource);
            }

            if (activityProperties.ContainsKey("InternalRelativeUsers"))
            {
                var item = (WfResourceDescriptorCollection)activityProperties["InternalRelativeUsers"];
                if (item != null)
                {
                    actDesp.InternalRelativeUsers.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("InternalRelativeUsers"))
            {
                WfResourceDescriptorCollection interRelUser = JSONSerializerExecute.Deserialize<WfResourceDescriptorCollection>(dictionary["InternalRelativeUsers"]);
                actDesp.InternalRelativeUsers.CopyFrom(interRelUser);
            }

            if (activityProperties.ContainsKey("ExternalUsers"))
            {
                var item = (WfExternalUserCollection)activityProperties["ExternalUsers"];
                if (item != null)
                {
                    actDesp.ExternalUsers.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("ExternalUsers"))
            {
                WfExternalUserCollection externalUser = JSONSerializerExecute.Deserialize<WfExternalUserCollection>(dictionary["ExternalUsers"]);
                actDesp.ExternalUsers.CopyFrom(externalUser);
            }

            if (activityProperties.ContainsKey("EnterEventExecuteServices"))
            {
                var item = (WfServiceOperationDefinitionCollection)activityProperties["EnterEventExecuteServices"];
                if (item != null)
                {
                    actDesp.EnterEventExecuteServices.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("EnterEventExecuteServices"))
            {
                WfServiceOperationDefinitionCollection svcOperationDef =
                    JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["EnterEventExecuteServices"]);
                actDesp.EnterEventExecuteServices.CopyFrom(svcOperationDef);
            }

            if (activityProperties.ContainsKey("LeaveEventExecuteServices"))
            {
                var item = (WfServiceOperationDefinitionCollection)activityProperties["LeaveEventExecuteServices"];
                if (item != null)
                {
                    actDesp.LeaveEventExecuteServices.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("LeaveEventExecuteServices"))
            {
                WfServiceOperationDefinitionCollection svcOperationDef =
                    JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(dictionary["LeaveEventExecuteServices"]);
                actDesp.LeaveEventExecuteServices.CopyFrom(svcOperationDef);
            }

            if (activityProperties.ContainsKey("ParametersNeedToBeCollected"))
            {
                var item = (WfParameterNeedToBeCollected)activityProperties["ParametersNeedToBeCollected"];
                if (item != null)
                {
                    actDesp.ParametersNeedToBeCollected.CopyFrom(item);
                }
            }
            else if (dictionary.ContainsKey("ParametersNeedToBeCollected"))
            {
                WfParameterNeedToBeCollected parameters =
                     JSONSerializerExecute.Deserialize<WfParameterNeedToBeCollected>(dictionary["ParametersNeedToBeCollected"]);
                actDesp.ParametersNeedToBeCollected.CopyFrom(parameters);
            }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfActivityDescriptor actDesp = (WfActivityDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "ActivityType", actDesp.ActivityType);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "CodeName", actDesp.CodeName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Url", actDesp.Url);

            SetPropertiesValue(actDesp, "Variables", actDesp.Variables);
            SetPropertiesValue(actDesp, "Condition", actDesp.Condition);
            SetPropertiesValue(actDesp, "Resources", actDesp.Resources);
            SetPropertiesValue(actDesp, "RelativeLinks", actDesp.RelativeLinks);
            SetPropertiesValue(actDesp, "BranchProcessTemplates", actDesp.BranchProcessTemplates);
            SetPropertiesValue(actDesp, "EnterEventReceivers", actDesp.EnterEventReceivers);
            SetPropertiesValue(actDesp, "LeaveEventReceivers", actDesp.LeaveEventReceivers);
            SetPropertiesValue(actDesp, "EnterEventExecuteServices", actDesp.EnterEventExecuteServices);
            SetPropertiesValue(actDesp, "LeaveEventExecuteServices", actDesp.LeaveEventExecuteServices);
            SetPropertiesValue(actDesp, "InternalRelativeUsers", actDesp.InternalRelativeUsers);
            SetPropertiesValue(actDesp, "ExternalUsers", actDesp.ExternalUsers);
            SetPropertiesValue(actDesp, "ParametersNeedToBeCollected", actDesp.ParametersNeedToBeCollected);

            return dictionary;
        }

        private void SetPropertiesValue(WfActivityDescriptor actDesp, string propertyName, object input)
        {
            if (actDesp.Properties.ContainsKey(propertyName))
            {
                string itemvalue = JSONSerializerExecute.Serialize(input);
                actDesp.Properties.SetValue<string>(propertyName, itemvalue);
            }
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
