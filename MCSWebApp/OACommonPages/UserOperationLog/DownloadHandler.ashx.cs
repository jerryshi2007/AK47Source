using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using System.IO;

namespace MCS.OA.CommonPages.UserOperationLog
{
	/// <summary>
	/// 下载上传文件
	/// </summary>
	public sealed class DownloadHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			HttpResponse Response = context.Response;
			try
			{
				int id;
				if (int.TryParse(context.Request.Params["id"], out id))
				{
					UploadFileHistory uploadFilelog = UploadFileHistoryAdapter.Instance.Load(id);
					if (uploadFilelog != null)
					{
						Response.Clear();
						Response.ClearHeaders();
						using (Stream stream = uploadFilelog.GetMaterialContentStream())
						{
							stream.CopyTo(Response.OutputStream);
						}

					   Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					   Response.AppendHeader("CONTENT-DISPOSITION",
						string.Format("{0};filename={1}", "inline", HttpUtility.UrlEncode(uploadFilelog.CurrentFileName)));
					}
				}
			}
			catch (Exception ex)
			{
				Response.Write("下载出错 : " + ex.Message);
			}
			finally
			{
				Response.End();
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}

	}
}