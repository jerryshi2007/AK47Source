using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class VariableConverterTest
    {
        [TestMethod]
        public void StandardClientVariablesToServer()
        {
            WfClientVariableDescriptorCollection cvc = new WfClientVariableDescriptorCollection();

            cvc.AddOrSetValue("NameVariable", "Shen Zheng's variable");
            cvc["NameVariable"].Name = "Shen Zheng";
            cvc.AddOrSetValue("Enabled", "True", WfClientVariableDataType.Boolean);

            WfVariableDescriptorCollection svc = new WfVariableDescriptorCollection();

            WfClientVariableDescriptorCollectionConverter.Instance.ClientToServer(cvc, svc);

            cvc.Output();
            svc.Output();

            cvc.AssertCollection(svc);
        }

        [TestMethod]
        public void StandardServerVariablesToClient()
        {
            WfVariableDescriptorCollection svc = new WfVariableDescriptorCollection();

            svc.SetValue("NameVariable", "Shen Zheng's variable");
            svc["NameVariable"].Name = "Shen Zheng";
            svc.SetValue("Enabled", "True", DataType.Boolean);

            WfClientVariableDescriptorCollection cvc = new WfClientVariableDescriptorCollection();

            WfClientVariableDescriptorCollectionConverter.Instance.ServerToClient(svc, cvc);

            cvc.Output();
            svc.Output();

            cvc.AssertCollection(svc);
        }
    }
}
