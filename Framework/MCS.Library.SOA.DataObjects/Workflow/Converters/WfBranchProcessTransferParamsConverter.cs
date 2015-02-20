using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfBranchProcessTransferParamsConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfBranchProcessTransferParams data = new WfBranchProcessTransferParams();

			data.Template = JSONSerializerExecute.Deserialize<WfBranchProcessTemplateDescriptor>(dictionary["Template"]);
			data.BranchParams.CopyFrom(JSONSerializerExecute.Deserialize<WfBranchProcessStartupParamsCollection>(dictionary["BranchParams"]));

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfBranchProcessTransferParams data = (WfBranchProcessTransferParams)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary["Template"] = data.Template;
			dictionary["BranchParams"] = data.BranchParams;

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfBranchProcessTransferParams) }; }
		}
	}
}
