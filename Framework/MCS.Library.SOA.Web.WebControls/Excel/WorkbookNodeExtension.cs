using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Excel workbook功能扩展类
	/// </summary>
	public static class WorkbookNodeExtension
	{
		/// <summary>
		/// 输出到Http的Response中。从UserSettings中读取文档类型和扩展名。
		/// </summary>
		/// <param name="workbook">WorkbookNode对象</param>
		/// <param name="fileNameWithoutExt">不带扩展名的文件名</param>
		public static void Response(this WorkbookNode workbook, string fileNameWithoutExt)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(workbook != null, "workbook");

			HttpResponse response = HttpContext.Current.Response;

			response.AddExcelXmlHeader(fileNameWithoutExt);

			response.Clear();
			workbook.Save(response.OutputStream);
			response.End();
		}

		/// <summary>
		/// 输出ExcelXml格式的Header
		/// </summary>
		/// <param name="response"></param>
		public static void AddExcelXmlHeader(this HttpResponse response, string fileNameWithoutExt)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(response != null, "response");

			string fileType = "text/xml";
			string fileExt = "xml";

			if (DeluxePrincipal.IsAuthenticated)
			{
				fileType = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue("CommonSettings", "downExcelXmlContentType", fileType);
				fileExt = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue("CommonSettings", "downExcelXmlFileExt", fileExt);
			}

			response.ContentType = fileType;
			response.AppendHeader("CONTENT-DISPOSITION",
						string.Format("{0};filename={1}", "inline", response.EncodeFileNameInContentDisposition(fileNameWithoutExt + "." + fileExt)));
		}
	}
}
