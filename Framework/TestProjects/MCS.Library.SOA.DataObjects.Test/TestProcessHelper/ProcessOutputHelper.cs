using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test
{
    /// <summary>
    /// 输出流程信息的帮助类
    /// </summary>
    public static class ProcessOutputHelper
    {
        /// <summary>
        /// 输出主线流程活动
        /// </summary>
        /// <param name="msActivities"></param>
        public static void Output(this WfMainStreamActivityDescriptorCollection msActivities, string name)
        {
            StringBuilder strB = new StringBuilder();

            if (name.IsNotEmpty())
                strB.AppendLine(name);

            foreach (WfMainStreamActivityDescriptor msActDesp in msActivities)
            {
                if (strB.Length > 0)
                    strB.Append("->");

                strB.Append(msActDesp.Activity.Key);

                if (msActDesp.Activity.AssociatedActivityKey.IsNotEmpty())
                    strB.AppendFormat("({0})", msActDesp.Activity.AssociatedActivityKey);
            }

            Console.WriteLine("Main Stream: {0}", strB.ToString());
        }

        public static void Output(this IWfProcessDescriptor processDesp)
        {
            Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

            if (processDesp != null)
            {
                OutputActivities(null, processDesp.InitialActivity, elapsedTransitions);
            }
        }

        private static void OutputActivities(IWfTransitionDescriptor transition, IWfActivityDescriptor targetActDesp, Dictionary<string, IWfTransitionDescriptor> elapsedTransitions)
        {
            if (transition != null)
            {
                //防止死循环
                if (elapsedTransitions.ContainsKey(transition.Key))
                    return;

                elapsedTransitions.Add(transition.Key, transition);
            }

            if (transition != null)
            {
                Console.Write("从 ");
                OutputActivity(transition.FromActivity);

                Console.Write(" 经过 {0} ", transition.Key);
            }

            Console.Write("到 ");
            OutputActivity(targetActDesp);

            Console.WriteLine();

            foreach (IWfTransitionDescriptor t in targetActDesp.ToTransitions)
            {
                OutputActivities(t, t.ToActivity, elapsedTransitions);
            }
        }

        private static void OutputActivity(IWfActivityDescriptor actDesp)
        {
            Console.Write(actDesp.Key);

            if (actDesp.AssociatedActivityKey.IsNotEmpty())
                Console.Write("({0})", actDesp.AssociatedActivityKey);
        }
    }
}