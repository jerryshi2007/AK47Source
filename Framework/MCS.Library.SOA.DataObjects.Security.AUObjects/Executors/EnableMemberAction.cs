using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	class EnableMemberAction : IPendingAction
	{
		private SchemaObjectBase container;

		public SchemaObjectBase Container
		{
			get { return container; }
		}

		private SchemaObjectBase member;

		public SchemaObjectBase Member
		{
			get { return member; }
		}

		private SCMemberRelation relation;

		public SCMemberRelation Relation
		{
			get { return relation; }
		}

		public EnableMemberAction(SchemaObjectBase container, SchemaObjectBase member)
		{
			this.container = container;
			this.member = member;
			this.relation = (SCMemberRelation)SCMemberRelationAdapter.Instance.Load(container.ID, member.ID);
		}

		public void DoAction()
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SchemaObjectAdapter.Instance.UpdateStatus(this.member, Schemas.SchemaProperties.SchemaObjectStatus.Normal);
				SCMemberRelationAdapter.Instance.UpdateStatus(this.relation, Schemas.SchemaProperties.SchemaObjectStatus.Normal);
				scope.Complete();
			}
		}
	}
}
