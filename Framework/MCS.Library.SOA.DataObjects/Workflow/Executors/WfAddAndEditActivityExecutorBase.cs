using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 运行时添加和删除主线活动执行器的基类
    /// </summary>
    public abstract class WfAddAndEditActivityExecutorBase : WfActivityRelativeExecutorBase
    {
        public WfActivityDescriptorCreateParams CreateParams
        {
            get;
            private set;
        }

        protected WfAddAndEditActivityExecutorBase(IWfActivity operatorActivity, IWfActivity targetActivity, WfActivityDescriptorCreateParams createParams, WfControlOperationType operationType)
            : base(operatorActivity, targetActivity, operationType)
        {
            createParams.NullCheck("createParams");

            this.CreateParams = createParams;
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            IWfActivity instActivity = PrepareInstanceActivity();

            WfActivityDescriptor instActDesp = (WfActivityDescriptor)instActivity.Descriptor;

            AdjustActivityDescriptorProperties(instActDesp, this.CreateParams);

            IWfActivityDescriptor msTargetActDesp = this.TargetActivity.GetMainStreamActivityDescriptor();

            if (msTargetActDesp != null)
            {
                //同步增加主线活动
                IWfActivityDescriptor newMSActDesp = PrepareActivityDescriptor(msTargetActDesp);

                ((WfActivityBase)instActivity).MainStreamActivityKey = newMSActDesp.Key;
                AdjustActivityDescriptorProperties((WfActivityDescriptor)newMSActDesp, this.CreateParams);
            }

            instActivity.GenerateCandidatesFromResources();
			WfRuntime.DecorateProcess(instActivity.Process);
        }

        /// <summary>
        /// 准备流程实例中需要调整参数的活动
        /// </summary>
        /// <returns></returns>
        protected abstract IWfActivity PrepareInstanceActivity();

        /// <summary>
        /// 根据目标活动，准备需要处理的流程活动描述
        /// </summary>
        /// <param name="targetActDesp"></param>
        /// <returns></returns>
        protected abstract IWfActivityDescriptor PrepareActivityDescriptor(IWfActivityDescriptor targetActDesp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newActDesp"></param>
        /// <param name="createParams"></param>
        private static void AdjustActivityDescriptorProperties(WfActivityDescriptor newActDesp, WfActivityDescriptorCreateParams createParams)
        {
            newActDesp.Name = createParams.Name;

            if (createParams.Variables != null)
            {
                foreach (WfVariableDescriptor variable in createParams.Variables)
                {
                    if (newActDesp.Properties.ContainsKey(variable.Key) == false)
                    {
                        PropertyDefine pd = new PropertyDefine();

                        pd.Name = variable.Key;
                        pd.DataType = PropertyDataType.Boolean;
                        pd.DefaultValue = variable.OriginalValue;

                        newActDesp.Properties.Add(new PropertyValue(pd));
                    }

                    newActDesp.Properties.SetValue(variable.Key, variable.ActualValue);
                }
            }

            if (createParams.Users != null)
            {
                newActDesp.Resources.Clear();
                createParams.Users.ForEach(u => newActDesp.Resources.Add(new WfUserResourceDescriptor(u)));
            }

            if (createParams.CirculateUsers != null)
            {
                newActDesp.EnterEventReceivers.Clear();
                createParams.CirculateUsers.ForEach(u => newActDesp.EnterEventReceivers.Add(new WfUserResourceDescriptor(u)));
            }

            newActDesp.BranchProcessTemplates.Clear();

            if (createParams.AllAgreeWhenConsign && newActDesp.Resources.Count > 1)
            {
                newActDesp.Properties.SetValue("AutoMoveAfterPending", true);

				if (newActDesp.BranchProcessTemplates.ContainsKey(WfTemplateBuilder.AutoStartSubProcessTemplateKey))
					newActDesp.BranchProcessTemplates.Remove(t => t.Key == WfTemplateBuilder.AutoStartSubProcessTemplateKey);

				WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(WfTemplateBuilder.AutoStartSubProcessTemplateKey);

                template.BranchProcessKey = WfProcessDescriptorManager.DefaultConsignProcessKey;
                template.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;
                template.BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;

                template.Resources.CopyFrom(newActDesp.Resources);

                newActDesp.BranchProcessTemplates.Add(template);
            }
        }
    }
}
