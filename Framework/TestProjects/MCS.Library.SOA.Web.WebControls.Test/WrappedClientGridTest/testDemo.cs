using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.Library.MVC;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public class testDemo
    {
    }


    class MyController
    {
        [ControllerMethod]
        private void TwoParamMethod(string name, string type)
        {
            HttpContext.Current.Response.Write(
                string.Format("Name={0}, Type={1}", HttpUtility.HtmlEncode(name), HttpUtility.HtmlEncode(type)));
        }


        [ControllerMethod]
        private void TwoParamMethod2(string name, int age)
        {
            HttpContext.Current.Response.Write(
                string.Format("Name={0}, Age={1}", HttpUtility.HtmlEncode(name), age));
        }
    }
}