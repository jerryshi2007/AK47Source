using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class WfOutputHelper
    {
        public static void OutputMainStream(IWfProcess process)
        {
            WfMainStreamActivityDescriptorCollection mainStreamActivities = process.GetMainStreamActivities(true);

            StringBuilder strB = new StringBuilder();

            foreach (WfMainStreamActivityDescriptor msActDesp in mainStreamActivities)
            {
                if (strB.Length > 0)
                    strB.Append("->");

                strB.Append(msActDesp.Activity.Key);

                if (msActDesp.Activity.AssociatedActivityKey.IsNotEmpty())
                    strB.AppendFormat("({0})", msActDesp.Activity.AssociatedActivityKey);
            }

            Console.WriteLine("Main Stream: {0}", strB.ToString());
        }

        public static void OutputEveryActivities(IWfProcess process)
        {
            StringBuilder strB = new StringBuilder();

            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            OutputActivityInfoRecursively(process.Descriptor.InitialActivity, elapsedTransitions, strB);

            Console.WriteLine("Every Step: {0}", strB.ToString());
        }

        private static void OutputActivityInfoRecursively(IWfActivityDescriptor actDesp, Dictionary<string, IWfTransitionDescriptor> elapsedTransitions, StringBuilder strB)
        {
            if (strB.Length > 0)
                strB.Append("->");

            strB.Append(actDesp.Key);

            if (actDesp.AssociatedActivityKey.IsNotEmpty())
                strB.AppendFormat("[{0}]", actDesp.AssociatedActivityKey);

            OutputCandidates(actDesp, strB);

            IWfTransitionDescriptor transition = actDesp.ToTransitions.FindElapsedTransition();

            if (transition == null)
                transition = actDesp.ToTransitions.GetAllEnabledTransitions().GetAllCanTransitTransitions().FirstOrDefault();

            if (transition != null)
            {
                if (elapsedTransitions.ContainsKey(transition.Key) == false)
                {
                    elapsedTransitions.Add(transition.Key, transition);
                    OutputActivityInfoRecursively(transition.ToActivity, elapsedTransitions, strB);
                }
            }
        }

        private static void OutputCandidates(IWfActivityDescriptor actDesp, StringBuilder strB)
        {
            if (actDesp.Instance != null)
            {
                StringBuilder strInnerB = new StringBuilder();

                foreach (IUser user in actDesp.Instance.Candidates.ToUsers())
                {
                    if (strInnerB.Length > 0)
                        strInnerB.Append(",");

                    strInnerB.AppendFormat("{0}", user.DisplayName);
                }

                if (strInnerB.Length > 0)
                    strB.AppendFormat("({0})", strInnerB.ToString());
            }
        }
    }
}
