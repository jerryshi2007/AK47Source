using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Applications.AppAdmin_LOG.server;

namespace MCS.Applications.AppAdmin_LOG.logList
{
	/// <summary>
	/// UserLogList ��ժҪ˵����
	/// </summary>
	public partial class UserLogList : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
			Response.Cache.SetNoStore();
			secFrm.Value = Request.QueryString["secFrm"];

			string strSql = @"SELECT DISTINCT DISPLAYNAME 
					FROM APP_LOG_TYPE 
					WHERE VISIBLE = 'y' 
						AND CODE_NAME <> 'appall' 
					ORDER BY DISPLAYNAME";

			HiddenXml.DocumentContent = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSql)).DocumentElement.OuterXml;
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
