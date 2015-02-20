using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

using MCS.Web.Library;

using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 已阅控件
	/// </summary>
	[ToolboxData("<{0}:WfReadControl runat=server></{0}:WfReadControl>")]
    public class WfReadControl : WebControl, INamingContainer
    {
        public static void SetTaskAccomplishedAfterRead(Object sender, EventArgs e)
        {
			WfControlBase.ResponsePostBackPlaceHolder();

            string requestTaskID = HttpContext.Current.Request["taskID"];

            if (string.IsNullOrEmpty(requestTaskID))
                return;

            UserTaskCollection usc = UserTaskAdapter.Instance.GetUserTasks(
                UserTaskIDType.TaskID, UserTaskFieldDefine.Url, requestTaskID);

            if (usc.Count > 0)
            {
                UserTask task = usc[0];
                task.SendToUserID = DeluxeIdentity.CurrentUser.ID;
                task.TaskID = requestTaskID;

                string url = task.Url;
                int removePosStart = url.IndexOf("needReadBtn");
                if (removePosStart >= 0) // found request query *needReadBtn*
                {
                    int removePosEnd = url.IndexOf('&', removePosStart);
                    if (removePosEnd > removePosStart) // found *needReadBtn=xxx&*, remove it
                    {
                        url = url.Remove(removePosStart, removePosEnd - removePosStart + 1);
                    }
                    else // not '&' after query string *needReadBtn*, remove all after *needReadBtn*
                    {
                        url = url.Remove(removePosStart);
                    }
                }
                task.Url = url;

                //UserTaskAdapter.Instance.SetTaskAccomplished(task,
                //    UserTaskIDType.TaskID | UserTaskIDType.SendToUserID,
                //    UserTaskFieldDefine.Url);
                UserTaskAdapter.Instance.SetUserTasksAccomplished(usc);
				WfControlBase.ResponsePostBackPlaceHolder();
                HttpContext.Current.Response.Write(ExtScriptHelper.GetRefreshBridgeScript());

				WebUtility.ResponseTimeoutScriptBlock("top.close();", ExtScriptHelper.DefaultResponseTimeout);
            }
        }
    }
}
