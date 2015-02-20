using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Caching;
using System.Web;
using System.IO;

using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.WebBase.Interfaces;
[assembly: System.Web.UI.WebResource("MCS.Library.Accredit.WebBase.stopLogo.gif", "img/gif")]

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// BaseWebPageClass 的摘要说明。
	/// </summary>
	public class WebBaseClass : System.Web.UI.Page, IPageInterface
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public WebBaseClass()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		/// <summary>
		/// 页面是否是设计模式,判断现在工程是否在运行，如果不在运行，有可能很多属性都不能正常赋值。
		/// </summary>
		protected bool IsDesignMode
		{
			get
			{
				return (Site != null && Site.DesignMode);
			}
		}

		/// <summary>
		/// 如果出现错误，返回到客户端的HTTP错误码
		/// </summary>
		protected int C_ERROR_STATUS_CODE = 201;

		#region IPageInterface 成员()
		/// <summary>
		/// 根据请求页面的Uri，得到该网站的根Url
		/// </summary>
		/// <returns>得到该网站的根Url。如当前页面为http://www.micosoft.com:8080/download.htm，
		/// 那么结果为http://www.microsoft.com:8080/</returns>
		public string GetRootUrl()
		{
			Uri uri = Request.Url;

			return uri.Scheme + "://" + uri.Host + (uri.IsDefaultPort ? string.Empty : ":" + uri.Port) + "/";
		}

		/// <summary>
		/// 得到Http请求中某一项的值
		/// </summary>
		/// <param name="strName">请求项的值</param>
		/// <param name="oDefault">缺省值</param>
		/// <returns>返回Http请求项的值，如果没有这一项，返回缺省值</returns>
		/// <example>
		///得到url中name的值，如果name不存在，则返回“没有这个对象！”
		/// <code>
		///	string Str = this.GetRequestData("name", "没有这个对象！").ToString();
		/// </code>
		/// </example>
		public object GetRequestData(string strName, object oDefault)
		{
			object oValue = Request.QueryString[strName];

			if (oValue == null)
				oValue = oDefault;

			System.Type t = oDefault.GetType();
			oValue = System.Convert.ChangeType(oValue, t);

			return oValue;
		}

		/// <summary>
		///得到Form提交的某一项的值
		/// </summary>
		/// <param name="strName">提交项的名称</param>
		/// <param name="oDefault">缺省值</param>
		/// <returns>返回Form提交项的值，如果没有这一项，返回缺省值</returns>
		/// <example>
		/// 得到Text4中的值，如果Text4不存在，则返回“没有这个对象！”
		/// <code>
		///	string Str4 = this.GetFormData("Text4", "没有这个对象！").ToString();
		/// </code>
		/// </example>
		public object GetFormData(string strName, object oDefault)
		{
			object oValue = Request.Form[strName];

			if (oValue == null)
				oValue = oDefault;

			System.Type t = oDefault.GetType();
			oValue = System.Convert.ChangeType(oValue, t);

			return oValue;
		}

		/// <summary>
		/// 根据XML的名称获得对应的XML文档对象。首先从Cache中查找，如果Cache中没有，则从文件中加载
		/// </summary>
		/// <param name="strXmlName">XML文件名（不带后缀）</param>
		/// <returns>XML文档对象</returns>
		/// <example>
		///得到当前站点xml目录下的test.xml的对象
		/// <code>
		///	GetXMLDocument("test")
		///	</code>
		/// </example>
		public XmlDocument GetXMLDocument(string strXmlName)
		{
			string strVirFile = Request.ApplicationPath + @"/XML/" + strXmlName + ".xml";
			string strPhyFile = Server.MapPath(strVirFile);
			return GetXmlDocFromPhysicalPath(strPhyFile, strXmlName);
		}

		/// <summary>
		/// 提供一个虚目录名称和一个XML文件名（不带后缀），返回一个XML文档对象（相对路径）
		/// </summary>
		/// <param name="strVirtualDir">虚目录名称</param>
		/// <param name="strXmlName">XML文件名（不带后缀）</param>
		/// <returns>XML文档对象</returns>
		/// <example>
		///得到当前站点xml目录下的test.xml的对象
		/// <code>
		///	GetXMLDocument("xml", "test")
		/// </code>
		/// </example>
		public XmlDocument GetXMLDocument(string strVirtualDir, string strXmlName)
		{
			return GetXmlDocFromPhysicalPath(GetPhysicalPath(strVirtualDir, strXmlName, "xml"), strXmlName);
		}

		/// <summary>
		/// 根据XSD的名称获得对应的XML文档对象。首先从Cache中查找，如果Cache中没有，则从文件中加载
		/// </summary>
		/// <param name="strXsdName">XSD名称</param>
		/// <returns>XML文档对象</returns>
		/// <example>
		///得到当前站点xsd目录下的test.xsd的对象
		/// <code>
		///	GetXSDDocument("test")
		/// </code>
		/// </example>
		public XmlDocument GetXSDDocument(string strXsdName)
		{
			string strVirFile = Request.ApplicationPath + @"/XSD/" + strXsdName + ".xsd";
			string strPhyFile = Server.MapPath(strVirFile);
			return GetXmlDocFromPhysicalPath(strPhyFile, strXsdName);
		}

		/// <summary>
		/// 提供一个虚目录名称和一个XSD文件名（不带后缀），返回一个XML文档对象（相对路径）
		/// </summary>
		/// <param name="strVirtualDir">虚目录名称</param>
		/// <param name="strXsdName">XSD文件名（不带后缀）</param>
		/// <returns>XML文档对象</returns>
		/// <example>
		///得到当前站点xsd目录下的test.xsd的对象
		/// <code>
		///	GetXSDDocument("xsd", "test")
		/// </code>
		/// </example>
		public XmlDocument GetXSDDocument(string strVirtualDir, string strXsdName)
		{
			return GetXmlDocFromPhysicalPath(GetPhysicalPath(strVirtualDir, strXsdName, "xsd"), strXsdName);
		}

		/// <summary>
		/// 根据文件名称（实际上是）得到文档类型
		/// </summary>
		/// <param name="strFileName">文件名称或是路径</param>
		/// <returns>文档类型</returns>
		/// <example>
		///得到file1的后缀名，然后返回文档类型
		/// <code>
		///	GetContentTypeByFileName("/hgzs/upload/file1.doc");
		/// </code>
		/// </example>
		public string GetContentTypeByFileName(string strFileName)
		{
			return S_GetContentTypeByFileName(strFileName);
		}

		#endregion

		#region public function for basePage
		/// <summary>
		/// 根据文件名称（实际上是）得到文档类型
		/// </summary>
		/// <param name="strFileName">文件名称或是路径</param>
		/// <returns>文档类型</returns>
		/// <example>
		///得到file1的后缀名，然后返回文档类型
		/// <code>
		///	BaseWebClass.S_GetContentTypeByFileName("/hgzs/upload/file1.doc");
		/// </code>
		/// </example>
		public string S_GetContentTypeByFileName(string strFileName)
		{
			string strDocType = Path.GetExtension(strFileName);

			string strResult = "application/octet-stream";

			if (strDocType != string.Empty)
			{
				string strExt = strDocType.Substring(1, strDocType.Length - 1).ToUpper();
				switch (strExt)
				{
					case "DOC":
					case "DOT":
					case "RTF":
						strResult = "application/msword";
						break;
					case "HTM":
					case "HTML":
					case "SHTML":
						strResult = "text/html";
						break;
					case "BMP":
						strResult = "image/bmp";
						break;
					case "GIF":
						strResult = "image/gif";
						break;
					case "GD":
						strResult = "application/gd";
						break;
					case "GW":
					case "GW2":
						strResult = "application/gw";
						break;
					case "spd":
						strResult = "application/spd";
						break;
					case "PS2":
					case "S2":
					case "S72":
					case "S92":
						strResult = "application/sed-s92";
						break;
					case "JPG":
					case "JPE":
					case "JPEG":
						strResult = "image/jpeg";
						break;
					case "MP1":
					case "MP2":
					case "MP3":
					case "MPA":
					case "MPGA":
						strResult = "audio/mpeg";
						break;
					case "MID":
					case "MIDI":
						strResult = "audio/mid";
						break;
					case "MPE":
					case "MPG":
					case "MPEG":
						strResult = "video/mpeg";
						break;
					case "MHT":
					case "MHTML":
						strResult = "message/rfc822";
						break;
					case "PDF":
						strResult = "application/pdf";
						break;
					case "PNG":
						strResult = "image/png";
						break;
					case "POT":
					case "PPA":
					case "PPS":
					case "PPT":
					case "PWZ":
						strResult = "application/vnd.ms-powerpoint";
						break;
					case "PS":
					case "EPS":
						strResult = "application/postscript";
						break;
					case "TXT":
					case "RC":
					case "PRC":
						strResult = "text/plain";
						break;
					case "TIF":
					case "TIFF":
						strResult = "image/tiff";
						break;
					case "VSD":
					case "VSS":
					case "VST":
						strResult = "application/vnd.visio";
						break;
					case "WMD":
						strResult = "video/x-ms-wmd";
						break;
					case "WMP":
						strResult = "video/x-ms-wmp";
						break;
					case "WMA":
						strResult = "audio/x-ms-wma";
						break;
					case "WMZ":
						strResult = "application/x-ms-wmz";
						break;
					case "WMV":
						strResult = "video/x-ms-wmv";
						break;
					case "AVI":
						strResult = "video/x-msvideo";
						break;
					case "ICO":
						strResult = "image/x-icon";
						break;
					case "XML":
					case "XSD":
					case "RESX":
						strResult = "text/xml";
						break;
					case "XLS":
						strResult = "application/vnd.ms-excel";
						break;
					default:
						{
							//XmlElement elem = GetFileTypeDespNode(strExt);

							//if (elem != null)
							//    strResult = elem.GetAttribute("mimeType");
							//else
								strResult = "application/octet-stream";
						}
						break;
				}
			}

			return strResult;
		}
		/// <summary>
		/// 根据文件名称（实际上是）得到文档对应的图标
		/// </summary>
		/// <param name="strFileName">文件名称或是路径</param>
		/// <param name="bIsBig">是否是大图标</param>
		/// <returns>文档类型</returns>
		/// <example>
		///得到file1的后缀名，然后返回文档图标
		/// <code>
		///	BaseWebClass.S_GetImageNameByFileName("/hgzs/upload/file1.doc");
		/// </code>
		/// </example>
		public string S_GetImageNameByFileName(string strFileName, bool bIsBig)
		{
			string strDir = "/images/";

			if (bIsBig)
				strDir += "32/";

			string strResult = "WordPad.gif";

			string strType = Path.GetExtension(strFileName).ToLower();

			if (strType != string.Empty)
			{
				string strExt = strType.Substring(1, strType.Length - 1).ToLower();
				switch (strExt)
				{
					case "jpg":
					case "jpeg":
					case "gif":
					case "tif":
					case "tiff":
						strResult = "image.gif";
						break;
					case "bmp":
					case "pcx":
					case "wmf":
						strResult = "bmp.gif";
						break;
					case "dot":
					case "doc":
						strResult = "word.gif";
						break;
					case "zip": strResult = "winzip.gif";
						break;
					case "pdf": strResult = "pdf.gif";
						break;
					case "xsd":
					case "xml":
					case "html":
					case "htm": strResult = "htm.gif";
						break;
					case "ppt":
					case "pps":
					case "ppa":
						strResult = "ppt.gif";
						break;
					case "gd":
					case "gw":
					case "gw2":
					case "ps2":
					case "s2":
					case "s72":
					case "s92":
					case "spd":
						strResult = "shusheng.gif";
						break;
					case "mp3":
					case "wav":
					case "wma":
						strResult = "sound.gif";
						break;
					case "mpeg":
					case "mpg":
					case "avi":
					case "wmv":
						strResult = "wmp.gif";
						break;
					case "csv":
					case "xla":
					case "xls":
					case "xlw":
						strResult = "excel.gif";
						break;
					default:
						{
							//XmlElement elem = GetFileTypeDespNode(strExt);

							//if (elem != null)
							//    strResult = elem.GetAttribute("image");

							break;
						}
				}
			}

			return strDir + strResult;
		}

		#endregion

		#region private function for basePage

		/// <summary>
		/// 获取在服务器中虚拟目录中的虚目录strVirtualDir下的strFileName文件名strSuffix扩展名的物理路径
		/// </summary>
		/// <param name="strVirtualDir">虚目录</param>
		/// <param name="strFileName">文件名</param>
		/// <param name="strSuffix">扩展名</param>
		/// <returns></returns>
		private string GetPhysicalPath(string strVirtualDir, string strFileName, string strSuffix)
		{
			if (strVirtualDir != string.Empty)
			{
				if (strVirtualDir[strVirtualDir.Length - 1] != '/')
					strVirtualDir += "/";
			}
			else
				strVirtualDir += "/";

			string strPath = strVirtualDir + strFileName + "." + strSuffix;

			return Server.MapPath(strPath);
		}

		/// <summary>
		/// 根据XML名称，打开strFilePhysicalName目录下的XML文件，并且返回相应的XmlDocument对象
		/// </summary>
		/// <param name="strFilePhysicalName">文件物理路径</param>
		/// <param name="strName">XML文档名称</param>
		/// <returns>XmlDocument对象</returns>
		private XmlDocument GetXmlDocFromPhysicalPath(string strFilePhysicalName, string strName)
		{
			XmlDocument xmlDoc = Cache[strName] as XmlDocument;

			if (xmlDoc == null)
			{
				xmlDoc = CreateXMLDocument(strFilePhysicalName);
				CacheDependency dependency = new CacheDependency(strFilePhysicalName);

				Cache.Insert(strName, xmlDoc, dependency);
			}

			return xmlDoc;
		}

		/// <summary>
		/// 根据文件物理路径打开文件，并且返回相应的XmlDocument对象
		/// </summary>
		/// <param name="strFilePhysicalName">文件物理路径</param>
		/// <returns>XmlDocument对象</returns>
		private XmlDocument CreateXMLDocument(string strFilePhysicalName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(strFilePhysicalName);

			return xmlDoc;
		}

		///// <summary>
		///// 根据系统配置信息数据获取指定文件类型(用于展示文件图标或者文件打开方式)
		///// </summary>
		///// <param name="strExt">文件的扩展名</param>
		///// <returns>配置中对应的配置节点</returns>
		//private XmlElement GetFileTypeDespNode(string strExt)
		//{
		//    SysConfig sc = new SysConfig();

		//    XmlNode nodeFileType = sc.GetSectionNode("BaseWebClass", "FileType");

		//    XmlElement elemItem = null;

		//    if (nodeFileType != null)
		//    {
		//        strExt = strExt.ToUpper();

		//        foreach (XmlElement elem in nodeFileType.ChildNodes)
		//        {
		//            string strExtNames = elem.GetAttribute("fileExt").ToUpper();

		//            if (strExtNames.IndexOf(strExt) != -1)
		//            {
		//                elemItem = elem;
		//                break;
		//            }
		//        }
		//    }

		//    return elemItem;
		//}
		//		/// <summary>
		//		/// 字符串编码转换
		//		/// </summary>
		//		/// <param name="strText"></param>
		//		/// <returns></returns>
		//		private static string UrlEncodeToGB2312(string strText)
		//		{
		//			byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(strText);
		//
		//			return HttpUtility.UrlEncode(bytes);
		//		}

		#endregion

		#region protected
		/// <summary>
		/// 设置错误信息的Xml文本
		/// </summary>
		/// <param name="xmlResult">需要放置信息的Xml对象</param>
		/// <param name="ex">例外对象</param>
		protected virtual void SetErrorResult(XmlDocument xmlResult, System.Exception ex)
		{
			TransExceptionInfo tei = new TransExceptionInfo(ex);

			string strMsg = "<Message>" + HttpUtility.HtmlEncode(tei.Message) + "</Message>";

			if (tei.SupportHtml != string.Empty)
				strMsg += "<SupportText>\n" + tei.SupportHtml + "</SupportText>";

			if ((ex is System.ApplicationException) == false || IsOutputStackTrace())
				strMsg += "\n" + "<MessageExt>" + HttpUtility.HtmlEncode(tei.StackTrace) + "</MessageExt>";

			SetErrorResult(xmlResult, strMsg, tei.StackTrace);
		}

		/// <summary>
		/// 设置错误信息的Xml文本
		/// </summary>
		/// <param name="xmlResult">需要放置信息的Xml对象</param>
		/// <param name="strError">错误信息</param>
		/// <param name="strStack">错误栈</param>
		protected void SetErrorResult(XmlDocument xmlResult, string strError, string strStack)
		{
			xmlResult.LoadXml("<ResponseError />");
			XmlElement nodeRoot = xmlResult.DocumentElement;

			XmlNode node = xmlResult.CreateNode(XmlNodeType.Element, "Value", string.Empty);
			node.InnerXml = strError;
			nodeRoot.AppendChild(node);

			node = xmlResult.CreateNode(XmlNodeType.Element, "Stack", string.Empty);

			if (IsOutputStackTrace())
				node.InnerText = strStack;

			nodeRoot.AppendChild(node);
		}

		/// <summary>
		/// 在返回的HTTP头当中增加错误信息
		/// </summary>
		/// <param name="strMessage">错误信息</param>
		protected void AddErrorHeaderInfo(string strMessage)
		{
			strMessage = strMessage.Replace("\n", " ");
			Response.StatusDescription = strMessage;
			Response.AppendHeader("OAInfo", strMessage);
		}

		/// <summary>
		/// 在返回的HTTP头当中增加错误栈
		/// </summary>
		/// <param name="strStackTrace">错误栈</param>
		protected void AddStackTraceHeaderInfo(string strStackTrace)
		{
			strStackTrace = strStackTrace.Replace("\n", " ");
			Response.AppendHeader("OAStackTrace", strStackTrace);
		}

		/// <summary>
		/// 从配置文件中得到是否输出错误栈
		/// </summary>
		/// <returns>True-输出错误栈，False-不输出错误栈</returns>
		protected bool IsOutputStackTrace()
		{
			return BaseWebSection.GetConfig().ShowErrorDebug;
		}

		/// <summary>
		/// 缺省的错误处理
		/// </summary>
		/// <param name="e">事件参数</param>
		/// <remarks>
		/// 重写了系统的OnError方法。首先清除了出错前输出的部分，
		/// 然后从资源中得到正确的错误信息并做一些相应的转义，
		/// 最后转到错误页面输出错误信息结束。
		/// </remarks>
		protected override void OnError(EventArgs e)
		{
			Response.Clear();											//清除已经输出的部分，然后转到错误页面
			string strHtml = CommonDefine.GetEmbeddedResString(CommonResource.ErrorTemplate);

			System.Exception ex = Server.GetLastError();				//得到错误信息

			if (ex != null)
			{
				TransExceptionInfo tei = new TransExceptionInfo(ex);

				string strStackTrace = string.Empty;

				if ((ex is System.ApplicationException == false) || IsOutputStackTrace())
					strStackTrace = Server.HtmlEncode(tei.StackTrace);
				//					strStackTrace = UrlEncodeToGB2312(tei.StackTrace);

				//				string strMsg = Server.HtmlEncode(tei.Message);//changed by ccic\yuanyong 2005-7-28
				string strMsg = tei.Message;//changed by ccic\yuanyong 2005-7-28
				//				string strMsg = UrlEncodeToGB2312(tei.Message);

				if (tei.SupportHtml != string.Empty)
					strMsg += "<p>" + tei.SupportHtml + "<p>";

				strStackTrace = Server.HtmlEncode(strStackTrace);

				string strMsgEncoding = strMsg.Replace("\n", "<br>");
				string strStackTraceEncoding = strStackTrace.Replace("\n", "<br>");

				strHtml = strHtml.Replace("{LabelError}", strMsgEncoding);
				strHtml = strHtml.Replace("{StackTrace}", strStackTraceEncoding);

				string url = BaseWebSection.GetConfig().ErrorImgUrl;
				if (string.IsNullOrEmpty(url))
				{
					url = this.ClientScript.GetWebResourceUrl(typeof(WebBaseClass),
						"MCS.Library.Accredit.WebBase.stopLogo.gif");
				}
				strHtml = strHtml.Replace("{ShowErrorImg}", url);

				Server.ClearError();

				Response.Write(strHtml);
				Response.End();
			}
		}

		/// <summary>
		/// 重载页面加载，调用了Page类的OnLoad(e)事件
		/// </summary>
		/// <param name="e">事件参数</param>
		protected override void OnLoad(EventArgs e)
		{
			GlobalInfo.InitHttpEnv(Request);

			base.OnLoad(e);
		}

		/// <summary>
		/// 检查权限，应该被派声类重载
		/// </summary>
		protected virtual void CheckPrivilege()
		{
		}

		/// <summary>
		/// 页面的加载过程结束，可以重载
		/// </summary>
		protected virtual void PageLoadFinally()
		{
		}

		#endregion
	}
}
