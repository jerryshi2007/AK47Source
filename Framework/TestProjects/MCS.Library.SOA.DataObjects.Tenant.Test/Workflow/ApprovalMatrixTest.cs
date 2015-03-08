using MCS.Library.Office.OpenXml.Excel;
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

            Assert.AreEqual(WfMatrixType.ApprovalMatrix, matrix.PropertyDefinitions.MatrixType);

            WfApprovalMatrixAdapter.Instance.Update(matrix);

            WfApprovalMatrix loaded = WfApprovalMatrixAdapter.Instance.LoadByID(matrix.ID);

            Assert.AreEqual(matrix.PropertyDefinitions.Count, loaded.PropertyDefinitions.Count);
            Assert.AreEqual(matrix.Rows.Count, loaded.Rows.Count);
        }

        [TestMethod]
        public void ApprovalMatrixToExcelTest()
        {
            WfApprovalMatrix matrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WorkBook workBook = matrix.ToWorkBook();

            workBook.Save("ApprovalMatrixToExcelTest.xlsx");

            WfApprovalMatrix deserialized = workBook.ToApprovalMatrix();

            deserialized.ID = matrix.ID;

            Assert.AreEqual(matrix.PropertyDefinitions.Count, deserialized.PropertyDefinitions.Count);
            Assert.AreEqual(matrix.Rows.Count, deserialized.Rows.Count);
        }
    }
}
