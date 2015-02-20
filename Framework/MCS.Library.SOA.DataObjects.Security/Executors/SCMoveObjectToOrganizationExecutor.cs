using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public class SCMoveObjectToOrganizationExecutor : SCOrganizationRelativeExecutor
	{
		public SCMoveObjectToOrganizationExecutor(SCOperationType opType, SCOrganization parent, SCBase data)
			: base(opType, parent, data)
		{
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{

		}
	}
}
