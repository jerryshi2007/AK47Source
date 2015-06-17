using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfAclAdapter : UpdatableAndLoadableAdapterBase<WfAclItem, WfAclItemCollection>
    {
        public static readonly WfAclAdapter Instance = new WfAclAdapter();

        private WfAclAdapter()
        {
        }

        public WfAclItemCollection LoadByResourceID(string resourceID)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            return Load(builder => builder.AppendItem("RESOURCE_ID", resourceID).AppendTenantCode());
        }

        public void Update(WfAclItemCollection aclItems)
        {
            aclItems.NullCheck("aclItems");

            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                AclSettings.GetConfig().Operations.ForEach(op => op.Update(aclItems));

                scope.Complete();
            }
        }

        public ConnectiveSqlClauseCollection GetAclQueryConditionsByUser(string userID)
        {
            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

            WhereSqlClauseBuilder builderUser = new WhereSqlClauseBuilder();

            builderUser.AppendItem("OBJECT_ID", userID);
            builderUser.AppendTenantCode();

            result.Add(builderUser);

            return result;
        }

        /// <summary>
        /// 根据已经存在的acl中的resourceID，得到相关acl
        /// </summary>
        /// <param name="acl"></param>
        /// <returns></returns>
        private WfAclItemCollection GetAclFromAclResourceIDs(WfAclItemCollection acl)
        {
            Dictionary<string, string> resourceIDDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (WfAclItem item in acl)
                resourceIDDict[item.ResourceID] = item.ResourceID;

            WfAclItemCollection result = null;

            if (resourceIDDict.Count > 0)
            {
                result = Load(builder =>
                {
                    builder.LogicOperator = LogicOperatorDefine.Or;

                    foreach (KeyValuePair<string, string> kp in resourceIDDict)
                        builder.AppendItem("RESOURCE_ID", kp.Key).AppendTenantCode();
                });
            }
            else
                result = new WfAclItemCollection();

            return result;
        }

        protected override string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
        }
    }
}
