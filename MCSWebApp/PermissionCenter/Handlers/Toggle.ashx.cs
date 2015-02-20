using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter.Handlers
{
	/// <summary>
	/// Toggle 的摘要说明
	/// </summary>
	public class Toggle : IHttpHandler
	{
		private static byte[] dummImgData = null;

		public void ProcessRequest(HttpContext context)
		{
			switch (context.Request.QueryString["tp"])
			{
				case "UserBrowseView":
					ProfileUtil.ToggleUserBrowseMode(int.Parse(context.Request.QueryString["i"]));
					this.ResponseDummyImage(context);
					break;
				default:
					break;
			}
		}

		private void ResponseDummyImage(HttpContext context)
		{
			if (dummImgData == null)
			{
				dummImgData = System.IO.File.ReadAllBytes(context.Server.MapPath("~/images/bgw.png"));
			}

			context.Response.ContentType = "image/png";
			using (var stream = context.Response.OutputStream)
			{
				stream.Write(dummImgData, 0, dummImgData.Length);
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