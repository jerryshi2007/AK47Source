using MCS.Library.Data.DataObjects;
using MCS.Web.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.Web.WebControls.Test.TreeGraphTest
{
    /// <summary>
    /// Summary description for ResponseOrgTree
    /// </summary>
    public class ResponseOrgTree : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.WriteLine("<!DOCTYPE html>");

                OrgTreeNode root = OrgTreeNode.PreareOrgTree();

                TreeGraph graph = root.GenerateGraph();

                graph.WriteHtmlGraph(writer);
            }
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