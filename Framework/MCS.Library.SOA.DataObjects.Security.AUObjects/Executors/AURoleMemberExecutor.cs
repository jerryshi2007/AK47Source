using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	/// <summary>
	/// 替换角色成员
	/// </summary>
	class AURoleMemberExecutor : AUExecutorBase
	{
		private AUSchemaRole schemaRole, actualSchemaRole;
		private SCUser[] users;
		private AdminUnit unit, actualUnit;
		private AURole actualUnitRole;
		private PendingActionCollection pendingActions = new PendingActionCollection();

		public AURoleMemberExecutor(AUOperationType opType, AUSchemaRole role, AdminUnit unit, SCUser[] users)
			: base(opType)
		{
			role.NullCheck("role");
			users.NullCheck("users");
			unit.NullCheck("unit");
			this.schemaRole = role;
			this.users = users;
			this.unit = unit;
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			AUCommon.DoDbAction(() =>
			{
				actualSchemaRole = (AUSchemaRole)SchemaObjectAdapter.Instance.Load(schemaRole.ID);
				if (actualSchemaRole.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
					throw new AUStatusCheckException(actualSchemaRole, this.OperationType);

				this.actualUnit = (AdminUnit)SchemaObjectAdapter.Instance.Load(this.unit.ID);
				if (this.actualUnit == null || this.actualUnit.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
					throw new AUStatusCheckException(actualUnit, this.OperationType);

				this.actualUnitRole = Adapters.AUSnapshotAdapter.Instance.LoadAURole(actualSchemaRole.ID, actualUnit.ID, true, DateTime.MinValue);
				if (actualUnitRole == null || actualUnitRole.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
					throw new AUStatusCheckException(actualUnitRole, this.OperationType);

				var roleMemberRelations = SCMemberRelationAdapter.Instance.LoadByContainerID(this.actualUnitRole.ID);

				Dictionary<string, SCUser> userDic = users.ToDictionary(m => m.ID);

				foreach (var item in roleMemberRelations)
				{
					if (item.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal)
					{
						if (userDic.ContainsKey(item.ID))
						{
							userDic.Remove(item.ID);
						}
						else
						{
							pendingActions.Add(new RemoveRelationAction(item));
						}
					}
				}

				foreach (SCUser user in userDic.Values)
				{
					pendingActions.Add(new AddRelationAction(this.actualUnitRole, user));
				}
			});
		}

		protected override object DoOperation(AUObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				pendingActions.DoActions();
				scope.Complete();
			}

			return this.actualUnitRole;
		}

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			base.PrepareOperationLog(context);
		}

		public bool NeedStatusCheck { get; set; }

		public bool NeedContainerStatusCheck { get; set; }
	}
}
