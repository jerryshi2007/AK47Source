using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;

namespace MCS.Library.WF.Contracts.Json.Converters
{
	public class WfCreateClientDynamicProcessParamsJsonConverter : WfClientKeyedDescriptorJsonConverterBase
	{
		private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfCreateClientDynamicProcessParams) };

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);

			WfCreateClientDynamicProcessParams createParams = (WfCreateClientDynamicProcessParams)obj;

			dictionary["activityMatrix"] = createParams.ActivityMatrix;

			return dictionary;
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfCreateClientDynamicProcessParams createParams = (WfCreateClientDynamicProcessParams)base.Deserialize(dictionary, type, serializer);

			createParams.ActivityMatrix = JSONSerializerExecute.Deserialize<WfClientActivityMatrixResourceDescriptor>(dictionary.GetValue("activityMatrix", (object)null));

			return createParams;
		}

		protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			return new WfCreateClientDynamicProcessParams() { Key = key };
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return _SupportedTypes;
			}
		}
	}
}
