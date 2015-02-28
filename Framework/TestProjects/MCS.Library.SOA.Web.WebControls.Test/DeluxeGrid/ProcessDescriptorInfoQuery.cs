using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeGrid
{
	public class ProcessDescriptorInfoQuery : ObjectDataSourceQueryAdapterBase<WfProcessDescriptorInfo, WfProcessDescriptorInfoCollection>
	{
		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.OrderByClause = "APPLICATION_NAME";
            qc.WhereClause = "PROCESS_KEY IS NULL";
			qc.SelectFields = "PROCESS_KEY,APPLICATION_NAME,PROGRAM_NAME,PROCESS_NAME,CREATE_TIME,CREATOR_ID,CREATOR_NAME,MODIFY_TIME,MODIFIER_ID,MODIFIER_NAME";
		}
	}
}