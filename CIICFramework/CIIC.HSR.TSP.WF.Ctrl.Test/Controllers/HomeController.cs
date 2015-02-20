using CIIC.HSR.TSP.WF.Ctrl.Test.Models;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCS.Library.WF.Contracts.Ogu;
using Newtonsoft.Json;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.Services;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using CIIC.HSR.TSP.WF.UI.Control.Acl;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using MCS.Library.WcfExtensions;
using System.IO;
using CIIC.HSR.TSP.WebComponents.Extensions;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult StartWorkflow()
        {
            //  IoCConfig.Start();
            // CIIC.HSR.TSP.TA.BizObject.BizCodeBO bo = new TA.BizObject.BizCodeBO() { BizCode = "codetest", BizCodeName ="AME"};


            Expense expense = new Expense() { Amount = 100, Department = "980000", Name = "安永杰", TransitionDate = DateTime.Now };
            return View(expense);
        }
        [WFAclAuthorize]
        public ActionResult MoveTo()
        {
            Expense expense = new Expense() { Amount = 101, Department = "IT", Name = "测试名称", TransitionDate = DateTime.Now };
            return View(expense);
        }
        public ActionResult StackWorkflow()
        {
            return View();
        }
        /// <summary>
        /// 工作流
        /// </summary>
        /// <param name="paras">流程上下文参数</param>
        /// <param name="expense">报销费用</param>
        /// <returns>json数据</returns>
        [HttpPost]
        public ActionResult StartWorkflow(WFStartWorkflowParameter paras, Expense expense)
        {


            //代办标题
            paras.TaskTitle = "流程启动了流程意见列表测试";
            paras.BusinessUrl = Url.Action("MoveTo", "home");
            paras.ResourceId = System.Guid.NewGuid().ToString();

            paras.DepartmentCode = "testDepartmentCode";
            paras.DepartmentName = "部门名称";

            if (string.Equals(paras.TemplateKey, "TestBranch"))
            {
                List<WfClientUser> list = new List<WfClientUser>();
                list.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
                list.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));
                list.Add(new WfClientUser("0AD5CBB0-B3DC-4BC7-A192-D0BFB8F1BBD0", "王慧"));

                paras.ProcessStartupParams.ApplicationRuntimeParameters["b1"] = list; //增加审批人
            }
            //加入流程参数
            paras.ProcessStartupParams.ApplicationRuntimeParameters["amount"] = expense.Amount;

            if (string.Equals(paras.TemplateKey, "testMatrix"))
            {
                List<WfClientUser> rm = new List<WfClientUser>();
                rm.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
                rm.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));

                paras.ProcessStartupParams.ApplicationRuntimeParameters["r1"] = rm; //增加审批人
            }
            else
            {


                List<WfClientUser> r1 = new List<WfClientUser>();
                r1.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
                r1.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));


                List<WfClientUser> r2 = new List<WfClientUser>();
                r2.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
                r2.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));


                paras.ProcessStartupParams.ApplicationRuntimeParameters["R1"] = r1;//huanglan

                paras.ProcessStartupParams.ApplicationRuntimeParameters["R2"] = r2;
                //new WfClientUser("597E1761-97DD-4216-ADAA-B16850594C96", "王玉梅");//wangym


                paras.ProcessStartupParams.ApplicationRuntimeParameters["R3"] = new WfClientUser("0AD5CBB0-B3DC-4BC7-A192-D0BFB8F1BBD0", "王慧");//willa
            }
            //此处是测试时用流程暂存业务数据，实际开发中不要模仿，真实情况应该是存在业务数据库中  
            paras.ProcessStartupParams.ProcessContext["bizData"] = JsonConvert.SerializeObject(expense);
            //paras.ProcessStartupParams.Assignees.Add(new WfClientUser { ID = "D1F1BCE2-F848-48BF-A8FF-0592D13DF29B", Name = "XX" });

            //在线上配置的审批结构Code，如果未配置，则是名称，如“拒绝”、”同意“等
            //if (paras.ActionResult == "0001")
            //{
            //    //OMP可以不设置此属性，默认就是false
            //    //SSP设置为true
            paras.EMailCollector.IsTenantMode = true;
            //    paras.ProcessStartupParams.ApplicationRuntimeParameters["Content"] = "测试内容";
            //    //流程完成时，预警类型的编码
            //    paras.EMailCollector.MailCompletedArguments.AlarmTypeCode = "ANUnitTest03";
            //    //待办发送时，预警类型的编码
            //    paras.EMailCollector.MailTaskArguments.AlarmTypeCode = "ANUnitTest03";
            //}

            //待办发送时，预警相关的模板参数，可以加多个
            //paras.EMailCollector.MailTaskArguments.TemplateKeyValues.Add("Content", "测试内容");

            //执行流程启动操作
            ResponseData data = paras.Execute();

            //处理客户端返回数据
            data.BusinessData = expense;//业务数据

            return Json(data);
        }

        [HttpPost]
        public ActionResult SaveWorkflow(WFSaveParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);

        }

        [HttpPost]
        public ActionResult CancelWorkflow(WFCancelParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);
        }

        [HttpPost]
        public ActionResult PauseWorkflow(WFPauseParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);
        }


        [HttpPost]
        public ActionResult RestoreWorkflow(WFRestoreParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);
        }


        [HttpPost]
        public ActionResult ResumeWorkflow(WFResumeParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);
        }
        [HttpPost]
        public ActionResult WithdrawWorkflow(WFWithdrawParameter paras, Expense expense)
        {
            ResponseData data = paras.Execute();
            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;
            return Json(data);
        }

        [HttpPost]
        public ActionResult MoveTo(WFMoveToParameter paras, Expense expense)
        {
            //加入流程参数
            paras.SetParameter("amount", expense.Amount.ToString());
            WFUIRuntimeContext runtime = WFContextHelper.GetWFContext(this.Request);
          
            var a =  runtime.Process.ApplicationRuntimeParameters;
            
            int i = 0;
            if (a.ContainsKey("r1"))
            {
                i = i + 1;
            }
            if (a.ContainsKey("r2"))
            {
                i = i + 2;
            }
            if (a.ContainsKey("r3"))
            {
                i = i + 4;
            }

            List<WfClientUser> rm = new List<WfClientUser>();
            rm.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
         
            if(i==1)
            { 
                paras.RuntimeContext.ApplicationRuntimeParameters["r2"] = rm; //增加审批人
                paras.AddWfClientAssignee(rm);
            }
            if (i == 3)
            {
                rm.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));
                paras.RuntimeContext.ApplicationRuntimeParameters["r3"] = rm; //增加审批人
                paras.AddWfClientAssignee(rm);
            }

           
            //这里可以做流程操作执行前处理

            //执行流程处理
            ResponseData data = paras.Execute();

            //这里可以做流程操作执行后处理

            //设置返回客户端的业务数据，如果不需要，可以不设置
            data.BusinessData = expense;

            return Json(data);
        }

        public ActionResult TaskList()
        {
            TaskModel tasks = new TaskModel();
            using (DataModel context = new DataModel())
            {
                //localhost.TaskPlugin taskplguin = new localhost.TaskPlugin();
                //string userId = HttpContext.GetCurrentUserInfo().UserID.ToString();
                //string tenantCd = HttpContext.GetCurrentUserInfo().TenentCode;

                //localhost.UserTaskQueryCondition userConditionUncompleted = new localhost.UserTaskQueryCondition();
                //userConditionUncompleted.TaskType = localhost.TaskStatus.Unprocessed;

                //localhost.UserTaskQueryCondition userConditionCompleted = new localhost.UserTaskQueryCondition();
                //userConditionCompleted.TaskType = localhost.TaskStatus.Processed;


                //tasks.UncompletedTask = taskplguin.QueryTask(tenantCd, userId, userConditionUncompleted, 1, 100, null);
                //tasks.CompletedTask = taskplguin.QueryTask(tenantCd, userId, userConditionCompleted, 1, 100, null);



                ServiceFactory sf = new ServiceFactory();
                var readerService = sf.CreateService<ITaskPluginBizlet>();

                string userId = HttpContext.GetCurrentUserInfo().UserID.ToString();
                string tenantCd = HttpContext.GetCurrentUserInfo().TenentCode;


                UserTaskQueryCondition userConditionUncompleted = new UserTaskQueryCondition();
                userConditionUncompleted.TaskType = TaskStatus.Unprocessed;

                UserTaskQueryCondition userConditionCompleted = new UserTaskQueryCondition();
                userConditionCompleted.TaskType = TaskStatus.Processed;

                tasks.UncompletedTask = readerService.QueryTask(tenantCd, userId, userConditionUncompleted, 1, 100, null);
                tasks.CompletedTask = readerService.QueryTask(tenantCd, userId, userConditionCompleted, 1, 100, null);

            }

            return View(tasks);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CacheTask()
        {
            TaskModel tasks = new TaskModel();
            using (DataModel context = new DataModel())
            {

                string userId = HttpContext.GetCurrentUserInfo().UserID.ToString();
                string tenantCd = HttpContext.GetCurrentUserInfo().TenentCode;

                UserTaskQueryCondition userConditionUncompleted = new UserTaskQueryCondition();
                userConditionUncompleted.TaskType = TaskStatus.Unprocessed;

                UserTaskQueryCondition userConditionCompleted = new UserTaskQueryCondition();
                userConditionCompleted.TaskType = TaskStatus.Processed;


                ServiceFactory sf = new ServiceFactory();
                var readerService = sf.CreateService<ITaskPluginBizlet>();

                tasks.UncompletedTask = readerService.GetTaskFromCache("OMPUnprocessedTask", userId, tenantCd);
                tasks.CompletedTask = readerService.QueryTask(tenantCd, userId, userConditionCompleted, 1, 100, null);


            }

            return View(tasks);
        }

        #region Matrix流程
        [HttpGet]
        public ActionResult ExcelToDynamicProcess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExcelToDynamicProcess(HttpPostedFileBase[] files)
        {

            if (files == null || files.Length <= 0)
            {
                return View();
            }
            HttpPostedFileBase file = files[0];
            //校验文件类型 
            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return View();
            }
            string processKey = file.FileName.Replace(".xlsx", "");
            // EXCEL格式 参照 ~\CIIC.HSR.TSP.WF.Ctrl.Test\Content\a86cf8e8-0004-9972-457a-ce919355297e.xlsx 文件
           // string processKey = "a86cf8e8-0004-9972-457a-ce919355297e";//流程定义的KEY，应该由SSP提供
            //设置TenantCode
            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = "Test1"; //GetCurrentTenantCode();
            //保存流程定义
            WfClientProcessDescriptorServiceProxy.Instance.ExcelToSaveDescriptor(processKey, file.InputStream);//file.InputStream EXCEL文件流

            return RedirectToAction("StartDynamicProcess", "Home");
        }


        public ActionResult ExportDynamicProcess(string processKey)
        {
            Response.Clear();

            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", processKey + ".xlsx"));
            using (Stream sr = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey))
            {
                sr.CopyTo(Response.OutputStream);
            }
            Response.Flush();
            Response.End();
            return Json(string.Empty);

        }

        [HttpGet]
        public ActionResult StartDynamicProcess()
        {
            Expense expense = new Expense() { Amount = 5, Department = "IT", Name = "安永杰", TransitionDate = DateTime.Now };
            return View(expense);
        }

        [HttpPost]
        public ActionResult StartDynamicProcess(WFStartWorkflowParameter paras, Expense expense)
        {

            //代办标题
            paras.TaskTitle = "动态流程Matrix测试";
            paras.BusinessUrl = Url.Action("MoveTo", "home");
            paras.ResourceId = System.Guid.NewGuid().ToString();
            paras.DepartmentCode = "1001";
            paras.DepartmentName = "部门名称";

            if (string.Equals(paras.TemplateKey, "testMatrix"))
            {
                List<WfClientUser> rm = new List<WfClientUser>();
                rm.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));
                rm.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));

                paras.ProcessStartupParams.ApplicationRuntimeParameters["r1"] = rm; //增加审批人
            }

            //加入流程参数
            paras.ProcessStartupParams.ApplicationRuntimeParameters["amount"] = expense.Amount;
            List<WfClientUser> r2 = new List<WfClientUser>();
            r2.Add(new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽"));//huanglan
            r2.Add(new WfClientUser("4C07EB25-4415-47C5-B05D-B7FDB99161E4", "胡莹"));//huying1222 
            paras.ProcessStartupParams.ApplicationRuntimeParameters["MyManager"] = r2;


            paras.ProcessStartupParams.ApplicationRuntimeParameters["huanglan"] = new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽");
            //此处是测试时用流程暂存业务数据，实际开发中不要模仿，真实情况应该是存在业务数据库中  
            paras.ProcessStartupParams.ProcessContext["bizData"] = JsonConvert.SerializeObject(expense);


            //起始节点发代办，可以不设置,默认发给自己(默认流转到第一个节点，所以收到的为已办)
            // paras.ProcessStartupParams.Assignees.Add(new WfClientUser { ID = "A22C2C31-5C5E-40C4-ABB4-6196E580FCF8" });

            //执行流程启动操作
            ResponseData data = paras.Execute();

            //处理客户端返回数据
            data.BusinessData = expense;//业务数据

            return Json(data);
        }
        #endregion

        #region 界面更新
        private void SaveExpense(WfClientRuntimeContext runtimeContext, Expense expense)
        {
            runtimeContext.ApplicationRuntimeParameters["Amount"] = expense.Amount;
            runtimeContext.ProcessContext["Expense"] = JsonConvert.SerializeObject(expense);
        }

        [HttpPost]
        public ActionResult UpdateProcess(WFUpdateProcessParameter param, Expense expense)
        {
            try
            {
                param.RuntimeContext.ApplicationRuntimeParameters["Amount"] = expense.Amount;
                param.RuntimeContext.AutoCalculate = true;
                SaveExpense(param.RuntimeContext, expense);
                param.SetViewModel(expense);//如果PartialView使用的Model为主视图的View，则通过该方法设置
                //param.SetPartialViewModel("WFGraph", expense.ProcessId);//如果PartialView需要Model，则通过该方法设置 ，可以与 SetViewModel方法共存

                ResponseData data = param.Execute();//执行调用  

                //通过UpdateElementsHtml返回HTML信息，另外必须返回JsonSuccess格式的ActionResult                
                return this.JsonSuccess("True", param.UpdateElementsHtml);
            }
            catch (System.Exception ex)
            {
                return this.JsonError(ex.Message, ex);
            }
        }
        #endregion


        #region 这是安公公用于测试简易流程的对象测试，会迁移到单元测试中的
        /*
        public ActionResult BuildProcess(string processKey)
        {
            IWfMatrixProcess matrixProcess = new WfMatrixProcess();

            //流程基本信息
            matrixProcess.ApplicationName = "测试应用名称";
            matrixProcess.Description = "测试动态流程描述";
            matrixProcess.Key = processKey;
            matrixProcess.Name = "测试动态流程名称";
            matrixProcess.ProgramName = "测试程序名称";

            //添加步骤
            Guid step1Id = Guid.NewGuid();
            Guid step2Id = Guid.NewGuid();
            matrixProcess.Activities.Add(new WfMatrixActivity()
            {
                Id = step1Id,
                Code = "测试编码",
                Description = "节点描述1",
                Name = "节点1",
                Sort = 1
            });
            matrixProcess.Activities.Add(new WfMatrixActivity()
            {
                Id = step2Id,
                Code = "测试编码",
                Description = "节点描述1",
                Name = "节点1",
                Sort = 2
            });

            //设计节点的审批人
            IWfMatrixActivity activity1 = matrixProcess.Activities.GetById(step1Id);
            IWfMatrixActivity activity2 = matrixProcess.Activities.GetById(step2Id);

            activity1.Candidates.Add(new WfMatrixCandidate()
            {
                Candidate = new WfMatrixParameterDefinition()
                {
                    Description = "动态角色描述",
                    DisplayName = "直线汇报经理",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "MyManager",//动态角色,运行时刻设置人
                    ParameterType = ParaType.String
                },
                ID = Guid.NewGuid(),
                ResourceType = ResourceType.Variable
            });

            activity2.Candidates.Add(new WfMatrixCandidate()
            {
                Candidate = new WfMatrixParameterDefinition()
                {
                    Description = "审批人描述",
                    DisplayName = "审批人黄兰",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "huanglan",//直接设置的审批人
                    ParameterType = ParaType.String
                },
                ID = Guid.NewGuid(),
                ResourceType = ResourceType.User
            });

            //设置节点条件
            IWfMatrixConditionCollection conditionCollection = new WfMatrixConditionCollection();
            conditionCollection.Id = Guid.NewGuid();
            conditionCollection.Sort = 1;
            conditionCollection.Relation = LogicalRelation.And;
            conditionCollection.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });
            conditionCollection.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });

            IWfMatrixConditionCollection conditionCollection2 = new WfMatrixConditionCollection();
            conditionCollection2.Sort = 2;
            conditionCollection2.Id = Guid.NewGuid();
            conditionCollection2.Id = Guid.NewGuid();
            conditionCollection2.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });

            activity1.Expression.Relation = LogicalRelation.And;
            activity1.Expression.Conditions.Add(conditionCollection);
            activity1.Expression.Conditions.Add(conditionCollection2);

            //保存流程，业务需要保存流程key，以便再次加载时使用
            IWfMatrixStorageManager storageManager = new WfMatrixStorageManager();
            storageManager.Save(matrixProcess);


            Response.Clear();

            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", processKey + ".xlsx"));
            using (Stream sr = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey))
            {
                sr.CopyTo(Response.OutputStream);
            }
            Response.Flush();
            Response.End();
            return Json(string.Empty);
        }
         */
        #endregion

    }
}