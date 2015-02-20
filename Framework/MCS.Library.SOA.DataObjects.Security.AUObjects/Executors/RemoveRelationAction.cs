using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	class RemoveRelationAction : IPendingAction
	{
		private SCSimpleRelationBase item;

		public RemoveRelationAction(SCSimpleRelationBase item)
		{
			this.item = item;
		}

		public void DoAction()
		{
			SCMemberRelationAdapter.Instance.UpdateStatus(this.item, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
		}
	}
}
