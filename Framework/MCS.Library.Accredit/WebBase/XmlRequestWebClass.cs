using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using MCS.Library.Core;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// ��̨����XMLHTTP�����WEB���ࣨ������ݣ�
	/// </summary>
	public class XmlRequestWebClass : WebBaseClass
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public XmlRequestWebClass()
		{
		}


		/// <summary>
		/// ���Ȩ�ޣ����ػ������Ӧ����
		/// </summary>
		protected override void CheckPrivilege()
		{
		}

		/// <summary>
		/// ҳ��ļ��ع��̽�������������
		/// </summary>
		protected override void PageLoadFinally()
		{
			Response.End();
		}

		/// <summary>
		/// �õ�����ĸ��ڵ�����
		/// </summary>
		protected string RootName
		{
			get
			{
				return _XmlRequest.DocumentElement.Name;
			}
		}

		/// <summary>
		/// �õ������XML��������
		/// </summary>
		protected string CommandName
		{
			get
			{
				XmlNode nodeCmd = GetCommandNode();

				XmlNode nodeAttr = nodeCmd.Attributes.GetNamedItem("name");

				ExceptionHelper.TrueThrow<ApplicationException>(nodeAttr == null, "�Ƿ���XML�����ʽ��û����������");

				return nodeAttr.Value.ToString();
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		protected int ParamsCount
		{
			get
			{
				XmlNode nodeCmd = GetCommandNode();

				return nodeCmd.ChildNodes.Count;
			}
		}

		/// <summary>
		/// ȡ�����ĳһ��������ֵ
		/// </summary>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		protected string ParamValue(int nIndex)
		{
			XmlNode nodeCmd = GetCommandNode();

			return nodeCmd.ChildNodes[nIndex].InnerText;
		}

		/// <summary>
		/// �õ�����ڵ㣨���в����ĸ��ڵ㣩
		/// </summary>
		/// <returns>��ʾ�����XML�ڵ�</returns>
		protected XmlNode GetCommandNode()
		{
			XmlNode nodeCmd = _XmlRequest.SelectSingleNode(".//COMMAND");

			ExceptionHelper.TrueThrow<ApplicationException>(nodeCmd == null, "�Ƿ���XML�����ʽ");

			return nodeCmd;
		}

		/*
		 * ���ز���
		 * 
		 * */
		/// <summary>
		/// ����ҳ�����
		/// </summary>
		/// <param name="e">�¼�����</param>
		protected override void OnLoad(EventArgs e)
		{
			try
			{
				Response.ContentType = "text/xml";

				if (Request.QueryString["xmlHttp"] == "false")
				{
					_XmlRequest.LoadXml(Request.QueryString["xmlRequest"]);
				}
				else
				{
					_XmlRequest.Load(Request.InputStream);
				}

				Debug.WriteLine(_XmlRequest.OuterXml, "XmlWebPage");

				CheckPrivilege();

				base.OnLoad(e);

				if (_XmlResult.InnerXml == String.Empty)
					SetOKResult();

				Debug.Write(_XmlResult.OuterXml.Substring(0, Math.Min(_XmlResult.OuterXml.Length, 512)), "XmlWebPage");
				Debug.Write("\n\n");
			}
			catch (System.Exception ex)
			{
				SetErrorResult(ex);

				Response.StatusCode = C_ERROR_STATUS_CODE;

				string strMsg = ex.Message;

				if (IsOutputStackTrace())
					AddStackTraceHeaderInfo(ex.StackTrace);

				AddErrorHeaderInfo(strMsg);

				Debug.WriteLine("Error:" + ex.Message, "XmlWebPage");
				Debug.WriteLine("Stack:" + ex.StackTrace, "XmlWebPage");
			}
			finally
			{
				_XmlResult.Save(Response.OutputStream);
				Response.End();
			}
		}

		/// <summary>
		/// ����ִ����ȷʱ�ķ��ؽ��
		/// </summary>
		/// <param name="strValues">�����������ж��</param>
		protected void SetOKResult(params string[] strValues)
		{
			SetOKResult(_XmlResult, strValues);
		}

		/// <summary>
		/// ����ִ����ȷʱ�ķ��ؽ��
		/// </summary>
		/// <param name="xmlResult">��Ž����XML����</param>
		/// <param name="strValues">�����������ж��</param>
		protected void SetOKResult(XmlDocument xmlResult, params string[] strValues)
		{
			xmlResult.LoadXml("<ResponseOK />");

			XmlElement nodeRoot = xmlResult.DocumentElement;

			for (int i = 0; i < strValues.Length; i++)
			{
				XmlNode nodeValue = xmlResult.CreateNode(XmlNodeType.Element, "Value", "");
				nodeValue.InnerText = strValues[i];

				nodeRoot.AppendChild(nodeValue);
			}
		}

		/// <summary>
		/// ���ô�����Ϣ��Xml�ı�
		/// </summary>
		/// <param name="ex">�������</param>
		protected void SetErrorResult(System.Exception ex)
		{
			SetErrorResult(_XmlResult, ex);
		}

		/// <summary>
		/// ���ؽ����XML����
		/// </summary>
		protected XmlDocument _XmlResult = new XmlDocument();
		/// <summary>
		/// ����������XML����
		/// </summary>
		protected XmlDocument _XmlRequest = new XmlDocument();
	}
}
