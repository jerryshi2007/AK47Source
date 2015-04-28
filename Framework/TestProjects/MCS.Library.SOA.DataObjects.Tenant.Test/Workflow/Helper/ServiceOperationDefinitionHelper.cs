using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class ServiceOperationDefinitionHelper
    {
        public static void AreEqual(this WfServiceOperationDefinition actual, WfServiceOperationDefinitionConfigurationElement element)
        {
            AssertStringEqual(actual.OperationName, element.OperationName);
            AssertStringEqual(actual.RtnXmlStoreParamName, element.ReturnParamName);
            AssertStringEqual(actual.AddressDef.Key, element.AddressKey);

            actual.Params.AreEqual(element.Parameters);
        }

        public static void AreEqual(this WfNetworkCredential actual, LogOnIdentity identity)
        {
            if (actual == null && identity == null)
                return;

            Assert.AreEqual(actual.LogOnNameWithoutDomain, identity.LogOnNameWithoutDomain);
            Assert.AreEqual(actual.Password, identity.Password);
            Assert.AreEqual(actual.Domain, identity.Domain);
        }

        public static void AreEqual(this WfServiceAddressDefinition actual, WfServiceAddressDefinitionConfigurationElement element)
        {
            if (actual == null && element == null)
                return;

            Assert.AreEqual(actual.RequestMethod, element.RequestMethod);
            Assert.AreEqual(actual.ContentType, element.ContentType);

            AssertStringEqual(actual.Address, element.Uri.ToUriString());

            actual.Credential.AreEqual(element.Identity);
        }

        public static void AreEqual(this WfServiceOperationParameterCollection actual, WfServiceOperationParameterConfigurationElementCollection element)
        {
            Assert.AreEqual(actual.Count, element.Count);

            foreach (WfServiceOperationParameter itemActual in actual)
            {
                WfServiceOperationParameterConfigurationElement itemElement = element[itemActual.Name];

                Assert.IsNotNull(itemElement);

                itemActual.AreEqual(itemElement);
            }
        }

        public static void AreEqual(this WfServiceOperationParameter actual, WfServiceOperationParameterConfigurationElement element)
        {
            AssertStringEqual(actual.Name, element.Name);
            Assert.AreEqual(actual.Type, element.Type);
            AssertStringEqual((string)actual.Value, element.Value);
        }

        private static void AssertStringEqual(string expected, string actual)
        {
            if (expected.IsNotEmpty() || actual.IsNotEmpty())
                Assert.AreEqual(expected, actual);
        }
    }
}
