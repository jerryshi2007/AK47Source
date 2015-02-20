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
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Web.Library.Script;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCSResponsiveOAPortal
{
    /// <summary>
    /// PortalServices 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    [XmlInclude(typeof(UserTaskCount)), XmlInclude(typeof(SimpleTask))]
    public class PortalServices : System.Web.Services.WebService
    {
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
        public object[] QueryUserTaskStatus()
        {
            return TaskStat.GetUserTaskStatusData();
        }

        [WebMethod]
        public object[] GetProgramsInApplication(string appName)
        {
            return WfApplicationAdapter.Instance.LoadProgramsByApplication(appName).ToArray();
        }
    }
}
