using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfActivityOperatorResourceDescriptorConverter : WfResourceDescriptorConverterBase  
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{

			WfActivityOperatorResourceDescriptor activityOperatorResDesp = (WfActivityOperatorResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			activityOperatorResDesp.ActivityKey = DictionaryHelper.GetValue(dictionary, "ActivityKey", string.Empty);

			return activityOperatorResDesp;
			 
		}

		public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfActivityOperatorResourceDescriptor activityOperatorResDesp = (WfActivityOperatorResourceDescriptor)obj;
			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);
			
			dictionary.Add("ActivityKey", activityOperatorResDesp.ActivityKey);
			
			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(WfActivityOperatorResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfActivityOperatorResourceDescriptor();
		}
	}
}
