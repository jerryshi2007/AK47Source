using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfProcessDescriptionCategoryAdapter :
		   UpdatableAndLoadableAdapterBase<WfProcessDescriptionCategory, WfProcessDescriptionCategoryCollection>
	{
		public static readonly WfProcessDescriptionCategoryAdapter Instance = new WfProcessDescriptionCategoryAdapter();

		private WfProcessDescriptionCategoryAdapter() { }
	}
}
