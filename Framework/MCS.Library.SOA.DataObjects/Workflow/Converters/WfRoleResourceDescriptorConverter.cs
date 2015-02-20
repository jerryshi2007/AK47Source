using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfRoleResourceDescriptorConverter : WfResourceDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfRoleResourceDescriptor roleResourceDesp = (WfRoleResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			IRole role = JSONSerializerExecute.Deserialize<OguRole>(dictionary["Role"]);
			roleResourceDesp.Role = role;

			return roleResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfRoleResourceDescriptor roleResourceDesp = (WfRoleResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			dictionary.Add("Role", roleResourceDesp.Role);
			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfRoleResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfRoleResourceDescriptor();
		}
	}
}
