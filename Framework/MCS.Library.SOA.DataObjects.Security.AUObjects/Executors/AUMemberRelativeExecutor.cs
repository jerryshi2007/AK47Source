using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	internal class AUMemberRelativeExecutor : AUMemberRelativeExecutorBase
	{
		public AUMemberRelativeExecutor(AUOperationType opType, SchemaObjectBase container, SchemaObjectBase member)
			: base(opType, container, member)
		{
		}

		protected override SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member)
		{
			return new SCMemberRelation(container, member);
		}
	}
}
