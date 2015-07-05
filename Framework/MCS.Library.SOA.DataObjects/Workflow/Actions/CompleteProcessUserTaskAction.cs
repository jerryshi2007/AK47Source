using MCS.Library.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 办结时发送通知的Action
    /// </summary>
    public class CompleteProcessUserTaskAction : UserTaskActionBase
    {
        public override void PrepareAction(WfActionParams actionParams)
        {
            IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

            if (currentActivity != null)
            {
                //暂时对当前活动的处理人发送通知
                UserTaskCollection notifyTasks = BuildUserNotifiesFromActivity(currentActivity);
                AppendResourcesToNotifiers(currentActivity, notifyTasks, currentActivity.Descriptor.Process.CompleteEventReceivers);

                foreach (UserTask task in notifyTasks)
                {
                    task.Status = TaskStatus.Yue;
                    task.TaskTitle = Translator.Translate(Define.DefaultCulture,
                        currentActivity.Process.Descriptor.Properties.GetValue("DefaultCompleteTaskPrefix", "流程已办结:")) + task.TaskTitle;
                }

                WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(notifyTasks);
                WfRuntime.ProcessContext.Acl.CopyFrom(notifyTasks.ToAcl());
            }
        }

        public override void AfterWorkflowPersistAction(WfActionParams actionParams)
        {
            WfRuntime.ProcessContext.NotifyUserTasks.DistinctByActivityUserAndStatus();
            UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);
        }

        public override void ClearCache()
        {
            WfRuntime.ProcessContext.NotifyUserTasks.Clear();
        }
    }
}
