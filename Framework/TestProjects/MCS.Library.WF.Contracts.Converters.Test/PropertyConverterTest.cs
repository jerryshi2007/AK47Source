using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.PropertyDefine;
using MCS.Library.WF.Contracts.PropertyDefine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class PropertyConverterTest
    {
        [TestMethod]
        [TestCategory("PropertyConverter")]
        public void StandardClientPropertiesToServer()
        {
            ClientPropertyValueCollection cpvc = new ClientPropertyValueCollection();

            cpvc.AddOrSetValue("Key", "N0");
            cpvc.AddOrSetValue("Name", "Shen Zheng");
            cpvc.AddOrSetValue("Description", "Shen Zheng's c to s property set");
            cpvc.AddOrSetValue("Enabled", true);

            PropertyValueCollection spc = PrepareServerProperties();

            ClientPropertyValueCollectionConverter.Instance.ClientToServer(cpvc, spc);

            OutputServerProperties(spc);

            Assert.AreEqual(cpvc.GetValue("Key", string.Empty), spc.GetValue("Key", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Name", string.Empty), spc.GetValue("Name", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Description", string.Empty), spc.GetValue("Description", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Enabled", false), spc.GetValue("Enabled", false));
        }

        [TestMethod]
        [TestCategory("PropertyConverter")]
        public void StandardServerPropertiesToClient()
        {
            PropertyValueCollection spc = PrepareServerProperties();

            spc.SetValue("Key", "N0");
            spc.SetValue("Name", "Shen Zheng");
            spc.SetValue("Description", "Shen Zheng's s to c property set");
            spc.SetValue("Enabled", true);

            ClientPropertyValueCollection cpvc = new ClientPropertyValueCollection();

            ClientPropertyValueCollectionConverter.Instance.ServerToClient(spc, cpvc);

            OutputClientProperties(cpvc);

            Assert.AreEqual(spc.GetValue("Key", string.Empty), cpvc.GetValue("Key", string.Empty));
            Assert.AreEqual(spc.GetValue("Name", string.Empty), cpvc.GetValue("Name", string.Empty));
            Assert.AreEqual(spc.GetValue("Description", string.Empty), cpvc.GetValue("Description", string.Empty));
            Assert.AreEqual(spc.GetValue("Enabled", false), cpvc.GetValue("Enabled", false));
        }

        [TestMethod]
        [Description("客户端会传递Server端不存在的属性")]
        public void ClientPropertiesToIgnoredServerProperties()
        {
            ClientPropertyValueCollection cpvc = new ClientPropertyValueCollection();

            cpvc.AddOrSetValue("Key", "N0");
            cpvc.AddOrSetValue("Name", "Shen Zheng");
            cpvc.AddOrSetValue("Description", "Shen Zheng's c to s property set");
            cpvc.AddOrSetValue("Enabled", true);
            cpvc.AddOrSetValue("Ignored", true);

            PropertyValueCollection spc = PrepareServerProperties();

            ClientPropertyValueCollectionConverter.Instance.ClientToServer(cpvc, spc);

            OutputServerProperties(spc);

            Assert.AreEqual(cpvc.GetValue("Key", string.Empty), spc.GetValue("Key", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Name", string.Empty), spc.GetValue("Name", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Description", string.Empty), spc.GetValue("Description", string.Empty));
            Assert.AreEqual(cpvc.GetValue("Enabled", false), spc.GetValue("Enabled", false));

            Assert.IsFalse(spc.ContainsKey("Ignored"));
        }

        /// <summary>
        /// 初始化一个基本的属性集合，包括Key、Name、Description、Enabled几个基本属性
        /// </summary>
        /// <returns></returns>
        private static PropertyValueCollection PrepareServerProperties()
        {
            PropertyValueCollection properties = new PropertyValueCollection();

            PropertyDefineCollection definitions = new PropertyDefineCollection();

            definitions.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["AllDescriptorProperties"]);

            properties.InitFromPropertyDefineCollection(definitions);

            return properties;
        }

        private static void OutputServerProperties(PropertyValueCollection spc)
        {
            spc.ForEach(pv => Console.WriteLine("{0}: {1}", pv.Definition.Name, pv.StringValue));
        }

        private static void OutputClientProperties(ClientPropertyValueCollection cpc)
        {
            cpc.ForEach(pv => Console.WriteLine("{0}: {1}", pv.Key, pv.StringValue));
        }
    }
}
