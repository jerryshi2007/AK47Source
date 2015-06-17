using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.PROCESS_DIMENSIONS")]
    [TenantRelativeObject]
	public class WfProcessDimension
	{
		[ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
		public string ProcessID
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

		public static WfProcessDimension FromProcess(IWfProcess process)
		{
			process.NullCheck("process");

			WfProcessDimension result = new WfProcessDimension();

			result.ProcessID = process.ID;

			XElement xml = XElement.Parse("<Process/>");

			((ISimpleXmlSerializer)process).ToXElement(xml, string.Empty);

			result.Data = xml.ToString();

			return result;
		}
	}

	[Serializable]
	public class WfProcessDimensionCollection : EditableDataObjectCollectionBase<WfProcessDimension>
	{
	}
}
