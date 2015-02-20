using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    [Serializable]
    public class LeaveActivityUserTaskAction : UserTaskActionBase
    {
        public override void PrepareAction(WfActionParams actionParams)
        {
            if (WfRuntime.ProcessContext.OriginalActivity != null)
            {
                IWfActivity originalActivity = WfRuntime.ProcessContext.OriginalActivity;

                //找到当前环节所有的待办，然后转为已办
                UserTaskCollection currentActTasks =
                    UserTaskAdapter.Instance.LoadUserTasks(builder =>
                        {
                            builder.AppendItem("ACTIVITY_ID", originalActivity.ID);
                            builder.AppendItem("STATUS", (int)TaskStatus.Ban);
                        });

                //从上下文中找到需要转已办的记录
                currentActTasks.CopyFrom(WfRuntime.ProcessContext.MoveToUserTasks.FindAll(
                    u => string.Compare(u.ActivityID, originalActivity.ID, true) == 0));

                currentActTasks.Sort((u1, u2) => u1.Level - u2.Level);

                if (currentActTasks.Count == 0)
                {
                    UserTaskCollection preparedUserTasks = (UserTaskCollection)originalActivity.Context["UserTasks"];

                    if (preparedUserTasks != null)
                    {
                        WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(preparedUserTasks);
                        currentActTasks = preparedUserTasks;
                    }
                }

                currentActTasks.ForEach(u =>
                {
                    if (u.Status == TaskStatus.Ban)
                        ChangeUserTaskToAccomplishedTasks(originalActivity, u);
                    else
                        WfRuntime.ProcessContext.DeletedUserTasks.Add(u);
                });

                UserTaskCollection notifyTasks = new UserTaskCollection();

                AppendResourcesToNotifiers(originalActivity, notifyTasks, originalActivity.Descriptor.LeaveEventReceivers);

                foreach (UserTask task in notifyTasks)
                {
                    task.Status = TaskStatus.Yue;
                    task.TaskTitle = Translator.Translate(Define.DefaultCulture,
                        originalActivity.Process.Descriptor.Properties.GetValue("DefaultLeaveTaskPrefix", "离开活动:")) + task.TaskTitle;
                }

                WfRuntime.ProcessContext.Acl.CopyFrom(notifyTasks.ToAcl());
                WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(notifyTasks);

                WfRuntime.ProcessContext.FireLeaveActivityPrepareAction();
            }
        }

        public override void PersistAction(WfActionParams actionParams)
        {
            WfRuntime.ProcessContext.FireLeaveActivityPersistAction();

            WfRuntime.ProcessContext.MoveToUserTasks.DistinctByActivityUserAndStatus();
            UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);

            WfRuntime.ProcessContext.NotifyUserTasks.DistinctByActivityUserAndStatus();
            UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);

            UserTaskAdapter.Instance.SetUserTasksAccomplished(WfRuntime.ProcessContext.AccomplishedUserTasks);
            UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);

            ClearCache();
        }

        public override void ClearCache()
        {
            WfRuntime.ProcessContext.MoveToUserTasks.Clear();
            WfRuntime.ProcessContext.NotifyUserTasks.Clear();
            WfRuntime.ProcessContext.AccomplishedUserTasks.Clear();
        }

        internal static void ChangeUserTaskToAccomplishedTasks(IWfActivity originalActivity, UserTask u)
        {
            NameValueCollection urlParams = UriHelper.GetUriParamsCollection(u.Url);

            urlParams["activityID"] = originalActivity.ID;

            urlParams["processID"] = originalActivity.Process.ApprovalRootProcess.ID;

            u.Url = UriHelper.CombineUrlParams(u.Url, urlParams);
            u.Context["ApprovalRootProcessID"] = originalActivity.Process.ApprovalRootProcess.ID;
            u.Context["ApprovalRootActivityID"] = originalActivity.ApprovalRootActivity.ID;

            WfRuntime.ProcessContext.AccomplishedUserTasks.Add(u);
        }
    }
}
