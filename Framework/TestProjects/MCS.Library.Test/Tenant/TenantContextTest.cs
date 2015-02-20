using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Test.Tenant
{
    [TestClass]
    public class TenantContextTest
    {
        [TestMethod]
        public void TenantContextSettingTest()
        {
            Assert.AreEqual(TenantContextSettings.GetConfig().Enabled, TenantContext.Current.Enabled);
            Assert.AreEqual(TenantContextSettings.GetConfig().DefaultTenantCode, TenantContext.Current.TenantCode);
        }
    }
}
