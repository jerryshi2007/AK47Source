using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Responsive.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls.Test.TreeGraphTest
{
    public partial class GenerateOrgTree : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            OrgTreeNode root = OrgTreeNode.PreareOrgTree();

            TreeGraph graph = root.GenerateGraph();

            StringBuilder strB = new StringBuilder();

            using (StringWriter writer = new StringWriter(strB))
            {
                graph.WriteHtmlGraph(writer);
            }

            this.container.InnerHtml = strB.ToString();

            base.OnPreRender(e);
        }
    }
}