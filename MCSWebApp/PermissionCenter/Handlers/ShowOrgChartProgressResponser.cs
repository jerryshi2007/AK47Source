using MCS.Library.Core;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter.Handlers
{
    public class ShowOrgChartProgressResponser : IProcessProgressResponser
    {
        public static readonly ShowOrgChartProgressResponser Instance = new ShowOrgChartProgressResponser();

        private ShowOrgChartProgressResponser()
        {
        }

        public void Register(ProcessProgress progress)
        {
            progress.MinStep = 0;
            progress.MaxStep = 100;
            progress.CurrentStep = 0;
            progress.StatusText = string.Empty;
        }

        public void Response(ProcessProgress progress)
        {
            HttpResponse response = HttpContext.Current.Response;

            string script = string.Format("document.getElementById(\"statusText\").innerText=\"{0}\";document.getElementById(\"progressBar\").style[\"width\"] = \"{1}%\";",
                progress.StatusText,
                progress.CurrentStep * 100 / progress.MaxStep);

            script = string.Format("<script type=\"text/javascript\">{0}</script>", script);

            response.Write(script);

            response.Flush();
        }
    }
}