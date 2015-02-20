using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public class SCMemberRelativeExecutor : SCMemberRelativeExecutorBase
	{
		public SCMemberRelativeExecutor(SCOperationType opType, SchemaObjectBase container, SCBase member)
			: base(opType, container, member)
		{
		}

		protected override SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member)
		{
			return new SCMemberRelation(container, member);
		}
	}
}
