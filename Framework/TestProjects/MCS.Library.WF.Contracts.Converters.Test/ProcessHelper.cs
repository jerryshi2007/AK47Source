using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Converters.Descriptors;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    public static class ProcessHelper
    {
        public static IWfProcessDescriptor CreateFreeStepsProcess(params IUser[] stepUsers)
        {
            WfFreeStepsProcessBuilder builder = new WfFreeStepsProcessBuilder("TestApp", "TestProg", stepUsers);

            return builder.Build(UuidHelper.NewUuidString(), "FreeSteps");
        }

        public static IWfProcess CreateFreeStepsProcessInstance(params IUser[] stepUsers)
        {
            IWfProcessDescriptor processDesp = CreateFreeStepsProcess(stepUsers);

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            process.Context["Context"] = "This is a context";

            return process;
        }

        public static IWfProcess CreateProcessInstance(WfClientProcessDescriptor client)
        {
            WfProcessDescriptor processDesp = null;

            WfClientProcessDescriptorConverter.Instance.ClientToServer(client, ref processDesp);
            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            return process;
        }

        public static WfProcessStartupParams GetInstanceOfWfProcessStartupParams(IWfProcessDescriptor processDesp)
        {
            WfProcessStartupParams startupParams = new WfProcessStartupParams();

            IUser requestor = (IUser)Consts.Users["Requestor"].ToOguObject();

            startupParams.Creator = requestor;
            startupParams.Department = requestor.TopOU;
            startupParams.DefaultUrl = "http://www.sina.com.cn";
            startupParams.Assignees.Add(new WfAssignee(requestor));
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.DefaultTaskTitle = "测试保存的流程";
            startupParams.ProcessDescriptor = processDesp;
            startupParams.AutoCommit = true;

            startupParams.ApplicationRuntimeParameters["Amount"] = 1000;
            startupParams.ApplicationRuntimeParameters["CostCenter"] = "1001";

            return startupParams;
        }
    }
}
