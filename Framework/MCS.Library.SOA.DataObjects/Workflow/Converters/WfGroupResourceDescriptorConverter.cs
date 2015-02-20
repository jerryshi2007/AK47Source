using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfGroupResourceDescriptorConverter : WfResourceDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfGroupResourceDescriptor groupResourceDesp = (WfGroupResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			IGroup group = JSONSerializerExecute.Deserialize<OguGroup>(dictionary["Group"]);
			groupResourceDesp.Group = group;

			return groupResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfGroupResourceDescriptor groupResourceDesp = (WfGroupResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);
			dictionary.Add("Group", groupResourceDesp.Group);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfGroupResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfGroupResourceDescriptor();
		}
	}
}
