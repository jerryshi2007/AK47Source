using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// ��Http�����װΪCommand����������Ĺ����ӿ�
	/// </summary>
	public interface IRequestCommand
	{
		/// <summary>
		/// ��������
		/// </summary>
		string CommandName
		{
			get;
		}

		/// <summary>
		/// ִ�еĲ���
		/// </summary>
		void Execute();
	}

	/// <summary>
	/// ҳ����ת��Command�Ļ���
	/// </summary>
	public abstract class PageNavigateCommandBase : IRequestCommand
	{
		private string commandName = string.Empty;
		private WindowFeature feature = null;
		private string navigateUrl = null;

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="cmdName"></param>
		public PageNavigateCommandBase(string cmdName)
		{
			this.commandName = cmdName;
		}

		/// <summary>
		/// ��������
		/// </summary>
		public string CommandName
		{
			get
			{
				return this.commandName;
			}
		}

		/// <summary>
		/// Ŀ��Url
		/// </summary>
		public string NavigateUrl
		{
			get { return navigateUrl; }
			set { navigateUrl = value; }
		}

		/// <summary>
		/// ��������
		/// </summary>
		public WindowFeature Feature
		{
			get { return feature; }
			set { feature = value; }
		}

		/// <summary>
		/// ִ��
		/// </summary>
		public abstract void Execute();
	}

	/// <summary>
	/// ���ս����Redirect������ҳ���Command
	/// </summary>
	public class RedirectCommand : PageNavigateCommandBase
	{
		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="cmdName"></param>
		public RedirectCommand(string cmdName)
			: base(cmdName)
		{
		}

		#region IRequestCommand ��Ա

		/// <summary>
		/// ����ִ��
		/// </summary>
		public override void Execute()
		{
			if (this.Feature != null)
			{
				HttpContext.Current.Response.Write(this.Feature.ToAdjustWindowScriptBlock(true));
			}
			HttpContext.Current.Response.Write(DeluxeClientScriptManager.AddScriptTags(string.Format("window.navigate(\"{0}\");", WebUtility.CheckScriptString(NavigateUrl))));
			HttpContext.Current.Response.End();
		}

		#endregion
	}

	/// <summary>
	/// ���ս����Transfer������ҳ���Command
	/// </summary>
	public class TransferCommand : PageNavigateCommandBase
	{
		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="cmdName"></param>
		public TransferCommand(string cmdName)
			: base(cmdName)
		{
		}

		#region IRequestCommand ��Ա

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="feature"></param>
		public override void Execute()
		{
			//if (this.Feature != null)
			//{
			//    //����������ʽ������
			//    HttpContext.Current.Response.Write("<!DOCTYPE html>\n");
			//    HttpContext.Current.Response.Write(this.Feature.ToAdjustWindowScriptBlock(true));
			//    HttpContext.Current.Response.Flush();                
			//}

			PageHandlerFactory factory = (PageHandlerFactory)Activator.CreateInstance(typeof(System.Web.UI.PageHandlerFactory), true);

			Uri url = new Uri(HttpContext.Current.Request.Url, this.NavigateUrl);

			IHttpHandler handler = factory.GetHandler(HttpContext.Current,
				HttpContext.Current.Request.RequestType,
				url.AbsolutePath, HttpContext.Current.Request.PhysicalApplicationPath);

			if (handler is Page)
			{
				HttpContext.Current.Items["transferTarget"] = AppendCurrentRequestParams(this.NavigateUrl);

				((Page)handler).PreRenderComplete += new EventHandler(TransferCommand_PreRenderComplete);

				//add by zhouweihai 2008-09-02 ҳ�����PageModule
				WebUtility.AttachPageModules((Page)handler);
			}

			StringBuilder strB = new StringBuilder(10240);

			TextWriter tw = new StringWriter(strB);

			HttpContext.Current.Server.Execute(handler, tw, true);

			HttpContext.Current.Response.Write(tw.ToString());
			HttpContext.Current.Response.End();
		}
		#endregion

		private void TransferCommand_PreRenderComplete(object sender, EventArgs e)
		{
			Page page = (Page)HttpContext.Current.CurrentHandler;

			if (page.Form != null)
			{
				page.ClientScript.RegisterClientScriptBlock(page.GetType(),
						"RequestCommand",
						string.Format("{0}.action = \"{1}\";", page.Form.ClientID, HttpContext.Current.Items["transferTarget"]),
						true);
			}

			if (page.Header != null)
			{
				if (this.Feature != null)
					page.Header.Controls.Add(new LiteralControl(this.Feature.ToAdjustWindowScriptBlock(true)));
			}

			//delete by zhouweihai 2008-09-02 
			//WebUtility.LoadConfigPageContent(true);
		}

		private Exception GetActualException(Exception ex)
		{
			Exception result = ex;

			while (ex != null)
			{
				if (ex.InnerException != null)
				{
					if (ex is HttpUnhandledException)
					{
						result = ex.InnerException;
						break;
					}
					else
						ex = ex.InnerException;
				}
				else
				{
					result = ex;
					break;
				}
			}

			return result;
		}

		private string AppendCurrentRequestParams(string targetUrl)
		{
			string result = targetUrl;

			string currentUrl = HttpContext.Current.Request.Url.ToString();

			int splitterIndex = currentUrl.IndexOf('?');

			if (splitterIndex != -1)
			{
				if (targetUrl.IndexOf('?') != -1)
					result += "&";
				else
					result += "?";

				result += currentUrl.Substring(splitterIndex + 1);
			}

			return result;
		}
	}
}
