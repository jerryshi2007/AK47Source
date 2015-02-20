using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    [TestClass]
    public class ActivityMatrixTest
    {
        [TestMethod]
        [Description("资源为活动矩阵时的，内嵌方法RowOperators.Count的测试")]
        public void ActiveMatrixRowOperatorsCountTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp();
            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);

            Assert.AreEqual(5, process.Activities.Count);
        }
    }
}
