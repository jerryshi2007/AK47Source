using System.Threading;
using System.Web;

namespace PermissionCenter.Handlers
{
	/// <summary>
	/// AdSync 的摘要说明
	/// </summary>
	public class AdSync : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "image/png";
			Thread.Sleep(10000);
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