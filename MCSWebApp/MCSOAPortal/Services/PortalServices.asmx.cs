using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Script.Services;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Web.Library;
using MCS.OA.Portal.Common;

namespace MCS.OA.Portal.Services
{
    [Flags]
    public enum UserTaskQueryType
    {
        UserTaskCount = 0,
        Notify = 1,
        NewTaskArrived = 2
    }

    /// <summary>
    /// PortalServices 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class PortalServices : System.Web.Services.WebService
    {
        #region SimpleTask
        private class SimpleTask
        {
            string taskID = string.Empty;
            string taskTitle = string.Empty;
            private DateTime deliverTime = DateTime.MinValue;
            string applicationName = string.Empty;
            string url = string.Empty;
            string feature = string.Empty;
            private TaskStatus status = TaskStatus.Ban;

            public SimpleTask(UserTask task)
            {
                this.taskID = task.TaskID;
                this.taskTitle = HttpUtility.HtmlEncode(task.TaskTitle);
                this.deliverTime = task.DeliverTime;
                this.applicationName = task.ApplicationName;
                this.url = task.Url;
                this.feature = GridCommon.GetFeature(task);
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

            public string Feature
            {
                get
                {
                    return this.feature;
                }
                set
                {
                    this.feature = value;
                }
            }
        }
        #endregion

        [WebMethod]
        public void UpdateTaskReadTime(string taskID)
        {
            UserTaskAdapter.Instance.SetTaskReadFlag(taskID);
        }

        [WebMethod]
        public void UpdateCompletedTaskReadTime(string taskID)
        {
            UserTaskAdapter.Instance.SetAccomplishedTaskReadFlag(taskID);
        }

        [WebMethod]
        public string[] QueryUserTaskStatus(string[] reqParams)
        {
            EnumItemDescriptionList despList =
                EnumItemDescriptionAttribute.GetDescriptionList(typeof(UserTaskQueryType));

            string[] result = new string[despList.Count];

            string userTaskChangeResult = DoUserTaskChangedQuery(reqParams[(int)UserTaskQueryType.UserTaskCount]);
            result[(int)UserTaskQueryType.UserTaskCount] = userTaskChangeResult;

            if (string.IsNullOrEmpty(userTaskChangeResult) == false)
                result[(int)UserTaskQueryType.NewTaskArrived] = DoNewTaskArrived();

            return result;
        }

        private static string DoNewTaskArrived()
        {
            UserTask task = GetLastUserTaskQuery.GetLatestUserTask(DeluxeIdentity.CurrentUser.ID);

            string result = string.Empty;

            if (task != null)
            {
                SimpleTask st = new SimpleTask(task);

                result = JSONSerializerExecute.Serialize(st);
            }

            return result;
        }

        private static string DoUserTaskChangedQuery(string tag)
        {
            string result = string.Empty;

            string serverTag;

            if (UserTaskChangingCache.Instance.TryGetValue(DeluxeIdentity.CurrentUser.ID, out serverTag) == false)
            {
                UdpNotifierCacheDependency dependency = new UdpNotifierCacheDependency();

                serverTag = Guid.NewGuid().ToString();
                UserTaskChangingCache.Instance.Add(DeluxeIdentity.CurrentUser.ID, serverTag, dependency);
            }

            if (serverTag != tag)
                result = GetUserTaskCountString(serverTag);

            return result;
        }

        internal static string GetUserTaskCountString(string serverTag)
        {
            UserTaskCount utc = UserTaskAdapter.Instance.GetUserTaskCount(DeluxeIdentity.CurrentUser.ID);
            return "{" + string.Format("tag:\"{0}\", banCount:{1}, yueCount:{2}, banExpiredCount:{3}, yueExpiredCount:{4}",
                                serverTag, utc.BanCount, utc.YueCount, utc.BanExpiredCount, utc.YueExpiredCount) + "}";
        }
    }
}
