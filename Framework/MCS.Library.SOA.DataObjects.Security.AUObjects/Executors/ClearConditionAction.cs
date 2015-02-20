using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	class ClearConditionAction : IPendingAction
	{
		private SchemaObjectBase owner;

		public ClearConditionAction(SchemaObjectBase owner)
		{
			this.owner = owner;
		}

		public void DoAction()
		{
			Adapters.AUConditionAdapter.Instance.DeleteByOwner(owner.ID, AUCommon.ConditionType);
		}
	}
}
