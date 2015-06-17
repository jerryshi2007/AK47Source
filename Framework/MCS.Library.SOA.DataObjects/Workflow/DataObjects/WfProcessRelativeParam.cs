using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.PROCESS_RELATIVE_PARAMS")]
    [TenantRelativeObject]
	public class WfProcessRelativeParam
	{
		[ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
		public string ProcessID
		{
			get;
			set;
		}

		[ORFieldMapping("PARAM_KEY", PrimaryKey = true)]
		public string ParamKey
		{
			get;
			set;
		}

		[ORFieldMapping("PARAM_VALUE")]
		public string ParamValue
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WfProcessRelativeParamCollection : EditableDataObjectCollectionBase<WfProcessRelativeParam>
	{
	}
}
