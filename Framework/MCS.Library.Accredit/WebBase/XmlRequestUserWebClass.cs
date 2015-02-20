using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Security.Principal;
using System.Diagnostics;

using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// �������ڴ���XML Http Requestҳ��Ļ��ࣨҪ�����ݣ�
	/// </summary>
	public class XmlRequestUserWebClass : XmlRequestWebClass
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public XmlRequestUserWebClass()
		{
		}

		private ILogOnUserInfo _LogOnUserInfo = null;
		/// <summary>
		/// �û���¼��Ϣ���ݡ�������������ļ��������˵����û����򷵻ظõ����û���
		/// </summary>
		public ILogOnUserInfo LogOnUserInfo
		{
			get
			{
				if (_LogOnUserInfo == null)
					_LogOnUserInfo = GetLogOnUserInfoObject();

				return _LogOnUserInfo;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual ILogOnUserInfo GetLogOnUserInfoObject()
		{
			//			if (Session["logonUserInfo"] == null)
			//			{
			//				if (Context.User.Identity.Name.Length == 0)
			//				{
			//					Session["logonUserInfo"] = GlobalInfo.UserLogOnInfo;
			//					string strMsg = "�Բ���ϵͳû�л�ȡ�����ĵ�¼��Ϣ���ݣ������µ�¼ϵͳ��";
			//					HG.HGSupport.Common.CtrlException.TrueThrow(Session["logonUserInfo"] == null, strMsg);
			//				}
			//				else
			//					Session["logonUserInfo"] = new LogOnUserInfo(HttpContext.Current);
			//			}
			//
			//			return (ILogOnUserInfo)Session["logonUserInfo"];

			ILogOnUserInfo ilou = null;
			if (Context.User.Identity.Name.Length == 0)
			{
				ilou = GlobalInfo.UserLogOnInfo;
				string strMsg = "�Բ���ϵͳû�л�ȡ�����ĵ�¼��Ϣ���ݣ������µ�¼ϵͳ��";
				ExceptionHelper.TrueThrow(ilou == null, strMsg);
			}
			else
				ilou = new LogOnUserInfo(HttpContext.Current);

			return ilou;
		}

		private void SetUserPrincipal(string strUserName)
		{
			HttpContext context = HttpContext.Current;

			IIdentity id = new GenericIdentity(strUserName);
			string[] strRoles = { "" };
			context.User = new GenericPrincipal(id, strRoles);
		}

		/// <summary>
		/// ����ҳ����أ�������Page���OnLoad(e)�¼�
		/// </summary>
		/// <param name="e">�¼�����</param>
		protected override void OnLoad(EventArgs e)
		{
			_LogOnUserInfo = GetLogOnUserInfoObject();

			GlobalInfo.InitLogOnUser(_LogOnUserInfo);

			base.OnLoad(e);
		}

	}
}
