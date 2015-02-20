using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	class ClearContainerAction : IPendingAction
	{
		private SchemaObjectBase container;

		public SchemaObjectBase Container
		{
			get { return container; }
		}

		public ClearContainerAction(SchemaObjectBase container)
		{
			this.container = container;
		}

		public void DoAction()
		{
			RelationHelper.Instance.ClearContainer(this.container);
		}
	}
}
