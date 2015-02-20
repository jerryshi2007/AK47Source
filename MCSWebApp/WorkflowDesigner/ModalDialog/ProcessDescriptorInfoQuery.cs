using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace WorkflowDesigner.ModalDialog
{
	public class ProcessDescriptorInfoQuery : ObjectDataSourceQueryAdapterBase<WfProcessDescriptorInfo, WfProcessDescriptorInfoCollection>
	{
		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "APPLICATION_NAME";

			qc.SelectFields = "PROCESS_KEY, APPLICATION_NAME, PROGRAM_NAME, PROCESS_NAME, ENABLED, CREATE_TIME, CREATOR_ID, CREATOR_NAME, MODIFY_TIME, MODIFIER_ID, MODIFIER_NAME, IMPORT_TIME, IMPORT_USER_ID, IMPORT_USER_NAME";
		}
	}
}