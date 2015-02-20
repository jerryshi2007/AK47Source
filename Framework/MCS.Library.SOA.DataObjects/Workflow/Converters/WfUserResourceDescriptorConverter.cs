using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfUserResourceDescriptorConverter : WfResourceDescriptorConverterBase 
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfUserResourceDescriptor userResourceDesp = (WfUserResourceDescriptor)base.Deserialize(dictionary, type, serializer);
			IUser user = JSONSerializerExecute.Deserialize<OguUser>(dictionary["User"]);
			userResourceDesp.User = user;

			return userResourceDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfUserResourceDescriptor userResourceDesp = (WfUserResourceDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			dictionary.Add("User", userResourceDesp.User);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfUserResourceDescriptor) }; }
		}

		protected override WfResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfUserResourceDescriptor();
		}
	}
}
