using System;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Web.Library;

namespace PermissionCenter.Handlers
{
	/// <summary>
	/// UserPhoto 的摘要说明
	/// </summary>
	public class UserPhoto : IHttpHandler
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
			string id = context.Request["id"];

			try
			{
				id.NullCheck("id");

				SchemaObjectPhoto photo = null;

				if (context.Request["time"] == "now")
				{
					TimePointContext tpc = TimePointContext.GetCurrentState();

					try
					{
						TimePointContext.Current.SimulatedTime = DateTime.MinValue;
						TimePointContext.Current.UseCurrentTime = true;

						photo = SchemaObjectAdapter.Instance.GetObjectPhoto(id, "PhotoKey", DateTime.MinValue);
					}
					finally
					{
						TimePointContext.RestoreCurrentState(tpc);
					}
				}
				else
				{
					photo = SchemaObjectAdapter.Instance.GetObjectPhoto(id, "PhotoKey", TimePointContext.Current.SimulatedTime);
				}

				if (photo != null)
					ResponsePhoto(WebUtility.GetContentTypeByFileName(photo.ImageInfo.OriginalName), photo.ContentData);
				else
					ReponseDefaultPhoto();
			}
			catch (System.Exception)
			{
				ReponseDefaultPhoto();
			}
		}
	
		private static void ResponsePhoto(string contentType, byte[] data)
		{
			HttpResponse response = HttpContext.Current.Response;

			response.ContentType = contentType;
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.BinaryWrite(data);
			response.End();
		}

		private static void ReponseDefaultPhoto()
		{
			HttpContext context = HttpContext.Current;

			string physicalPath = context.Server.MapPath("~/images/defaultphoto.jpg");
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.ContentType = "image/jpeg";
			context.Response.WriteFile(physicalPath);
		}
	}
}