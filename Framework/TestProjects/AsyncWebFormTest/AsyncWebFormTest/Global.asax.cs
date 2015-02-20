using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace AsyncWebFormTest
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            int worker, io;

            ThreadPool.GetMaxThreads(out worker, out io);

            ThreadPool.SetMaxThreads(2, io);
        }
    }
}