using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public interface IWfMatrixQuery
	{
		WfMatrix Query(WfMatrixQueryParamCollection param);
	}
}
