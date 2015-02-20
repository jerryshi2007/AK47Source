using MCS.Web.Library;
using MCS.Web.Responsive.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace MCSResponsiveOAPortal
{
    public partial class Drafting : System.Web.UI.Page
    {
        private static readonly string[] root = new string[] { "Root" };
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string json = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(GetResult(root)[0]);
            this.navPanel.Attributes["data-initial-data"] = json;
        }

        public override void ProcessRequest(HttpContext context)
        {
            string[] paths = context.Request.QueryString.GetValues("path");
            if (paths == null || paths.Length == 0)
            {
                base.ProcessRequest(context);
            }
            else
            {
                ArrayList result = GetResult(paths);

                context.Response.ContentType = "application/json";

                string json = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(result);

                context.Response.Output.Write(json);
            }
        }

        protected string InitalJsonFor(string name)
        {
            var doc = WebXmlDocumentCache.GetDocument("~/App_Data/DraftingData.xml");
            ArrayList list = new ArrayList();
            if (string.IsNullOrEmpty(name) == false)
            {
                CategoryLinkAdapter.Instance.DoSearch(new string[] { "Root", name }, list, System.Xml.XmlReader.Create(new StringReader(doc)));
            }

            return MCS.Web.Library.Script.JSONSerializerExecute.Serialize(list);
        }

        private ArrayList GetResult(string[] paths)
        {
            ArrayList result = new ArrayList();

            var doc = WebXmlDocumentCache.GetDocument("~/App_Data/DraftingData.xml");
            var schemaDoc = WebXmlDocumentCache.GetDocument("~/App_Data/CategorySchema.xsd");

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var parts = path.Split('.', '>');
                ArrayList list = new ArrayList();
                if (parts.Length > 0 && parts[0] == "Root")
                {
                    CategoryLinkAdapter.Instance.DoSearch(parts, list, XmlReader.Create(new StringReader(doc), Util.GetDraftingLinkXmlReaderSettings()));
                }

                result.Add(list);
            }

            return result;
        }
    }
}