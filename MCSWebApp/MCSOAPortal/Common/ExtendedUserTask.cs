using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.Portal.Common
{
	public class ExtendedUserTask : UserTask
	{
		[ORFieldMapping("PROJECT_NAME")]
		public string ProjectName
		{
			get;
			set;
		}
	}

	public class ExtendedUserTaskCollection : EditableDataObjectCollectionBase<ExtendedUserTask>
	{
	}
}