using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mime;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects.Workflow.Exporters;
using System.IO;

namespace WorkflowDesigner.ModalDialog
{
	/// <summary>
	/// Summary description for ExportWfProcessAsZip
	/// </summary>
	public class ExportWfProcessAsZip : IHttpHandler
	{
		//public void ProcessRequest(HttpContext context)
		//{
		//    var roleMode = context.Request.QueryString["roleAsPerson"] == "true" ? true : false;
		//    string[] keys = context.Request["wfProcessKeys"].Split(',');

		//    var bytes = new ZipPackageExporter(keys, roleMode).Export();

		//    context.Response.Clear();
		//    //content type 搞成 xml 为了不弹空页
		//    context.Response.ContentType = MediaTypeNames.Text.Xml;
		//    if (keys.Count() > 1)
		//        context.Response.AppendHeader("content-disposition", string.Format("attachment;fileName= ProcessDescription{0}.zip", DateTime.Now.ToString("yyyy-MM-dd")));
		//    else
		//        context.Response.AppendHeader("content-disposition", string.Format("attachment;fileName={0}.zip", HttpUtility.UrlEncode(keys[0])));

		//    context.Response.BinaryWrite(bytes);
		//    context.Response.End();
		//}

		public void ProcessRequest(HttpContext context)
		{
			var roleMode = context.Request.QueryString["roleAsPerson"] == "true" ? true : false;
			string[] keys = context.Request["wfProcessKeys"].Split(',');

			context.Response.Clear();
			//content type 搞成 xml 为了不弹空页
			context.Response.ContentType = "application/zip";

			if (keys.Count() > 1)
				context.Response.AppendHeader("content-disposition", string.Format("attachment;fileName= ProcessDescription{0}.zip", DateTime.Now.ToString("yyyy-MM-dd")));
			else
				context.Response.AppendHeader("content-disposition", string.Format("attachment;fileName={0}.zip", HttpUtility.UrlEncode(keys[0])));

			byte[] bytes = WfProcessExporter.ExportProcessDescriptors(new WfExportProcessDescriptorParams(roleMode), keys);
			context.Response.BinaryWrite(bytes);

			context.Response.End();
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