using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Data.Builder;

using MCS.Applications.AppAdmin_LOG.server;

namespace MCS.Applications.AppAdmin_LOG.logList
{
	/// <summary>
	/// UserLogDetail ��ժҪ˵����
	/// </summary>
	public partial class UserLogDetail : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
			string sortID = GetRequestData("sortID", "0").ToString();

			string strSql = @"SELECT UOL.*, ALT.DISPLAYNAME AS APP_DISPLAYNAME, AOT.DISPLAYNAME AS OP_DISPLAYNAME 
				FROM USER_OPEATION_LOG UOL, APP_LOG_TYPE ALT, APP_OPERATION_TYPE AOT 
				WHERE ALT.GUID = AOT.APP_GUID 
					AND UOL.APP_GUID = ALT.GUID 
					AND UOL.OP_GUID = AOT.GUID 
					AND UOL.ID = " + TSqlBuilder.Instance.CheckQuotationMark(sortID, true);

			XmlDocument doc = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSql));
			
			SetControlValue(doc.DocumentElement.FirstChild);			
		}

		private void SetControlValue(XmlNode firstNode)
		{
			OP_USER_DISPLAYNAME.InnerText = firstNode.SelectSingleNode("OP_USER_DISPLAYNAME").InnerText;
			HOST_IP.InnerText = firstNode.SelectSingleNode("HOST_IP").InnerText;
			//HOST_NAME.InnerText = firstNode.SelectSingleNode("HOST_NAME").InnerText;
			APP_DISPLAYNAME.InnerText = firstNode.SelectSingleNode("APP_DISPLAYNAME").InnerText;
			OP_DISPLAYNAME.InnerText = firstNode.SelectSingleNode("OP_DISPLAYNAME").InnerText;
			OP_URL.InnerText = firstNode.SelectSingleNode("OP_URL").InnerText;

			OP_USER_DISTINCTNAME.InnerText = firstNode.SelectSingleNode("OP_USER_DISTINCTNAME").InnerText;
			OP_USER_LOGONNAME.InnerText = firstNode.SelectSingleNode("OP_USER_LOGONNAME").InnerText;
			
			GOAL_EXPLAIN.InnerText = firstNode.SelectSingleNode("GOAL_EXPLAIN").InnerText;
			LOG_DATE.InnerText = firstNode.SelectSingleNode("LOG_DATE").InnerText;
			ORIGINAL_DATA.InnerText = firstNode.SelectSingleNode("ORIGINAL_DATA").InnerText;
			if (firstNode.SelectSingleNode("LOG_SUCCED").InnerText == "y")
                LOG_SUCCED.InnerText = "��";	else LOG_SUCCED.InnerText = "��";
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
