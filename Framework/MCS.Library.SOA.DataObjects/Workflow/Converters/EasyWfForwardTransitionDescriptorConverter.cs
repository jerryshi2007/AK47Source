using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	public class EasyWfForwardTransitionDescriptorConverter : EasyWfTransitionDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfForwardTransitionDescriptor forwardTranDesp = (WfForwardTransitionDescriptor)base.Deserialize(dictionary, type, serializer);
			
			return forwardTranDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfForwardTransitionDescriptor forwardTranDesp = (WfForwardTransitionDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			return dictionary;
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfForwardTransitionDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfForwardTransitionDescriptor), typeof(IWfForwardTransitionDescriptor) }; }
		}
	}
}
