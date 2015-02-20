using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Controllers
{
    public class HtmlController : Controller
    {

       
        // GET: Html
          [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}