using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.Ctrl.Test.localhost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        public ActionResult Index()
        {
            localhost.TaskPlugin taskplugin = new localhost.TaskPlugin();
            UserTask user1 = new UserTask();
            user1.TaskID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";
            user1.Status = "2";
            user1.TaskStartTime = DateTime.Now;
            user1.ExpireTime = DateTime.Now;
            user1.DeliverTime = DateTime.Now;
            user1.DraftUserID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";

            UserTask user2 = new UserTask();
            user2.TaskID = "A16D4CCD-7833-4E54-BD13-529C55A291D6";
            user2.Status = "2";
            user1.TaskStartTime = DateTime.Now;
            user1.ExpireTime = DateTime.Now;
            user1.DeliverTime = DateTime.Now;
            user1.DraftUserID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";



            List<UserTask> userList = new List<UserTask>();

            userList.Add(user1);
            userList.Add(user2);

            var serializer = new JavaScriptSerializer();
            string strJson = serializer.Serialize(userList);

            DictionaryEntry[] args = new DictionaryEntry[1];
            args[0] = new DictionaryEntry() { Key = "TenantCode", Value = "Test1" };
            taskplugin.SendUserTasks(strJson, args);
            return View();
        }
    }
}