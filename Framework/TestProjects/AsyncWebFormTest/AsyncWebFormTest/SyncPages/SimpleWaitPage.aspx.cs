using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AsyncWebFormTest.Common;
using System.Threading;

namespace AsyncWebFormTest.SyncPages
{
    public partial class SimpleWaitPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.WriteThreadInfo("PageLoad");

            int worker, io;

            ThreadPool.GetMaxThreads(out worker, out io);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Helper.WriteThreadInfo("PreRender");

            Thread.Sleep(5000);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            Helper.WriteThreadInfo("PreRenderComplete");
            base.OnPreRenderComplete(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Helper.WriteThreadInfo("Render");

            outputText.Text = Helper.GetOutputText();

            base.Render(writer);
        }
    }
}