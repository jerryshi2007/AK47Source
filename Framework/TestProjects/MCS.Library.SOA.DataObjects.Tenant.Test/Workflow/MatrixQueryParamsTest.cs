using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class MatrixQueryParamsTest
    {
        [TestMethod]
        public void ApprovalMatrixQueryParamMatchProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(approvalMatrix.ID);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            SOARoleContext context = SOARoleContext.CreateContext(approvalMatrix.PropertyDefinitions, process);

            context.QueryParams.Output();

            Assert.AreEqual(1, context.QueryParams.Count);
            Assert.IsNotNull(context.QueryParams[0].QueryValue);
        }

        [TestMethod]
        public void ApprovalMatrixQueryParamMismatchProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(approvalMatrix.ID);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            SOARoleContext context = SOARoleContext.CreateContext(approvalMatrix.PropertyDefinitions, process);

            context.QueryParams.Output();

            Assert.AreEqual(1, context.QueryParams.Count);
            Assert.IsNull(context.QueryParams[0].QueryValue);
        }

        [TestMethod]
        public void ActivityMatrixQueryParamMatchProcessTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp();

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];
            WfActivityMatrixResourceDescriptor resource = (WfActivityMatrixResourceDescriptor)normalActDesp.Resources[0];

            SOARoleContext context = SOARoleContext.CreateContext(resource.PropertyDefinitions, process);

            context.QueryParams.Output();

            Assert.AreEqual(3, context.QueryParams.Count);
            Assert.IsNotNull(context.QueryParams[0].QueryValue);
            Assert.IsNotNull(context.QueryParams[1].QueryValue);
            Assert.IsNotNull(context.QueryParams[2].QueryValue);
        }

        [TestMethod]
        public void ActivityMatrixQueryParamMismatchProcessTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp();

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>());

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];
            WfActivityMatrixResourceDescriptor resource = (WfActivityMatrixResourceDescriptor)normalActDesp.Resources[0];

            SOARoleContext context = SOARoleContext.CreateContext(resource.PropertyDefinitions, process);

            context.QueryParams.Output();

            Assert.AreEqual(3, context.QueryParams.Count);
            Assert.IsNull(context.QueryParams[0].QueryValue);
            Assert.IsNull(context.QueryParams[1].QueryValue);
            Assert.IsNull(context.QueryParams[2].QueryValue);
        }

        [TestMethod]
        public void AllReservedColumnsActivityMatrixQueryParamTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(ActivityMatrixHelper.PrepareReservedActivityMatrixResourceDescriptor());

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>());

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];
            WfActivityMatrixResourceDescriptor resource = (WfActivityMatrixResourceDescriptor)normalActDesp.Resources[0];

            SOARoleContext context = SOARoleContext.CreateContext(resource.PropertyDefinitions, process);

            context.QueryParams.Output();
        }
    }
}
