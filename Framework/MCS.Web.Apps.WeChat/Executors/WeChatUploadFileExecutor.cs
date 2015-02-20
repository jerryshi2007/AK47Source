using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatUploadFileExecutor : WeChatAuthenticatedExecutorBase
	{
		private static string Boundary = "----------Ef1GI3gL6Ef1ei4Ij5Ij5cH2Ef1KM7";

		public WeChatUploadFileExecutor(string filePath, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			filePath.CheckStringIsNullOrEmpty("filePath");

			this.FilePath = filePath;
		}

		public string FilePath
		{
			get;
			private set;
		}

		public WeChatUploadFileRetInfo UploadedFileInfo
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["action"] = "upload_material";
			parameters["f"] = "json";
			parameters["ticket_id"] = this.LoginInfo.AccountID;
			parameters["ticket"] = this.LoginInfo.Ticket;
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh_CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/filetransfer?" + parameters.ToUrlParameters(true); ;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;

			request.Method = "POST";
			request.UserAgent = "Shockwave Flash";
			request.Accept = "text/*";
			request.ContentType = string.Format("multipart/form-data; boundary={0}", Boundary);
			request.Headers["DNT"] = "1";
			request.ServicePoint.Expect100Continue = false;
			request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

			string fileName = Path.GetFileName(this.FilePath);

			using (MemoryStream stream = new MemoryStream())
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine("--" + Boundary);
					writer.WriteLine("Content-Disposition: form-data; name=\"Filename\"");
					writer.WriteLine();
					writer.WriteLine(fileName);

					writer.WriteLine("--" + Boundary);
					writer.WriteLine("Content-Disposition: form-data; name=\"folder\"");
					writer.WriteLine();
					writer.WriteLine("/cgi-bin/uploads");

					writer.WriteLine("--" + Boundary);
					writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", fileName);
					writer.WriteLine("Content-Type: application/octet-stream");

					writer.WriteLine();

					writer.Flush();

					using (Stream fileStream = LoadFile(this.FilePath))
					{
						Byte[] data = fileStream.ToBytes();
						stream.Write(data, 0, data.Length);
					}

					writer.WriteLine();
					writer.WriteLine("--" + Boundary);
					writer.WriteLine("Content-Disposition: form-data; name=\"Upload\"");
					writer.WriteLine();

					writer.WriteLine("Submit Query");
					writer.Write("--" + Boundary + "--");

					writer.Flush();

					request.ContentLength = stream.Length;
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(request.GetRequestStream());
				}
			}

			return request;
		}

		private static Stream LoadFile(string filePath)
		{
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}

		protected override void ProcessResponseText(string responseText)
		{
			WeChatUploadFileRetInfo retInfo = JSONSerializerExecute.Deserialize<WeChatUploadFileRetInfo>(responseText);

			retInfo.CheckResult();

			this.UploadedFileInfo = retInfo;
		}
	}
}
