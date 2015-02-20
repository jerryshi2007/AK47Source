using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.TA.Bizlet.Impl.Test;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Unitest
{
    [TestClass]
    public class TenantSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            IoCConfig.Start();
        }
        [TestMethod]
        public void GetEffectiveElement()
        {
            TenantSection tenantSection = ConfigurationManager.GetSection("tenantSetting") as TenantSection;
            var tenants=tenantSection.Tenants.GetEffectiveElement();
            Assert.IsTrue(null != tenants);
        }
        [TestMethod]
        public void GetUnitWorkName()
        {
            TenantSection tenantSection = ConfigurationManager.GetSection("tenantSetting") as TenantSection;
            var tenants = tenantSection.Tenants.GetEffectiveElement().Name;
            Assert.IsTrue(!string.IsNullOrEmpty(tenants));
        }
        [TestMethod]
        public void GetUnitWork()
        {
            IUnitOfWork unitOfWork = TenantHelper.GetUnitWOrk("Test1");
            Assert.IsNotNull(unitOfWork);
        }
        [TestMethod]
        public void GetUserTask()
        {
            //localhost.TaskPlugin taskplguin = new localhost.TaskPlugin();
            //var tasks=taskplguin.QueryTask("Test1", "503d350d-f46a-4590-8c43-9849e7741467", localhost.TaskStatus.Unprocessed, 0, 100, null);
            //foreach (var item in tasks.Items)
            //{
            //    Console.WriteLine(item.TASK_GUID + ";" + item.SEND_TO_USER_NAME);
            //}
        }
    }
}
