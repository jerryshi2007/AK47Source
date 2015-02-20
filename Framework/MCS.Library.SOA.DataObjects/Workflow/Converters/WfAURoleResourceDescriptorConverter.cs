using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfAURoleResourceDescriptorConverter : WfResourceDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfAURoleResourceDescriptor auRoleResourceDesp = (WfAURoleResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			WrappedAUSchemaRole role = JSONSerializerExecute.Deserialize<WrappedAUSchemaRole>(dictionary["AUSchemaRole"]);
			auRoleResourceDesp.AUSchemaRole = role;

			return auRoleResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfAURoleResourceDescriptor auRoleResourceDesp = (WfAURoleResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			dictionary.Add("AUSchemaRole", auRoleResourceDesp.AUSchemaRole);

			return dictionary;
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfAURoleResourceDescriptor();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfAURoleResourceDescriptor) }; }
		}
	}
}
