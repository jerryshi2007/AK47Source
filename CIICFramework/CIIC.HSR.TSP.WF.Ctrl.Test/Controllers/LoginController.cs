using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.Services;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.Ctrl.Test.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            Account account = new Account() { Name = "willa", password = "111111", TenantCode = "Test1" };
            account.ReturnUrl = returnUrl;
            return View(account);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(Account account)
        {
            IAALoginBizlet loginBizlet=Containers.Global.Singleton.Resolve<IAALoginBizlet>();
            OperateResult result = null;
            System.Configuration.ClientSettingsSection section = ConfigurationManager.GetSection("applicationSettings/CIIC.HSR.TSP.WF.Ctrl.Test.Properties.Settings") as System.Configuration.ClientSettingsSection;
            SettingElement  sectionSetting= section.Settings.Get("CIIC_HSR_TSP_WF_Ctrl_Test_localhost_TaskPlugin");


            if (sectionSetting.Value.ValueXml.InnerText.IndexOf("CIIC.HSR.SSP.SspOguService") != -1)
              {                  
                  //通过 代办的接口调用自动适应 OMP 或SSP
                  result = loginBizlet.LoginForTenant(account.Name,account.password,account.TenantCode);
              }
              else 
              {
                   
                  result = loginBizlet.Login(account.Name, account.password);
              } 

            if (true||result.IsSuccess)
            {
                FormsAuthentication.SetAuthCookie(account.Name, true);
                IPermissionManagementBizlet aaUserBizlet = Containers.Global.Singleton.Resolve<IPermissionManagementBizlet>();
                ILoginSuccessContract loginSuccessBizlet = Containers.Global.Singleton.Resolve<ILoginSuccessContract>();
                var profileCode = CIIC.HSR.SSP.Tenants.TenantsUtilities.GetTenantByTenantCode(account.TenantCode);
                loginSuccessBizlet.OnLoginSuccess(aaUserBizlet.GetAAUserByAccount(account.Name).UserId, account.TenantCode, profileCode.ProfileCode);

                string returnUrl = FormsAuthentication.DefaultUrl;

                if (!string.IsNullOrEmpty(account.ReturnUrl))
                {
                    returnUrl = account.ReturnUrl;
                }
                
                return new RedirectResult(returnUrl);
            }
            else
            {
                ModelState.AddModelError("LoginKey", "登陆失败：" + result.Message);
                return View(account);
            }
        }
        public ActionResult SignOff()
        {
            FormsAuthentication.SignOut();
            return new RedirectResult(FormsAuthentication.LoginUrl);
        }
    }
}