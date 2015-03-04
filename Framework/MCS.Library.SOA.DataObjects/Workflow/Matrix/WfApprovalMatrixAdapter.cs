using MCS.Library.Core;
using MCS.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfApprovalMatrixAdapter
    {
        public static readonly WfApprovalMatrixAdapter Instance = new WfApprovalMatrixAdapter();

        private WfApprovalMatrixAdapter()
        {
        }

        public WfApprovalMatrix LoadByID(string id)
        {
            id.NullCheck("id");

            WfApprovalMatrix matrix = new WfApprovalMatrix() { ID = id };

            matrix.PropertyDefinitions = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(id);
            matrix.Rows = SOARolePropertiesAdapter.Instance.LoadByRoleID(id, null);

            return matrix;
        }

        public WfApprovalMatrix GetByID(string id)
        {
            id.NullCheck("id");

            WfApprovalMatrix matrix = new WfApprovalMatrix() { ID = id };

            matrix.PropertyDefinitions = SOARolePropertyDefinitionAdapter.Instance.GetByRoleID(id);
            matrix.Rows = SOARolePropertiesAdapter.Instance.GetByRoleID(id);

            return matrix;
        }

        /// <summary>
        /// 更新矩阵
        /// </summary>
        /// <param name="matrix"></param>
        public void Update(WfApprovalMatrix matrix)
        {
            matrix.NullCheck("matrix");
            matrix.ID.CheckStringIsNullOrEmpty("matrix.ID");

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                SOARolePropertyDefinitionAdapter.Instance.Update(matrix.ID, matrix.PropertyDefinitions);
                SOARolePropertiesAdapter.Instance.Update(matrix.ID, matrix.Rows);

                scope.Complete();
            }
        }
    }
}
