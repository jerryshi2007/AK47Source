using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public static class AssertClientToClientHelper
    {
        public static void AssertCollection(this IEnumerable<WfClientKeyedDescriptorBase> cvc, IEnumerable<WfClientKeyedDescriptorBase> svc)
        {
            Assert.AreEqual(cvc.Count(), svc.Count());

            foreach (WfClientKeyedDescriptorBase c in cvc)
            {
                WfClientKeyedDescriptorBase serverObj = svc.Where(s => s.Key == c.Key).First();

                c.AreSame(serverObj);
            }
        }

        public static void AssertActivityDescriptor(this WfClientActivityDescriptor cad, WfClientActivityDescriptor sad)
        {
            cad.AreSame(sad);
            Assert.AreEqual(cad.ActivityType, sad.ActivityType);

            cad.Condition.AreSame(sad.Condition);
            cad.Variables.AssertCollection(sad.Variables);
            cad.Resources.AssertResources(sad.Resources);

            cad.ToTransitions.AssertCollection(sad.ToTransitions);
            cad.ToTransitions.ForEach(ct => ct.IsValid());
            sad.ToTransitions.ForEach(st => st.IsValid());

            cad.GetFromTransitions().AssertCollection(sad.GetFromTransitions());
            cad.GetFromTransitions().ForEach(ct => ct.IsValid());
            sad.GetFromTransitions().ForEach(st => st.IsValid());

            cad.RelativeLinks.AssertRelativeLinks(sad.RelativeLinks);
        }

        public static void AssertActivityDescriptorCollection(this WfClientActivityDescriptorCollection cadc, WfClientActivityDescriptorCollection sadc)
        {
            Assert.AreEqual(cadc.Count, sadc.Count);

            for (int i = 0; i < cadc.Count; i++)
                cadc[i].AssertActivityDescriptor(sadc[i]);
        }

        public static void AreSame(this WfClientProcessCurrentInfo expected, WfClientProcessCurrentInfo actual)
        {
            AssertStringEqual(expected.InstanceID, actual.InstanceID);
            AssertStringEqual(expected.ResourceID, actual.ResourceID);

            AssertStringEqual(expected.ApplicationName, actual.ApplicationName);
            AssertStringEqual(expected.ProgramName, actual.ProgramName);
            AssertStringEqual(expected.ProcessName, actual.ProcessName);
            AssertStringEqual(expected.DescriptorKey, actual.DescriptorKey);
            AssertStringEqual(expected.OwnerActivityID, actual.OwnerActivityID);
            AssertStringEqual(expected.OwnerTemplateKey, actual.OwnerTemplateKey);
            AssertStringEqual(expected.CurrentActivityID, actual.CurrentActivityID);

            Assert.AreEqual(expected.Sequence, actual.Sequence);
            Assert.AreEqual(expected.Committed, actual.Committed);

            Assert.AreEqual(expected.CreateTime, actual.CreateTime);
            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);

            expected.Creator.AreSame(actual.Creator);
            expected.Department.AreSame(actual.Department);

            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.UpdateTag, actual.UpdateTag);
        }

        public static void AssertProcessDescriptor(this WfClientProcessDescriptor cpd, WfClientProcessDescriptor spd)
        {
            cpd.AreSame(spd);

            cpd.Activities.AssertActivityDescriptorCollection(spd.Activities);

            if (cpd.InitialActivity != null)
                Assert.AreEqual(cpd.InitialActivity.Key, spd.InitialActivity.Key);

            if (cpd.CompletedActivity != null)
                Assert.AreEqual(cpd.CompletedActivity.Key, spd.CompletedActivity.Key);

            cpd.RelativeLinks.AssertRelativeLinks(spd.RelativeLinks);
        }

        public static void AreSame(this WfClientBranchProcessTemplateDescriptor expected, WfClientBranchProcessTemplateDescriptor actual)
        {
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.BlockingType, actual.BlockingType);
                Assert.AreEqual(expected.SubProcessApprovalMode, actual.SubProcessApprovalMode);
                Assert.AreEqual(expected.ExecuteSequence, actual.ExecuteSequence);
                Assert.AreEqual(expected.BranchProcessKey, actual.BranchProcessKey);
                Assert.AreEqual(expected.DefaultProcessName, actual.DefaultProcessName);
                Assert.AreEqual(expected.DefaultTaskTitle, actual.DefaultTaskTitle);

                expected.Condition.AreSame(actual.Condition);

                expected.Resources.AssertResources(actual.Resources);
                expected.CancelSubProcessNotifier.AssertResources(actual.CancelSubProcessNotifier);
                expected.RelativeLinks.AssertRelativeLinks(actual.RelativeLinks);
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientTransferParams expected, WfClientTransferParams actual)
        {
            if (expected != null && actual != null)
            {
                AssertStringEqual(expected.NextActivityDescriptorKey, actual.NextActivityDescriptorKey);
                AssertStringEqual(expected.FromTransitionDescriptorKey, actual.FromTransitionDescriptorKey);

                expected.Operator.AreSame(actual.Operator);
                expected.Assignees.AreSame(actual.Assignees);

                expected.BranchTransferParams.AreSame(actual.BranchTransferParams);
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientBranchProcessTransferParamsCollection expected, WfClientBranchProcessTransferParamsCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i]);
        }

        public static void AreSame(this WfClientBranchProcessTransferParams expected, WfClientBranchProcessTransferParams actual)
        {
            expected.Template.AreSame(actual.Template);

            expected.BranchParams.AreSame(actual.BranchParams);
        }

        public static void AreSame(this WfClientBranchProcessStartupParamsCollection expected, WfClientBranchProcessStartupParamsCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i]);
        }

        public static void AreSame(this WfClientBranchProcessStartupParams expected, WfClientBranchProcessStartupParams actual)
        {
            if (expected != null && actual != null)
            {
                AssertStringEqual(expected.DefaultTaskTitle, actual.DefaultTaskTitle);
                AssertStringEqual(expected.ResourceID, actual.ResourceID);

                expected.Assignees.AreSame(actual.Assignees);
                expected.Department.AreSame(actual.Department);

                expected.ApplicationRuntimeParameters.AreSame(actual.ApplicationRuntimeParameters);
                expected.RelativeParams.AreSame(actual.RelativeParams);
                expected.StartupContext = actual.StartupContext;
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientConditionDescriptor c, WfClientConditionDescriptor s)
        {
            Assert.IsNotNull(c);
            Assert.IsNotNull(s);
            AssertStringEqual(c.Expression, s.Expression);
        }

        public static void AreSame(this WfClientKeyedDescriptorBase c, WfClientKeyedDescriptorBase s)
        {
            Assert.IsNotNull(c);
            Assert.IsNotNull(s);

            Assert.AreEqual(c.Key, s.Key);
            Assert.AreEqual(c.Name, s.Name);
            AssertStringEqual(c.Description, s.Description);
        }

        public static void AssertResources(this WfClientResourceDescriptorCollection cc, WfClientResourceDescriptorCollection sc)
        {
            Assert.AreEqual(cc.Count, sc.Count);
        }

        public static void AssertRelativeLinks(this WfClientRelativeLinkDescriptorCollection expected, WfClientRelativeLinkDescriptorCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                expected[i].AreSame(actual[i]);

                Assert.AreEqual(expected[i].Category, actual[i].Category);
                Assert.AreEqual(expected[i].Url, actual[i].Url);
            }
        }

        public static void AreSame(this IDictionary<string, object> expected, IDictionary<string, object> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            foreach (KeyValuePair<string, object> kp in expected)
                Assert.AreEqual(kp.Value, actual[kp.Key]);
        }

        public static void AssertContains(this IDictionary<string, object> container, IDictionary<string, object> member)
        {
            if (member.Count > 0)
            {
                foreach (KeyValuePair<string, object> kp in member)
                {
                    Assert.IsTrue(container.ContainsKey(kp.Key), string.Format("必须包含key:{0}", kp.Key));

                    Assert.AreEqual(member[kp.Key], container[kp.Key]);
                }
            }
        }

        public static void AreSame(this NameValueCollection expected, NameValueCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            foreach (string key in expected.Keys)
                Assert.AreEqual(expected[key], actual[key]);
        }

        public static void AreSame(this WfClientProcessStartupParams expected, WfClientProcessStartupParams actual)
        {
            Assert.AreEqual(expected.ProcessDescriptorKey, actual.ProcessDescriptorKey);
            Assert.AreEqual(expected.AutoCommit, actual.AutoCommit);
            Assert.AreEqual(expected.AutoStartInitialActivity, actual.AutoStartInitialActivity);
            Assert.AreEqual(expected.CheckStartProcessUserPermission, actual.CheckStartProcessUserPermission);
            Assert.AreEqual(expected.DefaultTaskTitle, actual.DefaultTaskTitle);
            Assert.AreEqual(expected.RelativeID, actual.RelativeID);
            Assert.AreEqual(expected.RelativeURL, actual.RelativeURL);
            Assert.AreEqual(expected.ResourceID, actual.ResourceID);
            Assert.AreEqual(expected.RuntimeProcessName, actual.RuntimeProcessName);

            expected.Assignees.AreSame(actual.Assignees);
            expected.Creator.AreSame(actual.Creator);
            expected.Department.AreSame(actual.Department);

            expected.ApplicationRuntimeParameters.AreSame(actual.ApplicationRuntimeParameters);
            expected.ProcessContext.AreSame(actual.ProcessContext);
        }

        public static void AreSame(this WfClientAssigneeCollection expected, WfClientAssigneeCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i], true);
        }

        public static void AreSame(this WfClientAssigneeCollection expected, WfClientAssigneeCollection actual, bool checkUrl)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i], checkUrl);
        }

        public static void AreSame(this WfClientAssignee expected, WfClientAssignee actual)
        {
            AreSame(expected, actual, true);
        }

        public static void AreSame(this WfClientAssignee expected, WfClientAssignee actual, bool checkUrl)
        {
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.AssigneeType, actual.AssigneeType);

                if (checkUrl)
                    AssertStringEqual(expected.Url, actual.Url);

                Assert.IsTrue(expected.User.AreSame(actual.User));
                Assert.IsTrue(expected.Delegator.AreSame(actual.Delegator));
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientOpinion expected, WfClientOpinion actual)
        {
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.ResourceID, actual.ResourceID);
            Assert.AreEqual(expected.ActivityID, actual.ActivityID);
            Assert.AreEqual(expected.ProcessID, actual.ProcessID);
            Assert.AreEqual(expected.LevelName, actual.LevelName);
            Assert.AreEqual(expected.LevelDesp, actual.LevelDesp);
            Assert.AreEqual(expected.OpinionType, actual.OpinionType);
            Assert.AreEqual(expected.IssueTime, actual.IssueTime);
            Assert.AreEqual(expected.AppendTime, actual.AppendTime);

            Assert.AreEqual(expected.IssuePersonID, actual.IssuePersonID);
            Assert.AreEqual(expected.IssuePersonName, actual.IssuePersonName);

            Assert.AreEqual(expected.AppendPersonID, actual.AppendPersonID);
            Assert.AreEqual(expected.AppendPersonName, actual.AppendPersonName);

            Assert.AreEqual(expected.ExtraData, actual.ExtraData);
        }

        public static void AreSame(this WfClientActivityMatrixResourceDescriptor expected, WfClientActivityMatrixResourceDescriptor actual)
        {
            AreSame(expected.PropertyDefinitions, actual.PropertyDefinitions);
            AreSame(expected.Rows, actual.Rows);
        }

        public static void AreSame(this WfClientApprovalMatrix expected, WfClientApprovalMatrix actual)
        {
            AssertStringEqual(expected.ID, actual.ID);

            AreSame(expected.PropertyDefinitions, actual.PropertyDefinitions);
            AreSame(expected.Rows, actual.Rows);
        }

        public static void AreSame(this WfClientRolePropertyDefinitionCollection expected, WfClientRolePropertyDefinitionCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                AreSame(expected[i], actual[i]);
            }
        }

        public static void AreSame(this WfClientRolePropertyDefinition expected, WfClientRolePropertyDefinition actual)
        {
            AssertStringEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.DataType, actual.DataType);
            AssertStringEqual(expected.Caption, actual.Caption);
            Assert.AreEqual(expected.SortOrder, actual.SortOrder);
            Assert.AreEqual(expected.DefaultValue, actual.DefaultValue);
        }

        public static void AreSame(this WfClientRolePropertyRowCollection expected, WfClientRolePropertyRowCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                AreSame(expected[i], actual[i]);
            }
        }

        public static void AreSame(this WfClientRolePropertyRow expected, WfClientRolePropertyRow actual)
        {
            AssertStringEqual(expected.Operator, actual.Operator);
            Assert.AreEqual(expected.OperatorType, actual.OperatorType);
            Assert.AreEqual(expected.RowNumber, actual.RowNumber);

            foreach (WfClientRolePropertyValue pve in expected.Values)
            {
                string actualValue = actual.Values.GetValue(pve.Column.Name, string.Empty);

                AssertStringEqual(pve.Value, actualValue);
            }
        }

        public static void AreSame(this WfCreateClientDynamicProcessParams expected, WfCreateClientDynamicProcessParams actual)
        {
            ((WfClientKeyedDescriptorBase)expected).AreSame((WfClientKeyedDescriptorBase)actual);
            expected.ActivityMatrix.AreSame(actual.ActivityMatrix);
        }


        public static void AreSame(this WfClientDelegation expected, WfClientDelegation actual)
        {
            Assert.AreEqual(expected.SourceUserID, actual.SourceUserID);
            Assert.AreEqual(expected.SourceUserName, actual.SourceUserName);
            Assert.AreEqual(expected.DestinationUserID, actual.DestinationUserID);
            Assert.AreEqual(expected.DestinationUserName, actual.DestinationUserName);

            Assert.AreEqual(expected.StartTime.ToLocalTime(), actual.StartTime);
            Assert.AreEqual(expected.EndTime.ToLocalTime(), actual.EndTime);

            Assert.AreEqual(expected.Enabled, actual.Enabled);

            Assert.AreEqual(expected.ApplicationName, actual.ApplicationName);
            Assert.AreEqual(expected.ProgramName, actual.ProgramName);
            Assert.AreEqual(expected.TenantCode, actual.TenantCode);
        }


        public static void AreSame(this WfClientDelegationCollection expected, WfClientDelegationCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                expected[i].AreSame(actual[i]);
            }
        }

        public static void AreSame(this WfClientProcessQueryCondition expected, WfClientProcessQueryCondition actual)
        {
            AssertStringEqual(expected.ApplicationName, actual.ApplicationName);
            AssertStringEqual(expected.ProcessName, actual.ProcessName);
            AssertStringEqual(expected.AssigneesUserName, actual.AssigneesUserName);
            AssertStringEqual(expected.DepartmentName, actual.DepartmentName);
            AssertStringEqual(expected.ProcessStatus, actual.ProcessStatus);

            AssertDateTimeEqual(expected.BeginStartTime, actual.BeginStartTime);
            AssertDateTimeEqual(expected.EndStartTime, actual.EndStartTime);

            Assert.AreEqual(expected.AssigneesSelectType, actual.AssigneesSelectType);
            Assert.AreEqual(expected.AssigneeExceptionFilterType, actual.AssigneeExceptionFilterType);

            Assert.AreEqual(expected.CurrentAssignees.Count, actual.CurrentAssignees.Count);

            for (int i = 0; i < expected.CurrentAssignees.Count; i++)
            {
                expected.CurrentAssignees[i].AreSame(actual.CurrentAssignees[i]);
            }
        }

        private static void AssertDateTimeEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Year, actual.Year);
            Assert.AreEqual(expected.Month, actual.Month);
            Assert.AreEqual(expected.Day, actual.Day);

            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
            Assert.AreEqual(expected.Second, actual.Second);

            Assert.AreEqual(expected.Millisecond, actual.Millisecond);
        }

        private static void AssertStringEqual(string expected, string actual)
        {
            if (expected.IsNotEmpty() || actual.IsNotEmpty())
                Assert.AreEqual(expected, actual);
        }
    }
}
