using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WfOperationServices.Test
{
    public static class OutputHelper
    {
        public static void Output(this WfClientProcessInfo processInfo)
        {
            Console.WriteLine("Process ID = {0}", processInfo.ID);
            Console.WriteLine("Process Status = {0}", processInfo.Status);
            Console.WriteLine("UpdateTags = {0}", processInfo.UpdateTag);

            processInfo.Creator.Output("Process Creator");

            processInfo.CurrentActivity.Output("Current Activity");
            processInfo.NextActivities.Output("Next Activities");
            processInfo.MainStreamActivityDescriptors.Output("MainStreamActivities");

            if (processInfo.AuthorizationInfo != null)
            {
                Console.WriteLine("InMoveToMode: {0}, IsProcessAdmin: {1}, IsProcessViewer: {2}, IsInAcl: {3}",
                    processInfo.AuthorizationInfo.InMoveToMode,
                    processInfo.AuthorizationInfo.IsProcessAdmin,
                    processInfo.AuthorizationInfo.IsProcessViewer,
                    processInfo.AuthorizationInfo.IsInAcl);
            }

            if (processInfo.CurrentOpinion != null)
            {
                Console.WriteLine("Opinion: {0}, User: {1}", processInfo.CurrentOpinion.Content, processInfo.CurrentOpinion.IssuePersonName);
            }
        }

        public static void Output(this WfClientUser user)
        {
            Output(user, string.Empty);
        }

        public static void Output(this WfClientUser user, string title)
        {
            if (user != null)
            {
                if (title.IsNotEmpty())
                    Console.Write("{0}: ", title);

                Console.WriteLine("User ID = {1}, User Name = {2}", title, user.ID, user.Name);
            }
        }

        public static void Output(this WfClientActivity activity, string title)
        {
            if (activity != null)
            {
                Console.WriteLine(title);
                Console.WriteLine("Activity ID = {0}, Name = {1}, Status = {2}", activity.ID, activity.Descriptor.Key, activity.Status);

                activity.Assignees.Output("Assignees");
                activity.Candidates.Output("Candidates");
            }
        }

        public static void Output(this WfClientAssigneeCollection assignees, string title)
        {
            if (assignees.Count > 0)
            {
                Console.WriteLine(title);

                foreach (WfClientAssignee assignee in assignees)
                    assignee.User.Output();
            }
        }

        public static void Output(this IEnumerable<WfClientNextActivity> nextActivities, string title)
        {
            if (nextActivities.Any())
            {
                Console.WriteLine(title);

                nextActivities.ForEach(na => na.Output());
            }
        }

        public static void Output(this WfClientNextActivity nextActivity)
        {
            if (nextActivity != null)
            {
                Console.WriteLine("Transition Key = {0}, Activity Key = {1}, Status = {2}",
                    nextActivity.Transition.Key,
                    nextActivity.Activity.DescriptorKey,
                    nextActivity.Activity.Status);
            }
        }

        public static void Output(this IEnumerable<WfClientMainStreamActivityDescriptor> msActDesps, string title)
        {
            if (msActDesps.Any())
            {
                Console.WriteLine(title);

                msActDesps.ForEach(na =>
                    {
                        Console.WriteLine("MS ActKey: {0}, Name: {1}, Status = {2}, Operator= {3}",
                            na.Activity.Key,
                            na.Activity.Name,
                            na.Status,
                            na.Operator != null ? na.Operator.Name : string.Empty);

                        na.Assignees.Output("MS Assignees");
                    });
            }
        }
    }
}
