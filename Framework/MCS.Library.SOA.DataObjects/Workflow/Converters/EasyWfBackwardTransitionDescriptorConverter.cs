using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class EasyWfBackwardTransitionDescriptorConverter : EasyWfTransitionDescriptorConverterBase
	{
		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfBackwardTransitionDescriptor();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(WfBackwardTransitionDescriptor), typeof(IWfBackwardTransitionDescriptor) };
			}
		}
	}
}
