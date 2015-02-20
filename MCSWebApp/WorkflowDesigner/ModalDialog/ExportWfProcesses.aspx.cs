using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MCS.Library.SOA.DataObjects;
using System.Xml.Linq;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using System.Text;

namespace WorkflowDesigner.ModalDialog
{
	public partial class ExportWfProcesses : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string sProcessKeys = Request["wfProcessKeys"].ToString();
			string[] arrProcessKeys = sProcessKeys.Split(',');

			var xDoc = new XDocument(
				new XDeclaration("1.0", "utf-8", "true"),
				new XElement("WorkflowProcesses")
				);

			for (int i = 0; i < arrProcessKeys.Length; i++)
			{
				XElementFormatter formatter = new XElementFormatter();

				formatter.OutputShortType = false;
				WfProcessDescriptor processDesc = (WfProcessDescriptor)WfProcessDescriptorManager.LoadDescriptor(arrProcessKeys[i]);
				XElement xeWfProcess = formatter.Serialize(processDesc);

				xDoc.Element("WorkflowProcesses").Add(xeWfProcess);
			}

			Response.ContentType = "text/xml";
			Response.ContentEncoding = Encoding.UTF8;
			if (arrProcessKeys.Count() > 1)
				Response.AppendHeader("content-disposition", "attachment;fileName=ExportedProcess.xml");
			else
				Response.AppendHeader("content-disposition",  string.Format("attachment;fileName={0}.xml", HttpUtility.UrlEncode(arrProcessKeys[0])));
			//"attachment;fileName=ExportedProcess.xml");

			xDoc.Save(Response.OutputStream);
		}
	}
}