using CIIC.HSR.TSP.WebComponents.Extensions;
using CIIC.HSR.TSP.WF.UI.Control;
using CIIC.HSR.TSP.WF.UI.Control.Acl;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using CIICWFControlTest.Models;
using MCS.Library.Principal;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIICWFControlTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult StartWorkflow()
        {
            Expense expense = new Expense()
            {
                ID = Guid.NewGuid().ToString(),
                Amount = 500,
                Department = DeluxeIdentity.CurrentUser.TopOU.DisplayName,
                Name = DeluxeIdentity.CurrentUser.DisplayName,
                TransitionDate = DateTime.Now
            };

            return View(expense);
        }

        [HttpPost]
        public ActionResult StartWorkflow(WFStartWorkflowParameter param, Expense expense)
        {
            return this.JsonExecute(() =>
                {
                    //设置当前操作用户
                    param.ProcessStartupParams.Creator = (WfClientUser)DeluxeIdentity.CurrentUser.ToClientOguObject();

                    param.BusinessUrl = Url.Action("MoveTo", "home");

                    param.ProcessStartupParams.ResourceID = expense.ID;
                    //加入流程参数
                    param.ProcessStartupParams.ApplicationRuntimeParameters["Amount"] = expense.Amount;
                    param.ProcessStartupParams.ApplicationRuntimeParameters["RequestorName"] = expense.Name;
                    param.ProcessStartupParams.ProcessContext["Expense"] = JsonConvert.SerializeObject(expense);

                    param.ProcessStartupParams.ApplicationRuntimeParameters["Subject"] = expense.Name;

                    ResponseData data = param.Execute((ex) => true);

                    return Json(data);
                });
        }

        [WFAclAuthorize]
        public ActionResult MoveTo()
        {
            Expense expense = new Expense() { Amount = 0, Department = "IT", Name = "测试名称", TransitionDate = DateTime.Now };

            WFUIRuntimeContext runtime = this.Request.GetWFContext();

            if (runtime != null && runtime.Process != null)
            {
                if (runtime.Process.ProcessContext.ContainsKey("Expense"))
                {
                    string serilizedData = (string)runtime.Process.ProcessContext["Expense"];

                    expense = JsonConvert.DeserializeObject<Expense>(serilizedData);
                }
            }

            return View(expense);
        }

        [HttpPost]
        public ActionResult MoveTo(WFMoveToParameter param, Expense expense)
        {
            return this.JsonExecute(() =>
            {
                SaveExpense(param.RuntimeContext, expense);

                ResponseData data = param.Execute((ex) => true);

                return Json(data);
            });
        }

        [HttpPost]
        public ActionResult Save(WFSaveParameter param, Expense expense)
        {
            return this.JsonExecute(() =>
            {
                SaveExpense(param.RuntimeContext, expense);

                ResponseData data = param.Execute();

                return Json(data);
            });
        }

        [HttpPost]
        public ActionResult UpdateProcess(WFUpdateProcessParameter param, Expense expense)
        {
            try
            {
                param.RuntimeContext.ApplicationRuntimeParameters["Amount"] = expense.Amount;

                List<WfClientUser> approvers = new List<WfClientUser>();

                approvers.Add(this.HttpContext.Request.GetWFContext().CurrentUser);

                param.RuntimeContext.ApplicationRuntimeParameters["Approvers"] = approvers;
                param.RuntimeContext.AutoCalculate = true;

                SaveExpense(param.RuntimeContext, expense);

                ResponseData data = param.Execute();

                HttpContext.Request.ReloadWFContext();

                return PartialView("WFToolbar");
            }
            catch (System.Exception ex)
            {
                return this.JsonError(ex.Message, ex);
            }
        }

        private void SaveExpense(WfClientRuntimeContext runtimeContext, Expense expense)
        {
            runtimeContext.ApplicationRuntimeParameters["Amount"] = expense.Amount;
            runtimeContext.ProcessContext["Expense"] = JsonConvert.SerializeObject(expense);
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
    }
}