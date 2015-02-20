using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	public class AUOperationCacheNotifierAction : IAUObjectOperationAction
	{
		public void AfterExecute(Executors.AUOperationType operationType)
		{
		}

		public void BeforeExecute(Executors.AUOperationType operationType)
		{
			switch (operationType)
			{
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.None:
					break;
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.AddAdminSchema:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.RemoveAdminSchema:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.UpdateAdminSchema:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.AddAdminUnit:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.RemoveAdminUnit:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.UpdateAdminUnit:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.AddSchemaRole:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.RemoveSchemaRole:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.UpdateAdminSchemaRole:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.ModifyAURoleMembers:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.AddAUScopeItem:
				case MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUOperationType.RemoveAUScopeItem:
				default:
					AUCacheHelper.InvalidateAllCache();
					break;
			}
		}
	}
}
