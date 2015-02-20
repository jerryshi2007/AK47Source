using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
    public class ProcessDescriptorInfoQuery1 : ObjectDataSourceQueryAdapterBase<WfProcessDescriptorInfo, WfProcessDescriptorInfoCollection>
	{
		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
            qc.OrderByClause = "APPLICATION_NAME";
            qc.SelectFields = "PROCESS_KEY,APPLICATION_NAME,PROGRAM_NAME,PROCESS_NAME,CREATE_TIME,CREATOR_ID,CREATOR_NAME,MODIFY_TIME,MODIFIER_ID,MODIFIER_NAME";
        }
	}
}
