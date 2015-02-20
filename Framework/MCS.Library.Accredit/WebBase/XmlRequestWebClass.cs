using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using MCS.Library.Core;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// 后台处理XMLHTTP请求的WEB基类（不带身份）
	/// </summary>
	public class XmlRequestWebClass : WebBaseClass
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public XmlRequestWebClass()
		{
		}


		/// <summary>
		/// 检查权限，重载基类的相应函数
		/// </summary>
		protected override void CheckPrivilege()
		{
		}

		/// <summary>
		/// 页面的加载过程结束，可以重载
		/// </summary>
		protected override void PageLoadFinally()
		{
			Response.End();
		}

		/// <summary>
		/// 得到请求的根节点名称
		/// </summary>
		protected string RootName
		{
			get
			{
				return _XmlRequest.DocumentElement.Name;
			}
		}

		/// <summary>
		/// 得到请求的XML命令名称
		/// </summary>
		protected string CommandName
		{
			get
			{
				XmlNode nodeCmd = GetCommandNode();

				XmlNode nodeAttr = nodeCmd.Attributes.GetNamedItem("name");

				ExceptionHelper.TrueThrow<ApplicationException>(nodeAttr == null, "非法的XML命令格式，没有命令名称");

				return nodeAttr.Value.ToString();
			}
		}

		/// <summary>
		/// 参数个数
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
		/// 取命令串中某一个参数的值
		/// </summary>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		protected string ParamValue(int nIndex)
		{
			XmlNode nodeCmd = GetCommandNode();

			return nodeCmd.ChildNodes[nIndex].InnerText;
		}

		/// <summary>
		/// 得到命令节点（所有参数的父节点）
		/// </summary>
		/// <returns>表示命令的XML节点</returns>
		protected XmlNode GetCommandNode()
		{
			XmlNode nodeCmd = _XmlRequest.SelectSingleNode(".//COMMAND");

			ExceptionHelper.TrueThrow<ApplicationException>(nodeCmd == null, "非法的XML命令格式");

			return nodeCmd;
		}

		/*
		 * 重载部分
		 * 
		 * */
		/// <summary>
		/// 重载页面加载
		/// </summary>
		/// <param name="e">事件对象</param>
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
		/// 设置执行正确时的返回结果
		/// </summary>
		/// <param name="strValues">参数，可以有多个</param>
		protected void SetOKResult(params string[] strValues)
		{
			SetOKResult(_XmlResult, strValues);
		}

		/// <summary>
		/// 设置执行正确时的返回结果
		/// </summary>
		/// <param name="xmlResult">存放结果的XML对象</param>
		/// <param name="strValues">参数，可以有多个</param>
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
		/// 设置错误信息的Xml文本
		/// </summary>
		/// <param name="ex">例外对象</param>
		protected void SetErrorResult(System.Exception ex)
		{
			SetErrorResult(_XmlResult, ex);
		}

		/// <summary>
		/// 返回结果的XML对象
		/// </summary>
		protected XmlDocument _XmlResult = new XmlDocument();
		/// <summary>
		/// 浏览器请求的XML对象
		/// </summary>
		protected XmlDocument _XmlRequest = new XmlDocument();
	}
}
