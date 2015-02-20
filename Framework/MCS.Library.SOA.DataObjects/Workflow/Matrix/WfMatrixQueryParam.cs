using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfMatrixQueryParam
	{
		public string QueryName { get; set; }
		public string QueryValue { get; set; }
	}

	public class WfMatrixQueryParamCollection : EditableDataObjectCollectionBase<WfMatrixQueryParam>
	{
		public WfMatrixQueryParamCollection(string wfMatrixID)
		{
			this.Add(new WfMatrixQueryParam() { QueryName = "MATRIX_ID", QueryValue = wfMatrixID });
		}
	}
}
