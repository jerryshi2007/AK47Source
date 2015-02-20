using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfDepartmentResourceDescriptorConverter : WfResourceDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfDepartmentResourceDescriptor deptResourceDesp = (WfDepartmentResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			IOrganization orga = JSONSerializerExecute.Deserialize<OguOrganization>(dictionary["Department"]);
			deptResourceDesp.Department = orga;
			return deptResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfDepartmentResourceDescriptor deptResourceDesp = (WfDepartmentResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);
			dictionary.Add("Department", deptResourceDesp.Department);
			return dictionary;

		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfDepartmentResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfDepartmentResourceDescriptor();
		}
	}
}
