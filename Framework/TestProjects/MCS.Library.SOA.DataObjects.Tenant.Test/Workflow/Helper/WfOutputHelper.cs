using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class WfOutputHelper
    {
        public static void OutputMainStream(this IWfProcess process)
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

        public static bool OutputEveryActivities(this IWfProcess process)
        {
            StringBuilder strB = new StringBuilder();

            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            bool result = OutputActivityInfoRecursively(process.Descriptor.InitialActivity, elapsedTransitions, strB);

            Console.WriteLine("Every Step: {0}", strB.ToString());

            return result;
        }

        public static void OutputAndAssertEveryActivities(this IWfProcess process)
        {
            Assert.IsTrue(process.OutputEveryActivities(), "流程没有经过结束活动");
        }

        public static void OutputEveryActivities(this IWfProcessDescriptor processDesp)
        {
            StringBuilder strB = new StringBuilder();

            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            OutputActivityInfoRecursively(processDesp.InitialActivity, elapsedTransitions, strB);

            Console.WriteLine("Every Step: {0}", strB.ToString());
        }

        public static void AssertAndOutputMatrixOperators(this IWfMatrixContainer matrix)
        {
            matrix.NullCheck("matrix");

            matrix.Rows.AssertAndOutputMatrixOperators();
        }

        public static void AssertAndOutputMatrixOperators(this SOARolePropertyRowCollection rows)
        {
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    SOARolePropertyRow row = rows[i];
                    Console.Write("Number: {0}", row.RowNumber);

                    foreach (SOARolePropertyValue v in row.Values)
                        Console.Write("; {0}: {1}", v.Column.Name, v.Value);

                    Console.WriteLine();

                    Assert.AreEqual(row.Operator, row.Values.GetValue("Operator", string.Empty));
                }
            }
        }

        /// <summary>
        /// 递归输出每一个活动的信息
        /// </summary>
        /// <param name="actDesp"></param>
        /// <param name="elapsedTransitions"></param>
        /// <param name="strB"></param>
        /// <returns>是否经历过结束点</returns>
        private static bool OutputActivityInfoRecursively(IWfActivityDescriptor actDesp, Dictionary<string, IWfTransitionDescriptor> elapsedTransitions, StringBuilder strB)
        {
            bool result = actDesp.ActivityType == WfActivityType.CompletedActivity;

            if (strB.Length > 0)
                strB.Append("->");

            strB.Append(actDesp.Key);
            strB.Append(":" + actDesp.Name);

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
                    result = result | OutputActivityInfoRecursively(transition.ToActivity, elapsedTransitions, strB);
                }
            }

            return result;
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
