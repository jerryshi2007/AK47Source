using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfActivityAssigneesResourceDescriptorConverter : WfResourceDescriptorConverterBase 
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfActivityAssigneesResourceDescriptor activityAssigneesResDesp = (WfActivityAssigneesResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			activityAssigneesResDesp.ActivityKey = DictionaryHelper.GetValue(dictionary, "ActivityKey", string.Empty);
			return activityAssigneesResDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfActivityAssigneesResourceDescriptor activityAssigneesResDesp = (WfActivityAssigneesResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);
			dictionary.Add("ActivityKey", activityAssigneesResDesp.ActivityKey);
			return dictionary;
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfActivityAssigneesResourceDescriptor();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(WfActivityAssigneesResourceDescriptor) }; }
		}
	}
}
