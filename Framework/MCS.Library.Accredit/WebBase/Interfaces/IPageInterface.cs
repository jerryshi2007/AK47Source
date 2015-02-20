using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MCS.Library.Accredit.WebBase.Interfaces
{
	/// <summary>
	/// 定义了BaseWebClass基类的接口
	/// </summary>
	public interface IPageInterface
	{
		#region Function Interface

		/// <summary>
		/// 根据请求页面的Uri，得到该网站的根Url
		/// </summary>
		/// <returns>得到该网站的根Url。如当前页面为http://www.micosoft.com:8080/download.htm，
		/// 那么结果为http://www.microsoft.com:8080/</returns>
		string GetRootUrl();

		/// <summary>
		/// 得到Http请求中某一项的值
		/// </summary>
		/// <param name="strName">请求项的值</param>
		/// <param name="oDefault">缺省值</param>
		/// <returns>返回Http请求项的值，如果没有这一项，返回缺省值</returns>
		object GetRequestData(string strName, object oDefault);

		/// <summary>
		/// 得到Form提交的某一项的值
		/// </summary>
		/// <param name="strName">提交项的名称</param>
		/// <param name="oDefault">缺省值</param>
		/// <returns>返回Form提交项的值，如果没有这一项，返回缺省值</returns>
		object GetFormData(string strName, object oDefault);

		/// <summary>
		/// 根据XML的名称获得对应的XML文档对象。首先从Cache中查找，如果Cache中没有，则从文件中加载
		/// </summary>
		/// <param name="strXmlName">XML名称</param>
		/// <returns>XML文档对象</returns>
		XmlDocument GetXMLDocument(string strXmlName);

		/// <summary>
		/// 提供一个虚目录名称和一个XML文件名（不带后缀），返回一个XML文档对象
		/// </summary>
		/// <param name="strVirtualDir">虚目录名称</param>
		/// <param name="strXmlName">XML文件名（不带后缀）</param>
		/// <returns>XML文档对象</returns>
		XmlDocument GetXMLDocument(string strVirtualDir, string strXmlName);

		/// <summary>
		/// 根据XSD的名称获得对应的XML文档对象。首先从Cache中查找，如果Cache中没有，则从文件中加载
		/// </summary>
		/// <param name="strXsdName">XSD名称</param>
		/// <returns>XML文档对象</returns>
		XmlDocument GetXSDDocument(string strXsdName);

		/// <summary>
		/// 提供一个虚目录名称和一个XSD文件名（不带后缀），返回一个XML文档对象
		/// </summary>
		/// <param name="strVirtualDir">虚目录名称</param>
		/// <param name="strXsdName">XSD文件名（不带后缀）</param>
		/// <returns>XML文档对象</returns>
		XmlDocument GetXSDDocument(string strVirtualDir, string strXsdName);

		/// <summary>
		/// 根据文件名称（实际上是）得到文档类型
		/// </summary>
		/// <param name="strFileName">文件名称</param>
		/// <returns>文档类型</returns>
		string GetContentTypeByFileName(string strFileName);

		#endregion
	}
}
