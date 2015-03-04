using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class ApprovalMatrixTest
    {
        [TestMethod]
        public void UpdateApprovalMatrixTest()
        {
            WfApprovalMatrix matrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(matrix);

            WfApprovalMatrix loaded = WfApprovalMatrixAdapter.Instance.LoadByID(matrix.ID);

            Assert.AreEqual(matrix.PropertyDefinitions.Count, loaded.PropertyDefinitions.Count);
            Assert.AreEqual(matrix.Rows.Count, loaded.Rows.Count);
        }
    }
}
