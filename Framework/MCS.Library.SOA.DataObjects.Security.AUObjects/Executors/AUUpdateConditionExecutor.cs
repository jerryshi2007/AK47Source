using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Executors;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	public class AUUpdateConditionsExecutor : AUExecutorBase
	{
		private AUAdminScope scope;
		private SCCondition condition;

		public AUUpdateConditionsExecutor(AUOperationType opType, AUAdminScope scope, SCCondition condition)
			: base(opType)
		{
			(this.scope = scope).NullCheck("scope");
			(this.condition = condition).NullCheck("condition");
		}

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this.scope.ID;
			log.SchemaType = this.scope.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this.scope.Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this.scope.ID);

			log.SearchContent = "";// this.ownerObj.ToFullTextString();

			context.Logs.Add(log);
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			base.PrepareData(context);

			this.scope = (AUAdminScope)SchemaObjectAdapter.Instance.Load(this.scope.ID);

			if (this.scope == null || this.scope.Status != SchemaObjectStatus.Normal)
				throw new SCStatusCheckException("指定条件容器对象不存在。");
		}

		protected override object DoOperation(AUObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SCConditionCollection collection = new SCConditionCollection();
				collection.Add(condition);
				AUConditionAdapter.Instance.UpdateConditions(this.scope.ID, "ADM", collection);

				scope.Complete();
			}

			return this.scope;
		}
	}
}
