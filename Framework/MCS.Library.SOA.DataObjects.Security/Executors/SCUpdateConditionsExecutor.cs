using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 更新条件
	/// </summary>
	public class SCUpdateConditionsExecutor : SCExecutorBase
	{
		private SCConditionOwner _Owner = null;

		private SCBase ownerObj = null;

		public SCUpdateConditionsExecutor(SCOperationType opType, SCConditionOwner owner)
			: base(opType)
		{
			owner.NullCheck("owner");

			this._Owner = owner;
		}

		public SCConditionOwner Owner
		{
			get
			{
				return this._Owner;
			}
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this._Owner.OwnerID;
			log.SchemaType = this.ownerObj.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this.ownerObj.Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this.ownerObj.Name);

			log.SearchContent = this.ownerObj.ToFullTextString();

			context.Logs.Add(log);
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			base.PrepareData(context);

			this.ownerObj = (SCBase)SchemaObjectAdapter.Instance.Load(this._Owner.OwnerID);

			if (ownerObj == null || ownerObj.Status != SchemaObjectStatus.Normal)
				throw new SCStatusCheckException("指定条件容器对象不存在。");
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SCConditionAdapter.Instance.UpdateConditions(this.Owner.OwnerID, this.Owner.Type, this.Owner.Conditions);

				scope.Complete();
			}

			return this._Owner;
		}
	}
}
