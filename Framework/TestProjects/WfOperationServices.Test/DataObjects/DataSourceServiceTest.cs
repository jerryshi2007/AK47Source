using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        [TestMethod]
        public void GetAllApplicationsTest()
        {
            WfClientApplicationCollection applications = WfClientDataSourceServiceProxy.Instance.GetAllApplications();

            Assert.IsTrue(applications.Count > 0);
        }

        [TestMethod]
        public void GetProgramsByApplicationTest()
        {
            WfClientApplicationCollection applications = WfClientDataSourceServiceProxy.Instance.GetAllApplications();

            Assert.IsTrue(applications.Count > 0);

            WfClientProgramInApplicationCollection programs = WfClientDataSourceServiceProxy.Instance.GetProgramsByApplication(applications[0].CodeName);

            Assert.IsTrue(programs.Count > 0);
            Assert.AreEqual(applications[0].CodeName, programs[0].ApplicationCodeName);
        }
    }
}
