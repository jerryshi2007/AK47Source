using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class ActivityConverterTest
    {
        private static readonly string UserID = UuidHelper.NewUuidString();
        private static readonly string OrganizationID = UuidHelper.NewUuidString();
        private static readonly string GroupID = UuidHelper.NewUuidString();
        private const string ResActivityKey = "N1";
        private const string RoleFullCodeName = "AppAdmin:Admin";

        [TestMethod]
        public void StandardClientActivitiesToServer()
        {
            WfClientActivityDescriptor clientActDesp = PrepareClientActivity("N1");

            clientActDesp.Output();

            WfActivityDescriptor serverActDesp = null;

            WfClientActivityDescriptorConverter.Instance.ClientToServer(clientActDesp, ref serverActDesp);

            serverActDesp.Output();
            serverActDesp.Variables.Output();

            clientActDesp.AssertActivityDescriptor(serverActDesp);
        }

        [TestMethod]
        public void StandardServerActivitiesToClient()
        {
            WfActivityDescriptor serverActDesp = PrepareServerActivity("N1");

            serverActDesp.Output();

            WfClientActivityDescriptor clientActDesp = null;

            WfClientActivityDescriptorConverter.Instance.ServerToClient(serverActDesp, ref clientActDesp);

            clientActDesp.Output();
            clientActDesp.Variables.Output();

            clientActDesp.AssertActivityDescriptor(serverActDesp);
        }

        private static WfClientActivityDescriptor PrepareClientActivity(string key)
        {
            WfClientActivityDescriptor actDesp = new WfClientActivityDescriptor(WfClientActivityType.InitialActivity);

            actDesp.Key = key;
            actDesp.Name = key;

            actDesp.Condition.Expression = "Amount > 1000";
            actDesp.Variables.AddOrSetValue("ActKey", key);
            actDesp.Variables.AddOrSetValue("ActName", key);

            actDesp.Resources.Add(new WfClientUserResourceDescriptor(new WfClientUser(ActivityConverterTest.UserID)));
            actDesp.Resources.Add(new WfClientDepartmentResourceDescriptor(new WfClientOrganization(ActivityConverterTest.OrganizationID)));
            actDesp.Resources.Add(new WfClientGroupResourceDescriptor(new WfClientGroup(ActivityConverterTest.GroupID)));
            actDesp.Resources.Add(new WfClientRoleResourceDescriptor(ActivityConverterTest.RoleFullCodeName));
            actDesp.Resources.Add(new WfClientActivityOperatorResourceDescriptor(ActivityConverterTest.ResActivityKey));
            actDesp.Resources.Add(new WfClientActivityAssigneesResourceDescriptor(ActivityConverterTest.ResActivityKey));
            actDesp.Resources.Add(new WfClientDynamicResourceDescriptor("ConditionResource", "Amount > 1000"));
            actDesp.BranchProcessTemplates.Add(BranchProcessTemplateConverterTest.PrepareClientBranchProcessTemplate());

            return actDesp;
        }

        private static WfActivityDescriptor PrepareServerActivity(string key)
        {
            WfActivityDescriptor actDesp = new WfActivityDescriptor(key, WfActivityType.InitialActivity);

            actDesp.Key = key;
            actDesp.Name = key;

            actDesp.Condition.Expression = "Amount > 1000";
            actDesp.Variables.SetValue("ActKey", key);
            actDesp.Variables.SetValue("ActName", key);

            actDesp.Resources.Add(new WfUserResourceDescriptor((IUser)OguUser.CreateWrapperObject(ActivityConverterTest.UserID, SchemaType.Users)));
            actDesp.Resources.Add(new WfDepartmentResourceDescriptor((IOrganization)OguOrganization.CreateWrapperObject(ActivityConverterTest.OrganizationID, SchemaType.Organizations)));
            actDesp.Resources.Add(new WfGroupResourceDescriptor((IGroup)OguGroup.CreateWrapperObject(ActivityConverterTest.GroupID, SchemaType.Groups)));
            actDesp.Resources.Add(new WfRoleResourceDescriptor(new SOARole(ActivityConverterTest.RoleFullCodeName)));
            actDesp.Resources.Add(new WfActivityOperatorResourceDescriptor() { ActivityKey = ActivityConverterTest.ResActivityKey });
            actDesp.Resources.Add(new WfActivityAssigneesResourceDescriptor() { ActivityKey = ActivityConverterTest.ResActivityKey });
            actDesp.Resources.Add(new WfDynamicResourceDescriptor("ConditionResource", "Amount > 1000"));
            actDesp.BranchProcessTemplates.Add(BranchProcessTemplateConverterTest.PrepareServerBranchProcessTemplate());

            return actDesp;
        }
    }
}
