using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class GlobalParametersTest
    {
        [TestMethod]
        [Description("全局设置的测试")]
        public void GlobalParametersReadTest()
        {
            WfGlobalParameters settings = WfGlobalParameters.LoadProperties("Default");

            settings.Properties.SetValue("AppName", "SinoOcean");

            settings.Update();

            Thread.Sleep(500);

            WfGlobalParameters settingsLoaded = WfGlobalParameters.Default;

            Assert.AreEqual(settings.Properties["AppName"].StringValue,
                settingsLoaded.Properties["AppName"].StringValue);
        }

        [TestMethod]
        [Description("App和Program的参数读取测试")]
        public void GlobalAppProgramParametersReadTest()
        {
            WfGlobalParameters settings = WfGlobalParameters.LoadProperties("ADMINISTRATION", "CONTRACT");

            settings.Properties.SetValue("AppName", "Seagull II");

            settings.Update();

            Thread.Sleep(500);

            WfGlobalParameters settingsLoaded = WfGlobalParameters.GetProperties("ADMINISTRATION", "CONTRACT");

            Assert.AreEqual(settings.Properties["AppName"].StringValue,
                settingsLoaded.Properties["AppName"].StringValue);
        }

        [TestMethod]
        [Description("App和Program的参数类别大小写无关读取测试")]
        public void GlobalAppProgramParametersCaseInsensitiveReadTest()
        {
            WfGlobalParameters settings = WfGlobalParameters.GetProperties("ADMINISTRATION", "CONTRACT");

            settings.Properties.SetValue("AppName", "Seagull II");

            settings.Update();

            Thread.Sleep(500);

            WfGlobalParameters settingsLoaded = WfGlobalParameters.GetProperties("Administration", "Contract");

            //读两遍，仅用于确认Cache
            settingsLoaded = WfGlobalParameters.GetProperties("Administration", "CONTRACT");

            Assert.AreEqual(settings.Properties["AppName"].StringValue,
                settingsLoaded.Properties["AppName"].StringValue);
        }

        [TestMethod]
        [Description("App和Program的递归参数读取测试")]
        public void GlobalAppProgramParametersRecursivelyReadTest()
        {
            WfGlobalParameters defaultSettings = WfGlobalParameters.LoadProperties("Default");

            defaultSettings.Properties.SetValue("AppName", "SinoOcean");
            defaultSettings.Properties.SetValue("ProgName", "DefaultProg");

            defaultSettings.Update();

            WfGlobalParameters appSettings = WfGlobalParameters.LoadProperties("ADMINISTRATION", "CONTRACT");

            appSettings.Properties.SetValue("AppName", string.Empty);
            appSettings.Properties.SetValue("ProgName", "ContractProg");

            appSettings.Update();

            Thread.Sleep(500);

            string pvApp = WfGlobalParameters.GetValueRecursively("ADMINISTRATION", "CONTRACT", "AppName", "DefaultValue");

            Assert.AreEqual("SinoOcean", pvApp);

            string pvProg = WfGlobalParameters.GetValueRecursively("ADMINISTRATION", "CONTRACT", "ProgName", "DefaultValue");

            Assert.AreEqual("ContractProg", pvProg);
        }
    }
}
