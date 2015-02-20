using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public abstract class WfResourceDescriptorConverterBase : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfResourceDescriptor resource = CreateInstance(dictionary, type, serializer);

			return resource;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.Add("shortType", obj.GetType().Name);
			dictionary.Add("__type", obj.GetType().AssemblyQualifiedName);

			return dictionary;
		}

		protected abstract WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);

	}
}
