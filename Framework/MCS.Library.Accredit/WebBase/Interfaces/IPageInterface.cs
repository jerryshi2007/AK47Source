using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MCS.Library.Accredit.WebBase.Interfaces
{
	/// <summary>
	/// ������BaseWebClass����Ľӿ�
	/// </summary>
	public interface IPageInterface
	{
		#region Function Interface

		/// <summary>
		/// ��������ҳ���Uri���õ�����վ�ĸ�Url
		/// </summary>
		/// <returns>�õ�����վ�ĸ�Url���統ǰҳ��Ϊhttp://www.micosoft.com:8080/download.htm��
		/// ��ô���Ϊhttp://www.microsoft.com:8080/</returns>
		string GetRootUrl();

		/// <summary>
		/// �õ�Http������ĳһ���ֵ
		/// </summary>
		/// <param name="strName">�������ֵ</param>
		/// <param name="oDefault">ȱʡֵ</param>
		/// <returns>����Http�������ֵ�����û����һ�����ȱʡֵ</returns>
		object GetRequestData(string strName, object oDefault);

		/// <summary>
		/// �õ�Form�ύ��ĳһ���ֵ
		/// </summary>
		/// <param name="strName">�ύ�������</param>
		/// <param name="oDefault">ȱʡֵ</param>
		/// <returns>����Form�ύ���ֵ�����û����һ�����ȱʡֵ</returns>
		object GetFormData(string strName, object oDefault);

		/// <summary>
		/// ����XML�����ƻ�ö�Ӧ��XML�ĵ��������ȴ�Cache�в��ң����Cache��û�У�����ļ��м���
		/// </summary>
		/// <param name="strXmlName">XML����</param>
		/// <returns>XML�ĵ�����</returns>
		XmlDocument GetXMLDocument(string strXmlName);

		/// <summary>
		/// �ṩһ����Ŀ¼���ƺ�һ��XML�ļ�����������׺��������һ��XML�ĵ�����
		/// </summary>
		/// <param name="strVirtualDir">��Ŀ¼����</param>
		/// <param name="strXmlName">XML�ļ�����������׺��</param>
		/// <returns>XML�ĵ�����</returns>
		XmlDocument GetXMLDocument(string strVirtualDir, string strXmlName);

		/// <summary>
		/// ����XSD�����ƻ�ö�Ӧ��XML�ĵ��������ȴ�Cache�в��ң����Cache��û�У�����ļ��м���
		/// </summary>
		/// <param name="strXsdName">XSD����</param>
		/// <returns>XML�ĵ�����</returns>
		XmlDocument GetXSDDocument(string strXsdName);

		/// <summary>
		/// �ṩһ����Ŀ¼���ƺ�һ��XSD�ļ�����������׺��������һ��XML�ĵ�����
		/// </summary>
		/// <param name="strVirtualDir">��Ŀ¼����</param>
		/// <param name="strXsdName">XSD�ļ�����������׺��</param>
		/// <returns>XML�ĵ�����</returns>
		XmlDocument GetXSDDocument(string strVirtualDir, string strXsdName);

		/// <summary>
		/// �����ļ����ƣ�ʵ�����ǣ��õ��ĵ�����
		/// </summary>
		/// <param name="strFileName">�ļ�����</param>
		/// <returns>�ĵ�����</returns>
		string GetContentTypeByFileName(string strFileName);

		#endregion
	}
}
