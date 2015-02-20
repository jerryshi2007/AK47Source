using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;

namespace MCSResponsiveOAPortal
{
    public class SimpleTask
    {
        string taskID = string.Empty;
        string taskTitle = string.Empty;
        private DateTime deliverTime = DateTime.MinValue;
        string applicationName = string.Empty;
        string url = string.Empty;
        private TaskStatus status = TaskStatus.Ban;

        public SimpleTask()
        {
        }

        public SimpleTask(UserTask task)
        {
            this.taskID = task.TaskID;
            this.taskTitle = HttpUtility.HtmlEncode(task.TaskTitle);
            this.deliverTime = task.DeliverTime;
            this.applicationName = task.ApplicationName;
            this.url = task.Url;
            this.status = task.Status;
        }

        public string TaskID
        {
            get
            {
                return this.taskID;
            }
            set
            {
                this.taskID = value;
            }
        }

        public string TaskTitle
        {
            get
            {
                return this.taskTitle;
            }
            set
            {
                this.taskTitle = value;
            }
        }

        public DateTime DeliverTime
        {
            get
            {
                return this.deliverTime;
            }
            set
            {
                this.deliverTime = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }
            set
            {
                this.applicationName = value;
            }
        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        public TaskStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }

        public string PopupTitle
        {
            get
            {
                string result = "新待办事项";

                if (this.status == TaskStatus.Yue)
                    result = "新待阅事项";

                return result;
            }
        }
    }

    public static class TaskStat
    {
        public const int UserTaskCountField = 0;
        public const int NotifyField = 1;
        public const int NewTaskArrivedField = 2;

        public static object[] GetUserTaskStatusData()
        {
            object[] result = new object[5];

            var taskStat = UserTaskAdapter.Instance.GetUserTaskCount(DeluxeIdentity.CurrentUser.ID);
            result[TaskStat.UserTaskCountField] = taskStat;
            if (taskStat.BanCount > 0)
            {
                var lastTask = TaskQuery.Instance.GetLatestUserTask(DeluxeIdentity.CurrentUser.ID);
                if (lastTask != null)
                    result[TaskStat.NewTaskArrivedField] = new SimpleTask(lastTask);
            }

            return result;
        }
    }
}