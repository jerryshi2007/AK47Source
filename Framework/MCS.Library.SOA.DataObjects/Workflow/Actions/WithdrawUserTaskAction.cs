using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 撤回操作的UserTask生成器
    /// </summary>
    [Serializable]
    public class WithdrawUserTaskAction : UserTaskActionBase
    {
        public override void PrepareAction(WfActionParams actionParams)
        {
            if (WfRuntime.ProcessContext.OriginalActivity != null)
            {
                UserTaskCollection currentProcessTasks =
                            UserTaskAdapter.Instance.LoadUserTasks(builder => builder.AppendItem("ACTIVITY_ID",
                                WfRuntime.ProcessContext.OriginalActivity.ID));

                //删除已经发送的待办
                WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(currentProcessTasks);

                //对被撤回的点发送通知
                this.PrepareNotifyTasks(WfRuntime.ProcessContext.OriginalActivity);

                IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

                //对当前的活动点发送待办
                if (currentActivity != null)
                    this.PrepareMoveToTasks(currentActivity);

                WfRuntime.ProcessContext.FireWithdrawPrepareAction();
            }
        }

        public override void PersistAction(WfActionParams actionParams)
        {
            WfRuntime.ProcessContext.FireWithdrawPersistAction();

            WfRuntime.ProcessContext.MoveToUserTasks.DistinctByActivityUserAndStatus();
            UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);

            WfRuntime.ProcessContext.NotifyUserTasks.DistinctByActivityUserAndStatus();
            UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);

            UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);

            AppCommonInfoAdapter.Instance.UpdateProcessStatus(WfRuntime.ProcessContext.AffectedProcesses.FindAll(p => p.IsApprovalRootProcess));

            ClearCache();
        }

        public override void ClearCache()
        {
            WfRuntime.ProcessContext.MoveToUserTasks.Clear();
            WfRuntime.ProcessContext.NotifyUserTasks.Clear();
            WfRuntime.ProcessContext.DeletedUserTasks.Clear();
        }

        private void PrepareNotifyTasks(IWfActivity originalActivity)
        {
            UserTaskCollection notifyTasks = BuildUserNotifiesFromActivity(originalActivity);

            foreach (UserTask task in notifyTasks)
            {
                task.Status = TaskStatus.Yue;
                task.TaskTitle = Translator.Translate(Define.DefaultCulture, 
                    originalActivity.Process.Descriptor.Properties.GetValue("DefaultWithtrawTaskPrefix", "流程被撤回:")) + task.TaskTitle;
            }

            WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(notifyTasks);
        }

        private void PrepareMoveToTasks(IWfActivity currentActivity)
        {
            UserTaskCollection moveToTasks = BuildUserTasksFromActivity(currentActivity);

            WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(moveToTasks);
        }
    }
}
