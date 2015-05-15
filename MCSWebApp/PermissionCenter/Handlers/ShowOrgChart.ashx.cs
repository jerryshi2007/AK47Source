using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Web.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI.HtmlControls;

namespace PermissionCenter.Handlers
{
    /// <summary>
    /// Summary description for ShowOrgChart
    /// </summary>
    public class ShowOrgChart : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            ProcessProgress.Current.RegisterResponser(ShowOrgChartProgressResponser.Instance);

            using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.WriteLine("<!DOCTYPE html>");

                WriteProgressBar(writer);
            }

            OrgTreeNode root = BuildOguTree(WebUtility.GetRequestQueryString("ou", string.Empty));

            ResponseHideProgressContainerScript();

            using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
            {
                if (root != null)
                {
                    TreeGraph graph = root.GenerateGraph();
                    graph.WriteHtmlGraph(writer);
                }
            }
        }

        private static OrgTreeNode BuildOguTree(string ouID)
        {
            OrgTreeNode result = null;

            if (ouID.IsNotEmpty())
            {
                ProcessProgress.Current.MaxStep += 1;

                SCOrganization root = (SCOrganization)SchemaObjectAdapter.Instance.Load(ouID, TimePointContext.Current.SimulatedTime);

                if (root != null)
                    result = PrepareOguTree(root);
            }

            return result;
        }

        private static OrgTreeNode PrepareOguTree(SCOrganization organization)
        {
            OrgTreeNode node = new OrgTreeNode(organization);
            ProcessProgress.Current.Increment();
            ProcessProgress.Current.Response(string.Format("已经处理了{0}个组织", ProcessProgress.Current.CurrentStep));

            foreach (SchemaObjectBase obj in organization.CurrentChildren)
            {
                if (obj is SCOrganization)
                {
                    ProcessProgress.Current.MaxStep += 1;
                    node.Children.Add(PrepareOguTree((SCOrganization)obj));
                }
            }

            return node;
        }

        private static void ResponseHideProgressContainerScript()
        {
            HttpContext.Current.Response.ResponseWithScriptTag(writer =>
            {
                writer.WriteLine("document.getElementById(\"progressArea\").style[\"display\"] = \"none\";");
            });
        }

        private static void WriteProgressBar(TextWriter writer)
        {
            HtmlGenericControl container = new HtmlGenericControl("div");

            container.ID = "progressArea";

            HtmlGenericControl progressContainer = new HtmlGenericControl("div");

            container.Controls.Add(progressContainer);

            progressContainer.ID = "progressContainer";
            progressContainer.Style["width"] = "100%";
            progressContainer.Style["height"] = "20px";
            progressContainer.Style["border"] = "1px solid silver";

            HtmlGenericControl progressBar = new HtmlGenericControl("div");

            progressContainer.Controls.Add(progressBar);

            progressBar.ID = "progressBar";
            progressBar.Style["text-align"] = "left";
            progressBar.Style["width"] = "0%";
            progressBar.Style["height"] = "20px";
            progressBar.Style["background-color"] = "blue";

            HtmlGenericControl statusText = new HtmlGenericControl("div");

            container.Controls.Add(statusText);

            statusText.ID = "statusText";
            statusText.Style["text-align"] = "left";
            statusText.Style["padding-left"] = "8px";
            statusText.Style["padding-right"] = "8px";

            writer.Write(container.GetHtml());
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}