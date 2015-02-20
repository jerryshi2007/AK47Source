using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public static class ProcessRuntimeHelper
    {
        public static WfClientProcessStartupParams PrepareClientProcessStartupParams(string processKey)
        {
            WfClientProcessStartupParams startupParams = new WfClientProcessStartupParams();

            startupParams.ProcessDescriptorKey = processKey;
            startupParams.AutoCommit = true;
            startupParams.AutoStartInitialActivity = true;
            startupParams.CheckStartProcessUserPermission = true;
            startupParams.DefaultTaskTitle = "Please approve";
            startupParams.DefaultUrl = "http://www.sina.com"; ;
            startupParams.Creator = Consts.Users["Requestor"];
            startupParams.Department = Consts.Departments["RequestorOrg"];
            startupParams.RelativeID = UuidHelper.NewUuidString();
            startupParams.RelativeURL = "http://www.baidu.com";
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.RuntimeProcessName = "Test Process";

            startupParams.ApplicationRuntimeParameters["Amount"] = 1000;
            startupParams.ApplicationRuntimeParameters["CostCenter"] = "1001";

            startupParams.ProcessContext["Context"] = "This is a context";

            startupParams.Assignees.Add(Consts.Users["Requestor"]);

            return startupParams;
        }
    }
}
