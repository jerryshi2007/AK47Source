using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Data;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
    class AddMemberAction : IPendingAction
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

        public AddMemberAction(SchemaObjectBase container, SchemaObjectBase member)
        {
            this.container = container;
            this.member = member;

            this.relation = new SCMemberRelation(container, member);
            if (member.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
                relation.Status = Schemas.SchemaProperties.SchemaObjectStatus.Deleted;
        }

        public virtual void DoAction()
        {
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                PC.Adapters.SchemaObjectAdapter.Instance.Update(this.member);
                PC.Adapters.SCMemberRelationAdapter.Instance.Update(this.relation);
                scope.Complete();
            }
        }
    }
}
