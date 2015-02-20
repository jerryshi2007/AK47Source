using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	[Flags]
	public enum OuBrowseViewMode
	{
		Auto = 0,
		Fixed = 1,
		List = 0,
		Hierarchical = 2,
	}

	[Flags]
	public enum UserBrowseViewMode
	{
		Auto = 0,
		Fixed = 1,
		DetailList = 0,
		ReducedList = 2,
		ReducedTable = 4,
	}

	[Flags]
	public enum GeneralViewMode
	{
		Auto = 0,
		Fixed = 1,
		List = 0,
		Table = 2,
	}
}