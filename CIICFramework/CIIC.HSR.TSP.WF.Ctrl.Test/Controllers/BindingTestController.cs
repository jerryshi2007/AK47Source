using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Controllers
{
    public class BindingTestController : Controller
    {
        // GET: BindingTest
        public ActionResult Index()
        {
            IWfMatrixProcess matrixProcess = new WfMatrixProcess();

            //流程基本信息
            matrixProcess.ApplicationName = "测试应用名称";
            matrixProcess.Description = "测试动态流程描述";
            matrixProcess.Key = Guid.NewGuid().ToString();
            matrixProcess.Name = "测试动态流程名称";
            matrixProcess.ProgramName = "测试程序名称";
            matrixProcess.TenantCode = "Test1";

            //添加步骤
            Guid step1Id = Guid.NewGuid();
            Guid step2Id = Guid.NewGuid();
            //matrixProcess.Activities.Add(new WfMatrixActivity()
            //{
            //    Id = step1Id,
            //    Code = "测试编码",
            //    Description = "节点描述1",
            //    Name = "节点1",
            //    Sort = 1
            //});
            //matrixProcess.Activities.Add(new WfMatrixActivity()
            //{
            //    Id = step2Id,
            //    Code = "测试编码",
            //    Description = "节点描述1",
            //    Name = "节点1",
            //    Sort = 2
            //});
            return View(matrixProcess);
        }
        [HttpPost]
        public ActionResult Index(IWfMatrixProcess maxtrixProcess)
        {
            return View(maxtrixProcess);
        }
    }
}