using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    public static class OutputHelper
    {
        public static void Output(this WfClientKeyedDescriptorBase cp)
        {
            if (cp != null)
                Console.WriteLine("Key: {0}, Name: {1}, Enabled: {2}", cp.Key, cp.Name, cp.Enabled);
        }

        public static void Output(this IEnumerable<WfClientKeyedDescriptorBase> cpc)
        {
            if (cpc != null)
                cpc.ForEach(cp => cp.Output());
        }

        public static void Output(this WfKeyedDescriptorBase cp)
        {
            if (cp != null)
                Console.WriteLine("Key: {0}, Name: {1}, Enabled: {2}", cp.Key, cp.Name, cp.Enabled);
        }

        public static void Output(this IEnumerable<WfKeyedDescriptorBase> cpc)
        {
            if (cpc != null)
                cpc.ForEach(cp => cp.Output());
        }
    }
}
