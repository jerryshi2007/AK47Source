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
	/// UserLogDetail 的摘要说明。
	/// </summary>
	public partial class UserLogDetail : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在此处放置用户代码以初始化页面
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
                LOG_SUCCED.InnerText = "是";	else LOG_SUCCED.InnerText = "否";
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
