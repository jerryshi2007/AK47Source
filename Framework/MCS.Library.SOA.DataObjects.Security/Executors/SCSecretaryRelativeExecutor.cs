using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public class SCSecretaryRelativeExecutor : SCMemberRelativeExecutorBase
	{
		public SCSecretaryRelativeExecutor(SCOperationType opType, SchemaObjectBase container, SCBase member)
			: base(opType, container, member)
		{
		}

		protected override SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member)
		{
			return new SCSecretaryRelation(container, member);
		}

		protected override void Validate()
		{
			base.Validate();

			(this.Parent.ID == this.Data.ID).TrueThrow("不能添加自身为秘书。");
		}
	}
}
