using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Web.Library.Script;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Handlers
{
	/// <summary>
	/// 导出对象的HTTP处理器
	/// </summary>
	public class ObjectExport : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			var req = context.Request;
			if (req.HttpMethod == "POST" && req.IsAuthenticated)
			{
				ExportAction action = ExportAction.CreateAction(req.Form["context"]);

				SCObjectSet result = action.Execute(context.Request);

				var cateName = action.CategoryName;

				string fileName = cateName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
				if (TimePointContext.Current.UseCurrentTime == false)
					fileName += "_" + TimePointContext.Current.SimulatedTime.ToString("yyyyMMdd_HHmmss");

				fileName += ".xml";
				context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + MCS.Web.Library.ResponseExtensions.EncodeFileNameInContentDisposition(context.Response, fileName) + "\"");
				using (var output = context.Response.Output)
				{
					result.TimeContext = TimePointContext.Current.UseCurrentTime ? DateTime.Now : TimePointContext.Current.SimulatedTime;
					result.Write(output);
				}
			}
			else
			{
				throw new HttpException("请求的方式错误");
			}
		}
	}
}