using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;

namespace MCS.OA.Portal.Common
{
    internal sealed class ReplaceAssigneeHelper
    {
        public static void ExecuteReplace(ReplaceAssigneeHelperData rahd)
        {
            rahd.NullCheck("错误：传入参数为空");

            //取原始待办人
            IUser originalUser = (IUser)OguBase.CreateWrapperObject(rahd.OriginalUserID, SchemaType.Users);

            //取目的待办人
            List<IUser> targetUsers = new List<IUser>();
            foreach (string userID in rahd.TargetUsersID)
            {
                IUser targetUser = (IUser)OguBase.CreateWrapperObject(userID, SchemaType.Users);
                targetUsers.Add(targetUser);
            }

            //获取用户任务
            UserTaskCollection tasks = UserTaskAdapter.Instance.LoadUserTasks(
                                        build => build.AppendItem("TASK_GUID", rahd.TaskID, "="));
			(tasks.Count > 0).FalseThrow("指定的任务ID'{0}'不存在！", rahd.TaskID);

            //取出Activity
			string activityID = tasks[0].ActivityID;
            //待办人替换处理
            try
            {
                IWfProcess process = WfRuntime.GetProcessByActivityID(activityID);

                IWfActivity activity = process.Activities[activityID];

                //创建Executor并执行
                WfReplaceAssigneesExecutor replaceExec = new WfReplaceAssigneesExecutor(activity, activity,
                                                                    originalUser, targetUsers);
                replaceExec.Execute();
            }
            catch 
            {
                
            }
        } 
    }

    //替换待办人辅助信息
    internal class ReplaceAssigneeHelperData
    {
        //任务的TaskID
        public string TaskID { get; set; }
        //原始待办人的ID
        public string OriginalUserID { get; set; }
        //待办人ID数组
        public string[] TargetUsersID { get; set; }

        public ReplaceAssigneeHelperData()
        { }
    }
}