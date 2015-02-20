using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public static class AssertClientToServerHelper
    {
        public static void AssertCollection(this IEnumerable<WfClientKeyedDescriptorBase> cvc, IEnumerable<IWfKeyedDescriptor> svc)
        {
            Assert.AreEqual(cvc.Count(), svc.Count());

            foreach (WfClientKeyedDescriptorBase c in cvc)
            {
                IWfKeyedDescriptor serverObj = svc.Where(s => s.Key == c.Key).First();

                c.AreSame(serverObj);
            }
        }

        public static void AssertBranchProcessTemplateCollection(this IEnumerable<WfClientBranchProcessTemplateDescriptor> cvc, IEnumerable<IWfBranchProcessTemplateDescriptor> svc)
        {
            Assert.AreEqual(cvc.Count(), svc.Count());

            foreach (WfClientBranchProcessTemplateDescriptor c in cvc)
            {
                IWfBranchProcessTemplateDescriptor sv = svc.Where(s => s.Key == c.Key).First();

                c.AreSame(sv);
            }
        }

        public static void AssertActivityDescriptor(this WfClientActivityDescriptor cad, IWfActivityDescriptor sad)
        {
            cad.AreSame(sad);
            Assert.AreEqual(cad.ActivityType, sad.ActivityType.ToClientActivityType());

            cad.Condition.AreSame(sad.Condition);
            cad.Variables.AssertCollection(sad.Variables);
            cad.Resources.AssertResources(sad.Resources);
            cad.BranchProcessTemplates.AssertBranchProcessTemplateCollection(sad.BranchProcessTemplates);

            cad.ToTransitions.AssertCollection(sad.ToTransitions);
            cad.ToTransitions.ForEach(ct => ct.IsValid());
            sad.ToTransitions.ForEach(st => st.IsValid());

            cad.GetFromTransitions().AssertCollection(sad.FromTransitions);
            cad.GetFromTransitions().ForEach(ct => ct.IsValid());
            sad.FromTransitions.ForEach(st => st.IsValid());

            cad.RelativeLinks.AssertRelativeLinks(sad.RelativeLinks);
        }

        public static void IsValid(this WfClientTransitionDescriptor ct)
        {
            Assert.IsNotNull(ct);

            Assert.IsNotNull(ct.FromActivity);
            Assert.IsNotNull(ct.ToActivity);
        }

        public static void IsValid(this IWfTransitionDescriptor st)
        {
            Assert.IsNotNull(st);

            Assert.IsNotNull(st.FromActivity);
            Assert.IsNotNull(st.ToActivity);
        }

        public static void AssertActivityDescriptorCollection(this WfClientActivityDescriptorCollection cadc, WfActivityDescriptorCollection sadc)
        {
            Assert.AreEqual(cadc.Count, sadc.Count);

            for (int i = 0; i < cadc.Count; i++)
                cadc[i].AssertActivityDescriptor(sadc[i]);
        }

        public static void AreSame(this WfClientProcessCurrentInfo client, WfProcessCurrentInfo server)
        {
            AssertStringEqual(client.InstanceID, server.InstanceID);
            AssertStringEqual(client.ResourceID, server.ResourceID);

            AssertStringEqual(client.ApplicationName, server.ApplicationName);
            AssertStringEqual(client.ProgramName, server.ProgramName);
            AssertStringEqual(client.ProcessName, server.ProcessName);
            AssertStringEqual(client.DescriptorKey, server.DescriptorKey);
            AssertStringEqual(client.OwnerActivityID, server.OwnerActivityID);
            AssertStringEqual(client.OwnerTemplateKey, server.OwnerTemplateKey);
            AssertStringEqual(client.CurrentActivityID, server.CurrentActivityID);

            Assert.AreEqual(client.Sequence, server.Sequence);
            Assert.AreEqual(client.Committed, server.Committed);

            Assert.AreEqual(client.CreateTime, server.CreateTime);
            Assert.AreEqual(client.StartTime, server.StartTime);
            Assert.AreEqual(client.EndTime, server.EndTime);

            client.Creator.AreSame(server.Creator);
            client.Department.AreSame(server.Department);

            Assert.AreEqual(client.Status, server.Status.ToClientProcessStatus());
            Assert.AreEqual(client.UpdateTag, server.UpdateTag);
        }

        public static void AssertProcessDescriptor(this WfClientProcessDescriptor cpd, WfProcessDescriptor spd)
        {
            cpd.AreSame(spd);

            cpd.Activities.AssertActivityDescriptorCollection(spd.Activities);

            if (cpd.InitialActivity != null)
                Assert.AreEqual(cpd.InitialActivity.Key, spd.InitialActivity.Key);

            if (cpd.CompletedActivity != null)
                Assert.AreEqual(cpd.CompletedActivity.Key, spd.CompletedActivity.Key);

            cpd.RelativeLinks.AssertRelativeLinks(spd.RelativeLinks);
        }

        public static void AreSame(this WfClientConditionDescriptor c, WfConditionDescriptor s)
        {
            Assert.IsNotNull(c);
            Assert.IsNotNull(s);
            Assert.AreEqual(c.Expression, s.Expression);
        }

        public static void AreSame(this WfClientKeyedDescriptorBase c, IWfKeyedDescriptor s)
        {
            Assert.IsNotNull(c);
            Assert.IsNotNull(s);

            Assert.AreEqual(c.Key, s.Key);
            Assert.AreEqual(c.Name, s.Name);
            Assert.AreEqual(c.Description, s.Description);
        }

        public static void AssertResources(this WfClientResourceDescriptorCollection cc, WfResourceDescriptorCollection sc)
        {
            Assert.AreEqual(cc.Count, sc.Count);
        }

        public static void AssertRelativeLinks(this WfClientRelativeLinkDescriptorCollection expected, WfRelativeLinkDescriptorCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                expected[i].AreSame(actual[i]);

                Assert.AreEqual(expected[i].Category, actual[i].Category);
                Assert.AreEqual(expected[i].Url, actual[i].Url);
            }
        }

        /// <summary>
        /// 判断两个对象是否一致。主要是比较ID。如果都是null，也返回true
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool AreSame(this WfClientOguObjectBase src, IOguObject target)
        {
            bool result = false;

            if (src == null && target == null)
                result = true;
            else
                if (src != null && target != null)
                    result = string.Compare(src.ID, target.ID, true) == 0;

            return result;
        }

        public static void AreSame(this WfClientProcessStartupParams expected, WfProcessStartupParams actual)
        {
            //Assert.AreEqual(expected.ProcessDescriptorKey, actual.ProcessDescriptorKey);
            Assert.AreEqual(expected.AutoCommit, actual.AutoCommit);
            Assert.AreEqual(expected.AutoStartInitialActivity, actual.AutoStartInitialActivity);
            Assert.AreEqual(expected.CheckStartProcessUserPermission, actual.CheckStartProcessUserPermission);
            Assert.AreEqual(expected.DefaultTaskTitle, actual.DefaultTaskTitle);
            Assert.AreEqual(expected.DefaultUrl, actual.DefaultUrl);
            Assert.AreEqual(expected.RelativeID, actual.RelativeID);
            Assert.AreEqual(expected.RelativeURL, actual.RelativeURL);
            Assert.AreEqual(expected.ResourceID, actual.ResourceID);
            Assert.AreEqual(expected.RuntimeProcessName, actual.RuntimeProcessName);

            expected.Assignees.AreSame(actual.Assignees);
            expected.Creator.AreSame(actual.Creator);
            expected.Department.AreSame(actual.Department);

            expected.ApplicationRuntimeParameters.AreSame(actual.ApplicationRuntimeParameters);
        }

        public static void AreSame(this WfClientAssignee expected, WfAssignee actual)
        {
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.AssigneeType, actual.AssigneeType.ToClientAssigneeType());
                Assert.AreEqual(expected.Url, actual.Url);
                Assert.IsTrue(expected.User.AreSame(actual.User));
                Assert.IsTrue(expected.Delegator.AreSame(actual.Delegator));
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientBranchProcessTemplateDescriptor expected, IWfBranchProcessTemplateDescriptor actual)
        {
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.BlockingType, actual.BlockingType.ToClientBranchProcessBlockingType());
                Assert.AreEqual(expected.SubProcessApprovalMode, actual.SubProcessApprovalMode.ToClientSubProcessApprovalMode());
                Assert.AreEqual(expected.ExecuteSequence, actual.ExecuteSequence.ToClientBranchProcessExecuteSequence());
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

        public static void AreSame(this WfClientAssigneeCollection expected, WfAssigneeCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i]);
        }

        public static void AreSame(this WfClientOpinion expected, GenericOpinion actual)
        {
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.ResourceID, actual.ResourceID);
            Assert.AreEqual(expected.ActivityID, actual.ActivityID);
            Assert.AreEqual(expected.ProcessID, actual.ProcessID);
            Assert.AreEqual(expected.LevelName, actual.LevelName);
            Assert.AreEqual(expected.LevelDesp, actual.LevelDesp);
            Assert.AreEqual(expected.OpinionType, actual.OpinionType);
            Assert.AreEqual(expected.IssueTime, actual.IssueDatetime);
            Assert.AreEqual(expected.AppendTime, actual.AppendDatetime);

            Assert.AreEqual(expected.IssuePersonID, actual.IssuePerson.ID);
            Assert.AreEqual(expected.IssuePersonName, actual.IssuePerson.DisplayName);

            Assert.AreEqual(expected.AppendPersonID, actual.AppendPerson.ID);
            Assert.AreEqual(expected.AppendPersonName, actual.AppendPerson.DisplayName);

            Assert.AreEqual(expected.ExtraData, actual.RawExtData);
        }

        public static void AreSame(this WfClientTransferParams expected, WfTransferParams actual)
        {
            if (expected != null && actual != null)
            {
                expected.Operator.AreSame(actual.Operator);
                expected.Assignees.AreSame(actual.Assignees);

                expected.BranchTransferParams.AreSame(actual.BranchTransferParams);
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientBranchProcessTransferParamsCollection expected, WfBranchProcessTransferParamsCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i]);
        }

        public static void AreSame(this WfClientBranchProcessTransferParams expected, WfBranchProcessTransferParams actual)
        {
            expected.Template.AreSame(actual.Template);

            expected.BranchParams.AreSame(actual.BranchParams);
        }

        public static void AreSame(this WfClientBranchProcessStartupParamsCollection expected, WfBranchProcessStartupParamsCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
                expected[i].AreSame(actual[i]);
        }

        public static void AreSame(this WfClientBranchProcessStartupParams expected, WfBranchProcessStartupParams actual)
        {
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.DefaultTaskTitle, actual.DefaultTaskTitle);
                Assert.AreEqual(expected.ResourceID, actual.ResourceID);

                expected.Assignees.AreSame(actual.Assignees);
                expected.Department.AreSame(actual.Department);

                expected.ApplicationRuntimeParameters.AreSame(actual.ApplicationRuntimeParameters);
                expected.RelativeParams.AreSame(actual.RelativeParams);
                expected.StartupContext = actual.StartupContext;
            }
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreSame(this WfClientActivityMatrixResourceDescriptor expected, WfActivityMatrixResourceDescriptor actual)
        {
            AreSame(expected.PropertyDefinitions, actual.PropertyDefinitions);

            AreSame(expected.Rows, actual.Rows);
        }

        public static void AreSame(this WfClientRolePropertyDefinitionCollection expected, SOARolePropertyDefinitionCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                AreSame(expected[i], actual[i]);
            }
        }

        public static void AreSame(this WfClientRolePropertyDefinition expected, SOARolePropertyDefinition actual)
        {
            AssertStringEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.DataType, actual.DataType);
            AssertStringEqual(expected.Caption, actual.Caption);
            Assert.AreEqual(expected.SortOrder, actual.SortOrder);
            Assert.AreEqual(expected.DefaultValue, actual.DefaultValue);
        }

        public static void AreSame(this WfClientRolePropertyRowCollection expected, SOARolePropertyRowCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                AreSame(expected[i], actual[i]);
            }
        }

        public static void AreSame(this WfClientRolePropertyRow expected, SOARolePropertyRow actual)
        {
            AssertStringEqual(expected.Operator, actual.Operator);
            Assert.AreEqual(expected.OperatorType, actual.OperatorType.ToClientRoleOperatorType());
            Assert.AreEqual(expected.RowNumber, actual.RowNumber);

            foreach (WfClientRolePropertyValue pve in expected.Values)
            {
                string actualValue = actual.Values.GetValue(pve.Column.Name, string.Empty);

                AssertStringEqual(pve.Value, actualValue);
            }
        }

        public static void AreSame(this WfClientDelegation expected, WfDelegation actual)
        {
            Assert.AreEqual(expected.SourceUserID, actual.SourceUserID);
            Assert.AreEqual(expected.SourceUserName, actual.SourceUserName);
            Assert.AreEqual(expected.DestinationUserID, actual.DestinationUserID);
            Assert.AreEqual(expected.DestinationUserName, actual.DestinationUserName);
            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
            Assert.AreEqual(expected.Enabled, actual.Enabled);

            Assert.AreEqual(expected.ApplicationName, expected.ApplicationName);
            Assert.AreEqual(expected.ProgramName, expected.ProgramName);
            Assert.AreEqual(expected.TenantCode, expected.TenantCode);
        }

        private static void AssertStringEqual(string expected, string actual)
        {
            if (expected.IsNotEmpty() || actual.IsNotEmpty())
                Assert.AreEqual(expected, actual);
        }


        public static void AreSame(this WfClientProcessDescriptorInfo client, WfProcessDescriptorInfo server)
        {
            AssertStringEqual(client.ProcessKey, server.ProcessKey);
            AssertStringEqual(client.ProcessName, server.ProcessName);
            AssertStringEqual(client.ApplicationName, server.ApplicationName);
            AssertStringEqual(client.ProgramName, server.ProgramName);

            AssertStringEqual(client.Data, server.Data);
            Assert.AreEqual(client.Enabled, server.Enabled);

            Assert.AreEqual(client.CreateTime, server.CreateTime);
            Assert.AreEqual(client.ModifyTime, server.ModifyTime);
            Assert.AreEqual(client.ImportTime, server.ImportTime);

            client.Creator.AreSame(server.Creator);
            client.Modifier.AreSame(server.Modifier);
            client.ImportUser.AreSame(server.ImportUser); 
          
        }
        public static void AreSame(this WfClientProcessDescriptorInfo expected, WfClientProcessDescriptorInfo actual)
        {
            AssertStringEqual(expected.ProcessKey, actual.ProcessKey);
            AssertStringEqual(expected.ProcessName, actual.ProcessName);
            AssertStringEqual(expected.ApplicationName, actual.ApplicationName);
            AssertStringEqual(expected.ProgramName, actual.ProgramName);

            AssertStringEqual(expected.Data, actual.Data);
            Assert.AreEqual(expected.Enabled, actual.Enabled);

            Assert.AreEqual(expected.CreateTime, actual.CreateTime);
            Assert.AreEqual(expected.ModifyTime, actual.ModifyTime);
            Assert.AreEqual(expected.ImportTime, actual.ImportTime);

            expected.Creator.AreSame(actual.Creator);
            expected.Modifier.AreSame(actual.Modifier);
            expected.ImportUser.AreSame(actual.ImportUser);

        }
    }
}
