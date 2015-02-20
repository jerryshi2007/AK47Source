using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
    class RemoveMemberAction : IPendingAction
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
        private bool delayed;

        public SCMemberRelation Relation
        {
            get { return relation; }
        }

        /// <summary>
        /// 使用容器，成员和指定的初始化类型初始化
        /// </summary>
        /// <param name="container"></param>
        /// <param name="member"></param>
        /// <param name="delayInitialize">为true时，关系将延迟到执行前初始化</param>
        public RemoveMemberAction(SchemaObjectBase container, SchemaObjectBase member, bool delayInitialize)
        {
            this.container = container;
            this.member = member;
            this.delayed = delayInitialize;
            if (delayed == false)
            {
                this.relation = (SCMemberRelation)SCMemberRelationAdapter.Instance.Load(container.ID, member.ID);
                if (this.relation == null)
                    throw new Exception("关系尚不存在。");
            }
        }

        public RemoveMemberAction(SchemaObjectBase container, SchemaObjectBase member)
            : this(container, member, false)
        {
        }

        public void DoAction()
        {
            if (delayed)
            {
                this.relation = (SCMemberRelation)SCMemberRelationAdapter.Instance.Load(container.ID, member.ID);
                if (this.relation == null)
                    throw new Exception("关系尚不存在。");
            }

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                SCMemberRelationAdapter.Instance.UpdateStatus(this.relation, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
                SchemaObjectAdapter.Instance.UpdateStatus(this.member, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
                scope.Complete();
            }
        }
    }
}
