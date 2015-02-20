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
	/// BaseWebPageClass ��ժҪ˵����
	/// </summary>
	public class WebBaseClass : System.Web.UI.Page, IPageInterface
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public WebBaseClass()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		/// <summary>
		/// ҳ���Ƿ������ģʽ,�ж����ڹ����Ƿ������У�����������У��п��ܺܶ����Զ�����������ֵ��
		/// </summary>
		protected bool IsDesignMode
		{
			get
			{
				return (Site != null && Site.DesignMode);
			}
		}

		/// <summary>
		/// ������ִ��󣬷��ص��ͻ��˵�HTTP������
		/// </summary>
		protected int C_ERROR_STATUS_CODE = 201;

		#region IPageInterface ��Ա()
		/// <summary>
		/// ��������ҳ���Uri���õ�����վ�ĸ�Url
		/// </summary>
		/// <returns>�õ�����վ�ĸ�Url���統ǰҳ��Ϊhttp://www.micosoft.com:8080/download.htm��
		/// ��ô���Ϊhttp://www.microsoft.com:8080/</returns>
		public string GetRootUrl()
		{
			Uri uri = Request.Url;

			return uri.Scheme + "://" + uri.Host + (uri.IsDefaultPort ? string.Empty : ":" + uri.Port) + "/";
		}

		/// <summary>
		/// �õ�Http������ĳһ���ֵ
		/// </summary>
		/// <param name="strName">�������ֵ</param>
		/// <param name="oDefault">ȱʡֵ</param>
		/// <returns>����Http�������ֵ�����û����һ�����ȱʡֵ</returns>
		/// <example>
		///�õ�url��name��ֵ�����name�����ڣ��򷵻ء�û��������󣡡�
		/// <code>
		///	string Str = this.GetRequestData("name", "û���������").ToString();
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
		///�õ�Form�ύ��ĳһ���ֵ
		/// </summary>
		/// <param name="strName">�ύ�������</param>
		/// <param name="oDefault">ȱʡֵ</param>
		/// <returns>����Form�ύ���ֵ�����û����һ�����ȱʡֵ</returns>
		/// <example>
		/// �õ�Text4�е�ֵ�����Text4�����ڣ��򷵻ء�û��������󣡡�
		/// <code>
		///	string Str4 = this.GetFormData("Text4", "û���������").ToString();
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
		/// ����XML�����ƻ�ö�Ӧ��XML�ĵ��������ȴ�Cache�в��ң����Cache��û�У�����ļ��м���
		/// </summary>
		/// <param name="strXmlName">XML�ļ�����������׺��</param>
		/// <returns>XML�ĵ�����</returns>
		/// <example>
		///�õ���ǰվ��xmlĿ¼�µ�test.xml�Ķ���
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
		/// �ṩһ����Ŀ¼���ƺ�һ��XML�ļ�����������׺��������һ��XML�ĵ��������·����
		/// </summary>
		/// <param name="strVirtualDir">��Ŀ¼����</param>
		/// <param name="strXmlName">XML�ļ�����������׺��</param>
		/// <returns>XML�ĵ�����</returns>
		/// <example>
		///�õ���ǰվ��xmlĿ¼�µ�test.xml�Ķ���
		/// <code>
		///	GetXMLDocument("xml", "test")
		/// </code>
		/// </example>
		public XmlDocument GetXMLDocument(string strVirtualDir, string strXmlName)
		{
			return GetXmlDocFromPhysicalPath(GetPhysicalPath(strVirtualDir, strXmlName, "xml"), strXmlName);
		}

		/// <summary>
		/// ����XSD�����ƻ�ö�Ӧ��XML�ĵ��������ȴ�Cache�в��ң����Cache��û�У�����ļ��м���
		/// </summary>
		/// <param name="strXsdName">XSD����</param>
		/// <returns>XML�ĵ�����</returns>
		/// <example>
		///�õ���ǰվ��xsdĿ¼�µ�test.xsd�Ķ���
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
		/// �ṩһ����Ŀ¼���ƺ�һ��XSD�ļ�����������׺��������һ��XML�ĵ��������·����
		/// </summary>
		/// <param name="strVirtualDir">��Ŀ¼����</param>
		/// <param name="strXsdName">XSD�ļ�����������׺��</param>
		/// <returns>XML�ĵ�����</returns>
		/// <example>
		///�õ���ǰվ��xsdĿ¼�µ�test.xsd�Ķ���
		/// <code>
		///	GetXSDDocument("xsd", "test")
		/// </code>
		/// </example>
		public XmlDocument GetXSDDocument(string strVirtualDir, string strXsdName)
		{
			return GetXmlDocFromPhysicalPath(GetPhysicalPath(strVirtualDir, strXsdName, "xsd"), strXsdName);
		}

		/// <summary>
		/// �����ļ����ƣ�ʵ�����ǣ��õ��ĵ�����
		/// </summary>
		/// <param name="strFileName">�ļ����ƻ���·��</param>
		/// <returns>�ĵ�����</returns>
		/// <example>
		///�õ�file1�ĺ�׺����Ȼ�󷵻��ĵ�����
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
		/// �����ļ����ƣ�ʵ�����ǣ��õ��ĵ�����
		/// </summary>
		/// <param name="strFileName">�ļ����ƻ���·��</param>
		/// <returns>�ĵ�����</returns>
		/// <example>
		///�õ�file1�ĺ�׺����Ȼ�󷵻��ĵ�����
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
		/// �����ļ����ƣ�ʵ�����ǣ��õ��ĵ���Ӧ��ͼ��
		/// </summary>
		/// <param name="strFileName">�ļ����ƻ���·��</param>
		/// <param name="bIsBig">�Ƿ��Ǵ�ͼ��</param>
		/// <returns>�ĵ�����</returns>
		/// <example>
		///�õ�file1�ĺ�׺����Ȼ�󷵻��ĵ�ͼ��
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
		/// ��ȡ�ڷ�����������Ŀ¼�е���Ŀ¼strVirtualDir�µ�strFileName�ļ���strSuffix��չ��������·��
		/// </summary>
		/// <param name="strVirtualDir">��Ŀ¼</param>
		/// <param name="strFileName">�ļ���</param>
		/// <param name="strSuffix">��չ��</param>
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
		/// ����XML���ƣ���strFilePhysicalNameĿ¼�µ�XML�ļ������ҷ�����Ӧ��XmlDocument����
		/// </summary>
		/// <param name="strFilePhysicalName">�ļ�����·��</param>
		/// <param name="strName">XML�ĵ�����</param>
		/// <returns>XmlDocument����</returns>
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
		/// �����ļ�����·�����ļ������ҷ�����Ӧ��XmlDocument����
		/// </summary>
		/// <param name="strFilePhysicalName">�ļ�����·��</param>
		/// <returns>XmlDocument����</returns>
		private XmlDocument CreateXMLDocument(string strFilePhysicalName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(strFilePhysicalName);

			return xmlDoc;
		}

		///// <summary>
		///// ����ϵͳ������Ϣ���ݻ�ȡָ���ļ�����(����չʾ�ļ�ͼ������ļ��򿪷�ʽ)
		///// </summary>
		///// <param name="strExt">�ļ�����չ��</param>
		///// <returns>�����ж�Ӧ�����ýڵ�</returns>
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
		//		/// �ַ�������ת��
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
		/// ���ô�����Ϣ��Xml�ı�
		/// </summary>
		/// <param name="xmlResult">��Ҫ������Ϣ��Xml����</param>
		/// <param name="ex">�������</param>
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
		/// ���ô�����Ϣ��Xml�ı�
		/// </summary>
		/// <param name="xmlResult">��Ҫ������Ϣ��Xml����</param>
		/// <param name="strError">������Ϣ</param>
		/// <param name="strStack">����ջ</param>
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
		/// �ڷ��ص�HTTPͷ�������Ӵ�����Ϣ
		/// </summary>
		/// <param name="strMessage">������Ϣ</param>
		protected void AddErrorHeaderInfo(string strMessage)
		{
			strMessage = strMessage.Replace("\n", " ");
			Response.StatusDescription = strMessage;
			Response.AppendHeader("OAInfo", strMessage);
		}

		/// <summary>
		/// �ڷ��ص�HTTPͷ�������Ӵ���ջ
		/// </summary>
		/// <param name="strStackTrace">����ջ</param>
		protected void AddStackTraceHeaderInfo(string strStackTrace)
		{
			strStackTrace = strStackTrace.Replace("\n", " ");
			Response.AppendHeader("OAStackTrace", strStackTrace);
		}

		/// <summary>
		/// �������ļ��еõ��Ƿ��������ջ
		/// </summary>
		/// <returns>True-�������ջ��False-���������ջ</returns>
		protected bool IsOutputStackTrace()
		{
			return BaseWebSection.GetConfig().ShowErrorDebug;
		}

		/// <summary>
		/// ȱʡ�Ĵ�����
		/// </summary>
		/// <param name="e">�¼�����</param>
		/// <remarks>
		/// ��д��ϵͳ��OnError��������������˳���ǰ����Ĳ��֣�
		/// Ȼ�����Դ�еõ���ȷ�Ĵ�����Ϣ����һЩ��Ӧ��ת�壬
		/// ���ת������ҳ�����������Ϣ������
		/// </remarks>
		protected override void OnError(EventArgs e)
		{
			Response.Clear();											//����Ѿ�����Ĳ��֣�Ȼ��ת������ҳ��
			string strHtml = CommonDefine.GetEmbeddedResString(CommonResource.ErrorTemplate);

			System.Exception ex = Server.GetLastError();				//�õ�������Ϣ

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
		/// ����ҳ����أ�������Page���OnLoad(e)�¼�
		/// </summary>
		/// <param name="e">�¼�����</param>
		protected override void OnLoad(EventArgs e)
		{
			GlobalInfo.InitHttpEnv(Request);

			base.OnLoad(e);
		}

		/// <summary>
		/// ���Ȩ�ޣ�Ӧ�ñ�����������
		/// </summary>
		protected virtual void CheckPrivilege()
		{
		}

		/// <summary>
		/// ҳ��ļ��ع��̽�������������
		/// </summary>
		protected virtual void PageLoadFinally()
		{
		}

		#endregion
	}
}
