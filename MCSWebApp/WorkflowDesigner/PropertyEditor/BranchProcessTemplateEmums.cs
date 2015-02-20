using System;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner.PropertyEditor
{
    public enum BranchProcessKey
    {
        [EnumItemDescription(WfProcessDescriptorManager.DefaultApprovalProcessKey +"-默认审批流程", WfProcessDescriptorManager.DefaultApprovalProcessKey, 0)]
        DefaultApprovalProcessKey,
        [EnumItemDescription(WfProcessDescriptorManager.DefaultConsignProcessKey +"-默认会签流程", WfProcessDescriptorManager.DefaultConsignProcessKey, 1)]
        DefaultConsignProcessKey,
        [EnumItemDescription(WfProcessDescriptorManager.DefaultCirculationProcessKey + "-默认传阅流程", WfProcessDescriptorManager.DefaultCirculationProcessKey, 2)]
        DefaultCirculationProcessKey
    }

}