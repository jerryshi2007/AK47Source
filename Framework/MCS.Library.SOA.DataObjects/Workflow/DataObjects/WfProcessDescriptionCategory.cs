using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.PROCESS_DESCRIPTOR_CATEGORY")]
	public class WfProcessDescriptionCategory
	{
		[ORFieldMapping("ID")]
		public string ID { get; set; }

		[ORFieldMapping("NAME")]
		public string Name { get; set; }
	}

	[Serializable]
	[XElementSerializable]
	public class WfProcessDescriptionCategoryCollection :
		EditableDataObjectCollectionBase<WfProcessDescriptionCategory>
	{
	}
}
