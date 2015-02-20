using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;

namespace WorkflowDesigner.MatrixModalDialog
{
	/// <summary>
	/// ExportSOARolePropertyDefinitionHandler 的摘要说明
	/// </summary>
	public class ExportSOARolePropertyDefinitionHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			string roleID = context.Request["roleID"];
			if (string.IsNullOrEmpty(context.Request["roleID"]) == false)
			{
				SOARole role = new SOARole() { ID = roleID };

				SOARolePropertyDefinitionCollection rowsColl = SOARolePropertyDefinitionAdapter.Instance.LoadByRole(role);

				XDocument rolePropertiesDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), new XElement("SOARoleProperties"));

				XElementFormatter formatter = new XElementFormatter();

				formatter.OutputShortType = false;

				XElement xeRoleProperties = formatter.Serialize(rowsColl);

				rolePropertiesDoc.Element("SOARoleProperties").Add(xeRoleProperties);

				context.Response.Clear();
				context.Response.ContentType = "text/xml";
				context.Response.ContentEncoding = Encoding.UTF8;

				string fileName = string.Empty;

				if (context.Request["roleName"].IsNotEmpty())
					fileName = string.Format("{0}", context.Request["roleName"]);

				if (fileName.IsNullOrEmpty() && context.Request["roleCode"].IsNotEmpty())
					fileName = string.Format("{0}", context.Request["roleCode"]);

				if (fileName.IsNullOrEmpty())
					fileName = roleID;

				fileName += "_Properties";

				context.Response.AppendHeader("content-disposition", string.Format("attachment;fileName={0}.xml", context.Response.EncodeFileNameInContentDisposition(fileName)));
				rolePropertiesDoc.Save(context.Response.OutputStream);
				context.Response.End();
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