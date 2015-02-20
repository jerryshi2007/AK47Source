using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.Stat.Common
{
	public class AppCommonInfoProcess : AppCommonInfo
	{
		public string ProcessID
		{
			set;
			get;
		}
	}

	public class AppCommonInfoProcessCollection : EditableDataObjectCollectionBase<AppCommonInfoProcess>
	{
 
	}
}