using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;

namespace AUCenter.Services
{
	/// <summary>
	/// 导出数据
	/// </summary>
	public class Export : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			var req = context.Request;
			if (req.HttpMethod == "POST" && req.IsAuthenticated)
			{
				AUObjectExportExecutor action = AUObjectExportExecutor.CreateAction(req.Form["context"]);

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

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}