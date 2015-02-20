using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIIC.HSR.TSP.TA.Bizlet.Impl.Test;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.IoC;
using System.Collections.Generic;

using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using CIIC.HSR.TSP.WF.Unitest.localhost;
using CIIC.HSR.TSP.WF.BizObject;

namespace CIIC.HSR.TSP.WF.Unitest
{
    [TestClass]
    public class UnitTest1
    {
        private string _TenantCode = "Test1";
        [TestInitialize]
        public void Initialize()
        {
            IoCConfig.Start();
        }

        [TestMethod]
        public void QueryTasks()
        {
            localhost.TaskPlugin taskplugin = new localhost.TaskPlugin();
            localhost.UserTaskQueryCondition condition = new localhost.UserTaskQueryCondition();

            string sendToUserId = "25d9edad-3824-4399-896a-bca29dbc50a7";
            string tenantCode = "Test1";

            condition.TaskType = localhost.TaskStatus.Unprocessed;//查询待办 已办

            //condition.ProcessStatus = localhost.WfProcessStatus.Running;//流程运行中

            condition.ProgramName = "合同审批";
            condition.ApplicationName = "销售项目";
            condition.TaskTitle = "流程标题";
            condition.CreatedFrom = DateTime.Now.ToUniversalTime();
            condition.CreatedTo= DateTime.Now.ToUniversalTime();
            condition.CreatorUserId = "25d9edad-3824-4399-896a-bca29dbc50a7";

            var queryData = taskplugin.QueryTask(tenantCode, sendToUserId, condition, 1, 1, 100);

            Assert.IsTrue(true);
  
        }
                [TestMethod]
        public void SyncProcess()
        {
           localhost.TaskPlugin taskplugin = new localhost.TaskPlugin();

           ProcessBO bo1 = new ProcessBO();

           bo1.ProcessId = new Guid("a1584abd-7321-8168-4e34-bd6943a148d6");
           bo1.ProcessName="测试的";
           bo1.ProcessKey="TEST";
           bo1.Status="Running";
           bo1.CreatorId="941c2d73-77f2-4654-952d-4a9af899f11a";
           bo1.CreatorName = "张三";
           bo1.Created=DateTime.Now;

           ProcessBO bo2 = new ProcessBO();
           bo2.ProcessId = new Guid("a1574abd-7321-8168-4e34-bd6943a148d6");
           bo2.ProcessName = "测试的";
           bo2.ProcessKey = "TEST";
           bo2.Status = "Running";
           bo2.CreatorId = "941c2d73-77f2-4654-952d-4a9af899f12a";
           bo2.CreatorName = "张三";
           bo2.Created = DateTime.Now;

           List<ProcessBO> testList = new List<ProcessBO>();
           testList.Add(bo1);
           testList.Add(bo2);

           string listObjs = JsonConvert.SerializeObject(testList);

           taskplugin.SyncProcess(listObjs);
           Assert.IsTrue(true);

           //IServiceFactory serviceFactory = Containers.Global.Singleton.Resolve<IServiceFactory>();
           //serviceFactory.Context.TenantCode=_TenantCode;

           //ITaskQuery iTask = serviceFactory.CreateService<ITaskQuery>();
           //ITaskOperator iTaskOperator = serviceFactory.CreateService<ITaskOperator>();
           //iTaskOperator.SyncProcess(testList);
           //Assert.IsTrue(true);


        }

        [TestMethod]
        public void SendUserTasks()
        {
            ////localhost.TaskPlugin taskplugin = new localhost.TaskPlugin();
           
            //IServiceFactory serviceFactory=Containers.Global.Singleton.Resolve<IServiceFactory>();
            //ITaskQuery iTask = serviceFactory.CreateService<ITaskQuery>();
            //iTask.Context.TenantCode = "Test1";
            ////string useId = "A16D4CCD-7833-4E54-BD13-529C55A291D6";
            ////var result = iTask.QueryTask(useId, TaskStatus.Unprocessed,1,1,100);
            ////Assert.AreEqual(true, result.Items.Count == 1);

            //ITaskOperator iTaskOperator = serviceFactory.CreateService<ITaskOperator>();

            //List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks = new List<BizObject.USER_TASKBO>();


            //CIIC.HSR.TSP.WF.BizObject.USER_TASKBO bo1 = new BizObject.USER_TASKBO();
            //bo1.TASK_GUID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";
            //bo1.STATUS = "2";
            //bo1.TASK_START_TIME = DateTime.Now;
            //bo1.EXPIRE_TIME = DateTime.Now;
            //bo1.DELIVER_TIME = DateTime.Now;
            //bo1.DRAFT_USER_ID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";


            ////CIIC.HSR.TSP.WF.BizObject.USER_TASKBO bo2 = new BizObject.USER_TASKBO();
            ////bo2.TASK_GUID = "A16D4CCD-7833-4E54-BD13-529C55A291D3";
            ////bo2.STATUS = "2";
            ////bo2.TASK_START_TIME = DateTime.Now;
            ////bo2.EXPIRE_TIME = DateTime.Now;
            ////bo2.DELIVER_TIME = DateTime.Now;
            ////bo2.DRAFT_USER_ID = "A16D4CCD-7833-4E54-BD13-529C55A291D3";

            //tasks.Add(bo1);
            ////tasks.Add(bo2);
            //iTaskOperator.SendUserTasks(tasks, null);
            ////iTaskOperator.SetUserTasksAccomplished(tasks, null);
            ////iTaskOperator.DeleteUserAccomplishedTasks(tasks, null);
            ////iTaskOperator.DeleteUserTasks(tasks, null);
            ////Assert.IsTrue(true);

        }


        [TestMethod]
        public void WebServiceInterface()
        {

            localhost.TaskPlugin taskplugin = new localhost.TaskPlugin();
            CIIC.HSR.TSP.WF.BizObject.UserTask user1 = new UserTask();
            user1.TaskID = Guid.NewGuid().ToString();
            user1.Status = null;
            user1.TaskStartTime = DateTime.Now;
            user1.ExpireTime = DateTime.Now;
            user1.DeliverTime = DateTime.Now;
            user1.DraftUserID = "A16D4CCD-7833-4E54-BD13-529C55A291D5";

            UserTask user2 = new UserTask();
            user2.TaskID = Guid.NewGuid().ToString();
            user2.Status = null;
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


            //Assert.IsTrue(true);


      

        }
        private static readonly string TestJson = "[{\"TASK_GUID\":\"737dec15-a855-4d8e-84fa-b4bc57ba0617\",\"APPLICATION_NAME\":null,\"PROGRAM_NAME\":null,\"TASK_LEVEL\":null,\"TASK_TITLE\":\"test\",\"RESOURCE_ID\":null,\"PROCESS_ID\":null,\"ACTIVITY_ID\":null,\"URL\":\"http://www.baidu.com\",\"DATA\":null,\"EMERGENCY\":null,\"PURPOSE\":null,\"STATUS\":null,\"TASK_START_TIME\":null,\"EXPIRE_TIME\":null,\"SOURCE_ID\":null,\"SOURCE_NAME\":null,\"SEND_TO_USER\":null,\"SEND_TO_USER_NAME\":null,\"READ_TIME\":\"0001-01-01T00:00:00\",\"CATEGORY_GUID\":null,\"TOP_FLAG\":null,\"DRAFT_DEPARTMENT_NAME\":null,\"DELIVER_TIME\":null,\"DRAFT_USER_ID\":null,\"DRAFT_USER_NAME\":null,\"TenantCode\":null,\"TaskType\":null}]";
        private static readonly string TestJson2 = "[{\"TaskID\":\"f6653c6c-8ffd-8773-4009-5280b6cf70cc\",\"ApplicationName\":\"\",\"ProgramName\":\"\",\"TaskTitle\":\"测试流程启动\",\"ResourceID\":\"\",\"SourceID\":\"4EF6BE7E-9300-416D-B390-EBB859A6D05B\",\"SourceName\":\"曹节\",\"SendToUserID\":\"cc431b22-2420-47f0-8166-0af2085255cd\",\"SendToUserName\":\"吴子兰\",\"Body\":\"\",\"Level\":3,\"ProcessID\":\"65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"ActivityID\":\"abb7c5ea-690d-aec3-4ee2-cef86547247a\",\"Url\":\"?activityID=abb7c5ea-690d-aec3-4ee2-cef86547247a\\u0026processID=65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"NormalizedUrl\":\"?activityID=abb7c5ea-690d-aec3-4ee2-cef86547247a\\u0026processID=65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"Emergency\":0,\"Purpose\":\"\",\"Status\":1,\"TaskStartTime\":\"\\/Date(1411810358000)\\/\",\"ExpireTime\":\"\\/Date(-62135596800000)\\/\",\"DeliverTime\":\"\\/Date(1411810363603)\\/\",\"ReadTime\":\"\\/Date(-62135596800000)\\/\",\"Category\":{\"CategoryID\":\"\",\"CategoryName\":\"\",\"UserID\":\"\",\"InnerSortID\":0},\"TopFlag\":0,\"DraftDepartmentName\":\"\",\"CompletedTime\":\"\\/Date(-62135596800000)\\/\",\"DraftUserID\":\"4EF6BE7E-9300-416D-B390-EBB859A6D05B\",\"DraftUserName\":\"曹节\"},{\"TaskID\":\"f6653c6c-8ffd-8773-4009-5280b6cf70cc\",\"ApplicationName\":\"\",\"ProgramName\":\"\",\"TaskTitle\":\"测试流程启动\",\"ResourceID\":\"\",\"SourceID\":\"4EF6BE7E-9300-416D-B390-EBB859A6D05B\",\"SourceName\":\"曹节\",\"SendToUserID\":\"cc431b22-2420-47f0-8166-0af2085255cd\",\"SendToUserName\":\"吴子兰\",\"Body\":\"\",\"Level\":3,\"ProcessID\":\"65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"ActivityID\":\"abb7c5ea-690d-aec3-4ee2-cef86547247a\",\"Url\":\"?activityID=abb7c5ea-690d-aec3-4ee2-cef86547247a\\u0026processID=65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"NormalizedUrl\":\"?activityID=abb7c5ea-690d-aec3-4ee2-cef86547247a\\u0026processID=65ec2ef1-6a0a-9f3d-45b2-ab8c02f9868b\",\"Emergency\":0,\"Purpose\":\"\",\"Status\":1,\"TaskStartTime\":\"\\/Date(1411810358000)\\/\",\"ExpireTime\":\"\\/Date(-62135596800000)\\/\",\"DeliverTime\":\"\\/Date(1411810363603)\\/\",\"ReadTime\":\"\\/Date(-62135596800000)\\/\",\"Category\":{\"CategoryID\":\"\",\"CategoryName\":\"\",\"UserID\":\"\",\"InnerSortID\":0},\"TopFlag\":0,\"DraftDepartmentName\":\"\",\"CompletedTime\":\"\\/Date(-62135596800000)\\/\",\"DraftUserID\":\"4EF6BE7E-9300-416D-B390-EBB859A6D05B\",\"DraftUserName\":\"曹节\"}]";
        private static readonly string TestJson3 = "[{\"TaskID\":\"14527974-4530-8ff5-45e3-da2ba60fe762\",\"ApplicationName\":\"xxx\",\"ProgramName\":\"xx\",\"TaskTitle\":\"xx\",\"ResourceID\":\"d876ea3a-72bd-b737-4aed-a156ddde7d2d\",\"SourceID\":\"4ef6be7e-9300-416d-b390-ebb859a6d05b\",\"SourceName\":\"xx\",\"SendToUserID\":\"cc431b22-2420-47f0-8166-0af2085255cd\",\"SendToUserName\":\"吴子兰\",\"Body\":\"\",\"Level\":3,\"ProcessID\":\"26447da1-9bf0-80b9-4721-07a3123e64e3\",\"ActivityID\":\"4c74af33-5f37-9f04-4af2-3e7840fa179d\",\"Url\":\"\",\"NormalizedUrl\":\"\",\"Emergency\":0,\"Purpose\":\"\",\"Status\":1,\"TaskStartTime\":\"2014-1-1\",\"ExpireTime\":\"2014-1-1\",\"DeliverTime\":\"2014-1-1\",\"ReadTime\":\"2014-1-1\",\"CategoryID\":\"14527974-4530-8ff5-45e3-da2ba60fe762\"}]";
        [TestMethod]
        public void JsonStringToList()
        {
            //List<UserTask> tasks = new List<UserTask>() { 
            //    new UserTask(){TASK_GUID=Guid.NewGuid().ToString(),TASK_TITLE="test",URL="http://www.baidu.com",READ_TIME=DateTime.MinValue},
            //    new UserTask(){TASK_GUID=Guid.NewGuid().ToString(),TASK_TITLE="test",URL="http://www.baidu.com",READ_TIME=DateTime.MinValue},
            //};

            //string jsonStr = JsonConvert.SerializeObject(tasks);

            List<UserTask> listObjs = JsonConvert.DeserializeObject<List<UserTask>>(TestJson2);


         //   object task = JsonConvert.DeserializeObject(TestJson2);

            Assert.IsTrue(1==1);
        }
    }
}
