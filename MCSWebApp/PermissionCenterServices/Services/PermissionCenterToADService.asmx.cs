using MCS.Library.Core;
using MCS.Library.SOA.Security.ADSyncUtilities;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;

namespace PermissionCenter.Services
{
    /// <summary>
    /// Summary description for PermissionCenterToADService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PermissionCenterToADService : System.Web.Services.WebService
    {
        [WebMethod]
        public void StartSynchronize(string startPath, string taskID)
        {
            System.Diagnostics.Debug.WriteLine("AD同步开始于" + DateTime.Now.ToString());

            PermissionCenterSynchronizer synchronizer = new PermissionCenterSynchronizer();

            synchronizer.Start(startPath, taskID);

            System.Diagnostics.Debug.WriteLine("AD同步结束于" + DateTime.Now.ToString());
        }

        [WebMethod]
        public void StartADReverseSynchronize(string taskID)
        {
            System.Diagnostics.Debug.WriteLine("AD逆向同步开始于" + DateTime.Now.ToString());

            ADToPermissionCenterSynchronizer.Start(taskID);

            System.Diagnostics.Debug.WriteLine("AD逆向同步结束于" + DateTime.Now.ToString());
        }

        [WebMethod]
        public string GetVersion(string callerID)
        {
            string result = string.Format("Caller ID: {0}, TenantCode: {1}, Version: {2}",
                callerID,
                TenantContext.Current.TenantCode,
                this.GetType().AssemblyQualifiedName);

            EventLog.WriteEntry("PermissionCenterToADService", result);

            return result;
        }

        [WebMethod]
        public string LongWaiting(int minite)
        {
            EventLog.WriteEntry("PermissionCenter", "开始LongWaiting");

            for (int i = minite * 2 - 1; i >= 0; i--)
            {
                Thread.Sleep(30 * 1000);
                EventLog.WriteEntry("PermissionCenter", "仍在继续 LongWaiting");
            }

            EventLog.WriteEntry("PermissionCenter", "结束LongWaiting");

            return "OK";
        }
    }
}
