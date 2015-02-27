using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.DataObjects;

namespace WfOperationServices.Test.DataObjects
{
    [TestClass]
    public class DataSourceServiceTest
    {
        [TestMethod]
        public void QueryUserOperationLogByResourceIDTest()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientUserOperationLogPageQueryResult result =
                WfClientDataSourceServiceProxy.Instance.QueryUserOperationLogByResourceID(processDesp.Key, 0, 1, string.Empty, -1);

            Assert.IsTrue(result.QueryResult.Count > 0);

            Assert.AreEqual(processDesp.Key, result.QueryResult[0].ResourceID);

            WfClientUserOperationLog log = WfClientDataSourceServiceProxy.Instance.GetUserOperationLogByID(result.QueryResult[0].ID);

            Assert.AreEqual(result.QueryResult[0].ID, log.ID);
        }
    }
}
