using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	class AddRelationAction : IPendingAction
	{
		private SCMemberRelation relation;

		public AddRelationAction(SchemaObjectBase container, SchemaObjectBase member)
		{
			this.relation = new SCMemberRelation(container, member);
		}

		public void DoAction()
		{
			SCMemberRelationAdapter.Instance.Update(this.relation);
		}
	}
}
