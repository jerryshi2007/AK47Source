using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AsyncWebFormTest.Common;

namespace AsyncWebFormTest.AsyncPages
{
    public partial class SimpleWaitPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.WriteThreadInfo("PageLoad");

            RegisterAsyncTask(new PageAsyncTask(GetDelayAsyncTask));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Helper.WriteThreadInfo("PreRender");
        }

        private async Task GetDelayAsyncTask()
        {
            await Task.Delay(5000);

            Helper.WriteThreadInfo("TaskEnd");
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