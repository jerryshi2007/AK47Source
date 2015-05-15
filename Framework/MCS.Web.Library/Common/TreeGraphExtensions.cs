using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MCS.Web.Library
{
    /// <summary>
    /// TreeGraph的扩展
    /// </summary>
    public static class TreeGraphExtensions
    {
        private static Dictionary<TreeGraphNodeType, string> ConnectorImages = new Dictionary<TreeGraphNodeType, string>()
        {
            { TreeGraphNodeType.Cross, ImageContainer.CrossUrl },
            { TreeGraphNodeType.HLine, ImageContainer.HLineUrl },
            { TreeGraphNodeType.VLine, ImageContainer.VLineUrl },
            { TreeGraphNodeType.LeftCorner, ImageContainer.LeftCornerUrl },
            { TreeGraphNodeType.RightCorner, ImageContainer.RightCornerUrl},
            { TreeGraphNodeType.Tee, ImageContainer.TeeUrl },
            { TreeGraphNodeType.ReverseTee, ImageContainer.ReverseTeeUrl }
        };

        /// <summary>
        /// 注册树型图相关的样式
        /// </summary>
        public static void RegisterInlineStyles()
        {
            string style = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.Library.Resources.orgStyle.css");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("\n<style type=\"text/css\">{0}</style>\n", style);

            Page page = GetPage();

            page.ClientScript.RegisterClientScriptBlock(typeof(TreeGraphExtensions), "OrgTreeStyle", strB.ToString());
        }

        /// <summary>
        /// 输出树型图相关的样式
        /// </summary>
        public static void ResponseInlineStyles()
        {
            string style = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.Library.Resources.orgStyle.css");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("\n<style type=\"text/css\">{0}</style>\n", style);

            HttpContext.Current.NullCheck("HttpContext");

            HttpContext.Current.Response.Write(strB.ToString());
        }

        /// <summary>
        /// 输出Inline的样式
        /// </summary>
        /// <param name="writer"></param>
        public static void WriteInlineStyles(TextWriter writer)
        {
            string style = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.Library.Resources.orgStyle.css");

            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("\n<style type=\"text/css\">{0}</style>\n", style);

            writer.Write(strB.ToString());
        }

        private static Page GetPage()
        {
            Page page = WebUtility.GetCurrentPage();

            if (page == null)
                page = new Page();

            return page;
        }

        /// <summary>
        /// 输出树型图
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="writer"></param>
        /// <param name="outputStyle">是否输出样式，默认为true</param>
        public static void WriteHtmlGraph(this TreeGraph graph, TextWriter writer, bool outputStyle = true)
        {
            if (graph != null && writer != null)
            {
                if (outputStyle)
                    WriteInlineStyles(writer);

                writer.WriteLine("\n<div style=\"width: {0}px; height: {1}px; white-space: nowrap; overflow: hidden; margin: 0px 0px 0px 0px;\">",
                    graph.Width * 120 + 1, (graph.Height + 1) * 60);

                GenerateHtmlRow(writer, new TreeGraphNodeBase[] { graph.Root }, graph.Width);

                writer.WriteLine("\n</div>");
            }
        }

        private static void GenerateHtmlRow(TextWriter writer, IEnumerable<TreeGraphNodeBase> graphNodes, int width)
        {
            if (GenerateOneHtmlRow(writer, graphNodes, width))
            {
                List<TreeGraphNodeBase> nextRow = new List<TreeGraphNodeBase>();

                foreach (TreeGraphNodeBase childNode in graphNodes)
                    childNode.Children.CopyTo(nextRow);

                GenerateHtmlRow(writer, nextRow, width);
            }
        }

        private static bool GenerateOneHtmlRow(TextWriter writer, IEnumerable<TreeGraphNodeBase> graphNodes, int width)
        {
            if (graphNodes.Count() == 0)
                return false;

            Dictionary<int, TreeGraphNodeBase> graphNodesDict = new Dictionary<int, TreeGraphNodeBase>();

            graphNodes.ForEach(g => graphNodesDict[g.X] = g);

            writer.WriteLine();
            writer.WriteLine("<div style=\"margin: 0px 0px 0px 0px;\">");

            for (int i = 0; i < width; i++)
            {
                TreeGraphNodeBase matchedGraphNode = null;

                Control cell = null;
                if (graphNodesDict.TryGetValue(i, out matchedGraphNode))
                    cell = CreateMatchedCell(matchedGraphNode);
                else
                    cell = CreatePlaceHolderCell();

                writer.Write(cell.GetHtml());
            }

            writer.WriteLine("</div>");

            return true;
        }

        private static Control CreateMatchedCell(TreeGraphNodeBase graphNode)
        {
            Control result = null;

            if (graphNode is TreeGraphLabelNode)
                result = CreateLabelCell((TreeGraphLabelNode)graphNode);
            else
                if (graphNode is TreeGraphConnectorNode)
                    result = CreateConnectorCell((TreeGraphConnectorNode)graphNode);

            return result;
        }

        private static Control CreateConnectorCell(TreeGraphConnectorNode connector)
        {
            HtmlGenericControl imageContainer = new HtmlGenericControl("div");

            imageContainer.Attributes["class"] = "org-node";

            Image image = new Image();

            image.Attributes["class"] = "org-connector";
            image.ImageUrl = GetConnectorImageUrl(connector.NodeType);

            imageContainer.Controls.Add(image);

            return imageContainer;
        }

        private static string GetConnectorImageUrl(TreeGraphNodeType nodeType)
        {
            string result = string.Empty;

            ConnectorImages.TryGetValue(nodeType, out result);

            return result;
        }

        private static Control CreateLabelCell(TreeGraphLabelNode label)
        {
            HtmlGenericControl labelNode = new HtmlGenericControl("div");

            labelNode.Attributes["class"] = "org-node";

            HtmlGenericControl labelContainer = new HtmlGenericControl("div");
            labelNode.Controls.Add(labelContainer);

            labelContainer.Attributes["class"] = "org-label";

            HtmlGenericControl textArea = new HtmlGenericControl("div");
            textArea.Attributes["class"] = "org-label-text-area";

            labelContainer.Controls.Add(textArea);

            HtmlGenericControl text = new HtmlGenericControl("div");

            text.Attributes["class"] = "org-label-text";
            text.Attributes["title"] = label.Name;
            text.InnerText = label.Name;
            textArea.Controls.Add(text);

            HtmlGenericControl descriptionArea = new HtmlGenericControl("div");

            descriptionArea.Attributes["class"] = "org-label-description_area";
            labelContainer.Controls.Add(descriptionArea);

            HtmlGenericControl description = new HtmlGenericControl("div");

            description.Attributes["class"] = "org-label-description";
            description.Attributes["title"] = label.Description;
            description.InnerText = label.Description;
            descriptionArea.Controls.Add(description);

            return labelNode;
        }

        private static Control CreatePlaceHolderCell()
        {
            HtmlGenericControl cellDiv = new HtmlGenericControl("div");

            cellDiv.Attributes["class"] = "org-node org-placeholder";
            return cellDiv;
        }
    }
}
