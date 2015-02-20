using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Xml;
using System.Collections.Specialized;
using System.Web;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 页面访问票据
	/// </summary>
	[Serializable]
	public class AccessTicket
	{
		/// <summary>
		/// 获取页面访问票据的参数名称
		/// </summary>
		public const string AccquireAccessTicketParamName = "_aat";

		/// <summary>
		/// 生成票据时，是否自动将相对地址转换为绝对地址
		/// </summary>
		public const string AutoMakeAbsoluteParamName = "_ama";

		/// <summary>
		/// 页面访问票据的参数名称
		/// </summary>
		public const string AccessTicketParamName = "_at";

		/// <summary>
		/// 默认的有效期
		/// </summary>
		public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

		/// <summary>
		/// 源Url
		/// </summary>
		public string SourceUrl { get; set; }

		/// <summary>
		/// 目标Url
		/// </summary>
		public string DestinationUrl { get; set; }

		/// <summary>
		/// 生成时间
		/// </summary>
		public DateTime GenerateTime { get; set; }

		/// <summary>
		/// 从字符串恢复AccessTicket，如果不能解析，则返回null
		/// </summary>
		/// <param name="aTicketString"></param>
		/// <returns></returns>
		public static AccessTicket FromString(string aTicketString)
		{
			AccessTicket result = null;

			if (aTicketString.IsNotEmpty())
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(aTicketString);

				result = new AccessTicket();

				XmlElement root = xmlDoc.DocumentElement;

				result.SourceUrl = XmlHelper.GetAttributeText(root, "su", string.Empty);
				result.DestinationUrl = XmlHelper.GetAttributeText(root, "du", string.Empty);
				result.GenerateTime = XmlHelper.GetAttributeValue(root, "gt", DateTime.MinValue);
			}

			return result;
		}

		/// <summary>
		/// 转成字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<ATicket/>");

			XmlElement root = xmlDoc.DocumentElement;

			XmlHelper.AppendAttr(root, "su", this.SourceUrl);
			XmlHelper.AppendAttr(root, "du", this.DestinationUrl);
			XmlHelper.AppendAttr(root, "gt", string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", this.GenerateTime));

			return xmlDoc.OuterXml;
		}

		/// <summary>
		/// 如果目标路径是相对地址，则转换为和baseUri相关的绝对地址
		/// </summary>
		/// <param name="baseUri"></param>
		/// <returns></returns>
		public string MakeDestinationUrlAbsolute(Uri baseUri)
		{
			Uri destUri = new Uri(this.DestinationUrl, UriKind.RelativeOrAbsolute);

			if (destUri.IsAbsoluteUri == false)
			{
				baseUri.NullCheck("baseUri");

				this.DestinationUrl = destUri.MakeAbsolute(baseUri).ToString();
			}

			return this.DestinationUrl;
		}

		/// <summary>
		/// 票据的时间是否合法
		/// </summary>
		/// <param name="timeout">有效期</param>
		/// <returns></returns>
		public bool TimeStampIsValid(TimeSpan timeout)
		{
			return (DateTime.Now.Subtract(this.GenerateTime) <= timeout);
		}

		/// <summary>
		/// 目标Url是否合法
		/// </summary>
		/// <param name="destUrl"></param>
		/// <param name="urlCheckParts">需要检查的url中的部分</param>
		/// <returns></returns>
		public bool UrlIsValid(Uri destUrl, AccessTicketUrlCheckParts urlCheckParts)
		{
			bool result = true;

			if (this.DestinationUrl.IsNotEmpty())
			{
				Uri uri1 = new Uri(this.DestinationUrl, UriKind.RelativeOrAbsolute);

				if ((urlCheckParts & AccessTicketUrlCheckParts.SchemeHostAndPort) != AccessTicketUrlCheckParts.None)
					result = uri1.CompareSchemeAndHost(destUrl);

				if (result && (urlCheckParts & AccessTicketUrlCheckParts.PathAndParameters) != AccessTicketUrlCheckParts.None)
					result = uri1.ComparePathAndParameters(destUrl, AccessTicket.AccessTicketParamName);
			}

			return result;
		}

		/// <summary>
		/// 在url中添加AccessTicket
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public string AppendToUrl(string url)
		{
			string result = url;

			if (url.IsNotEmpty())
			{
				NameValueCollection uParams = UriHelper.GetUriParamsCollection(url);

				uParams[AccessTicketParamName] = Common.EncryptString(this.ToString());
				result = UriHelper.CombineUrlParams(url, uParams);
			}

			return result;
		}
	}
}
