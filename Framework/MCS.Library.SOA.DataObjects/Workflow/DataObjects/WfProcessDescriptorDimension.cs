using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.PROCESS_DESCRIPTOR_DIMENSIONS")]
	public class WfProcessDescriptorDimension
	{
		[ORFieldMapping("PROCESS_KEY", PrimaryKey = true)]
		public string ProcessKey
		{
			get;
			set;
		}

		[ORFieldMapping("DATA")]
		public string Data
		{
			get;
			set;
		}

		[ORFieldMapping("UPDATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Update | ClauseBindingFlags.Where, DefaultExpression = "GETDATE()")]
		public DateTime UpdateTime
		{
			get;
			set;
		}

		public static WfProcessDescriptorDimension FromProcessDescriptor(IWfProcessDescriptor processDesp)
		{
			processDesp.NullCheck("processDesp");

			WfProcessDescriptorDimension result = new WfProcessDescriptorDimension();

			result.ProcessKey = processDesp.Key;

			XElement xml = XElement.Parse("<Process/>");

			((ISimpleXmlSerializer)processDesp).ToXElement(xml, string.Empty);

			result.Data = xml.ToString();

			return result;
		}
	}

	[Serializable]
	public class WfProcessDescriptorDimensionCollection : EditableDataObjectCollectionBase<WfProcessDescriptorDimension>
	{
	}
}
