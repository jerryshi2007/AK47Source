using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	internal class PendingActionCollection : EditableDataObjectCollectionBase<IPendingAction>
	{
		public void DoActions()
		{
			foreach (IPendingAction action in this)
			{
				action.DoAction();
			}

			this.Clear();
		}
	}
}
