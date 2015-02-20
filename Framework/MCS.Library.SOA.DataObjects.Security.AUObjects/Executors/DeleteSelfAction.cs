using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Transactions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	/// <summary>
	/// 将自身从其容器中静默删除(注意不会自动删除其成员和关系)
	/// </summary>
	class DeleteSelfAction : IPendingAction
	{
		private SchemaObjectBase item;
		private SCParentsRelationObjectCollection parentRelations;
		private SCMemberRelationCollectionBase containerRelations;


		public DeleteSelfAction(SchemaObjectBase item)
		{
			this.item = item;
			item.ClearRelativeData();
			this.parentRelations = item.CurrentParentRelations;
			this.containerRelations = SCMemberRelationAdapter.Instance.LoadByMemberID(item.ID).FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
		}

		public void DoAction()
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				foreach (SCSimpleRelationBase item in this.containerRelations)
				{
					PC.Adapters.SCMemberRelationAdapter.Instance.UpdateStatus(item, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
				}

				foreach (SCRelationObject item in this.parentRelations)
				{
					PC.Adapters.SchemaRelationObjectAdapter.Instance.UpdateStatus(item, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
				}

				PC.Adapters.SchemaObjectAdapter.Instance.UpdateStatus(this.item, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);

				scope.Complete();
			}
		}
	}
}
