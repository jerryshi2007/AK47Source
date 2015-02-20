using MCS.Library.Core;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WcfExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;



namespace WfOperationServices.Test
{
    public static class OperationHelper
    {
        /// <summary>
        /// 创建一个简单流程并且保存一个流程定义
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor PrepareSimpleProcess()
        {
            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateSimpleClientProcessWithLines();

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(processDesp);

            return processDesp;
        }

        /// <summary>
        /// 创建一个有4个节点的流程，并且保存它。开始点有两条出线，根据Amount是否大于等于5000来判断。
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor PreapreProcessWithConditionLines()
        {
            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateClientProcessWithConditionLines();

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(processDesp);

            return processDesp;
        }

        /// <summary>
        /// 创建一个简单流程，并且启动该流程
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessInfo PrepareSimpleProcessInstance()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            return WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);
        }

        /// <summary>
        /// 创建一个有4个节点的流程，并且保存它。开始点有两条出线，根据Amount是否大于等于5000来判断。
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessInfo PreapreProcessWithConditionLinesInstance()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PreapreProcessWithConditionLines();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            clientStartupParams.ApplicationRuntimeParameters["Amount"] = 10000;
            clientStartupParams.ProcessContext["Context"] = "This is a context";

            return WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);
        }

        public static WfClientProcessInfo PrepareProcessWithWithActivityMatrixResourceInstance()
        {
            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateClientProcessWithActivityMatrixResourceDescriptor();
            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(processDesp);

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            clientStartupParams.ApplicationRuntimeParameters["CostCenter"] = "1001";
            clientStartupParams.ApplicationRuntimeParameters["PayMethod"] = "1";
            clientStartupParams.ApplicationRuntimeParameters["Age"] = 30;

            return WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);
        }

        public static WfClientProcessInfo PrepareBranchProcesses()
        {
            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateSimpleClientProcessWithoutLines();

            //增加子节点
            WfClientActivityDescriptor branchActDesp = new WfClientActivityDescriptor(WfClientActivityType.NormalActivity);
            branchActDesp.Key = "N1";
            branchActDesp.Name = "CFO";

            branchActDesp.Properties.AddOrSetValue("AutoStartBranchProcesses", true);
            branchActDesp.Properties.AddOrSetValue("AutoStartBranchApprovalProcess", true);
            branchActDesp.Properties.AddOrSetValue("AutoStartBranchProcessExecuteSequence", 0);

            //增加审批人
            branchActDesp.Resources.Add(new WfClientUserResourceDescriptor(Consts.Users["Requestor"]));
            branchActDesp.Resources.Add(new WfClientUserResourceDescriptor(Consts.Users["Approver1"]));
            branchActDesp.Resources.Add(new WfClientUserResourceDescriptor(Consts.Users["CEO"]));

            processDesp.Activities.Add(branchActDesp);

            //线
            WfClientTransitionDescriptor transition = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, branchActDesp.Key);
            transition.Key = "L1";
            processDesp.InitialActivity.ToTransitions.Add(transition);

            WfClientTransitionDescriptor transition2 = new WfClientTransitionDescriptor(branchActDesp.Key, processDesp.CompletedActivity.Key);
            transition2.Key = "L2";
            branchActDesp.ToTransitions.Add(transition2);

            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(processDesp);

            //启动参数
            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            //流程实例
            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();
            WfClientProcessInfo process = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);

            return WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(process.ID, runtimeContext);
        }
        public static bool ClearProcessDescriptorSqlServerData(string key)
        {
            WfClientProcessDescriptorServiceProxy.Instance.DeleteDescriptor(key);
            return true;
        }
    }
}
