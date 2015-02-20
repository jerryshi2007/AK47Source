using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Core;

namespace MCS.Applications.AppAdmin
{
	/// <summary>
	/// AppAdmin ��ժҪ˵����
	/// </summary>
	public partial class AppAdmin : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
			XmlDocument xmlUserInfo = new XmlDocument();
			xmlUserInfo.LoadXml("<UserInfo/>");
			XmlHelper.AppendNode( xmlUserInfo.FirstChild, "UserGuid", LogOnUserInfo.UserGuid);
			XmlHelper.AppendNode( xmlUserInfo.FirstChild, "UserLogOnName", LogOnUserInfo.UserLogOnName);
			for (int i = 0; i < LogOnUserInfo.OuUsers.Length; i++)
			{
				XmlNode OuUsersNode = XmlHelper.AppendNode( xmlUserInfo.FirstChild, "OuUsers");

				XmlHelper.AppendNode( OuUsersNode, "UserGuid", LogOnUserInfo.OuUsers[i].UserGuid);
				XmlHelper.AppendNode( OuUsersNode, "AllPathName", LogOnUserInfo.OuUsers[i].AllPathName);
				XmlHelper.AppendNode( OuUsersNode, "UserDisplayName", LogOnUserInfo.OuUsers[i].UserDisplayName);
				XmlHelper.AppendNode( OuUsersNode, "UserObjName", LogOnUserInfo.OuUsers[i].UserObjName);
				XmlHelper.AppendNode( OuUsersNode, "Sideline", LogOnUserInfo.OuUsers[i].Sideline.ToString());
			}

			if (SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName))
				XmlHelper.AppendAttr( xmlUserInfo.FirstChild, "AdminUser", "true");
			else
				XmlHelper.AppendAttr( xmlUserInfo.FirstChild, "AdminUser", "false");
			
			userInfo.Value = xmlUserInfo.OuterXml;
		}

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
