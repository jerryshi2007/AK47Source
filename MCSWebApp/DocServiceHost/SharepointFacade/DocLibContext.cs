using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using System.Net;
using DocServiceHost.Configuration;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DocServiceContract.Exceptions;

namespace MCS.Library.Services
{
	/// <summary>
	/// 文档库上下文
	/// </summary>
	public class DocLibContext : ClientContext
	{
		private string baseListName;

		public DocLibContext()
			: this(ServiceHelper.GetConfiguration().DocumentLibraryName)
		{
		}

		public DocLibContext(string baseListName)
			: base(ServiceHelper.GetConfiguration().ServerName)
		{
			this.baseListName = baseListName;

			MCS.Library.Core.ServerInfo serverInfo = ServiceHelper.GetConfiguration().ToServerInfo();

			this.Credentials = new NetworkCredential(serverInfo.Identity.LogOnNameWithoutDomain,
				serverInfo.Identity.Password, serverInfo.Identity.Domain);
		}

		public static Uri BaseUri
		{
			get
			{
				return MossServerInfoConfigurationSettings.GetConfig().Servers["documentServer"].BaseUri;
			}
		}

		/// <summary>
		/// 基本列表
		/// </summary>
		public List BaseList
		{
			get
			{
				return this.Web.Lists.GetByTitle(baseListName);
			}
		}

		/// <summary>
		/// 根据Uri得到二进制流
		/// </summary>
		/// <param name="serverRelativeUrl">相对Url</param>
		/// <returns></returns>
		public byte[] OpenBinary(string serverRelativeUrl)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Credentials = this.Credentials;
				string url = UriHelper.CombinePath(this.Url, serverRelativeUrl);

				return webClient.DownloadData(EncodeUrl(url));
			}
		}

		public string EncodeUrl(string url)
		{
			string start = url.Replace("http://", "");
			string[] splitedStrs = start.Split('/');
			string result = "";
			foreach (string str in splitedStrs)
			{
				result += "/" + HttpUtility.UrlEncode(str);
			}
			return "http://" + result.TrimStart('/');
		}

		/// <summary>
		/// 重写了ExecuteQuery，增加了异常处理功能
		/// </summary>
		public new void ExecuteQuery()
		{
			try
			{
				base.ExecuteQuery();
			}
			catch (ServerException ex)
			{
				ErrorProcessor.Process(ex, ex.ServerErrorCode);
			}
		}
	}
}