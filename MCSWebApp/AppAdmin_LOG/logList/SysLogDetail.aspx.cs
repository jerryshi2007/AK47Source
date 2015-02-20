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

using MCS.Library.Accredit.WebBase;
using MCS.Library.Data.Builder;
using MCS.Library.Core;

using MCS.Applications.AppAdmin_LOG.server;

namespace MCS.Applications.AppAdmin_LOG.logList
{
	/// <summary>
	/// SysLogDetail 的摘要说明。
	/// </summary>
	public partial class SysLogDetail : WebUserBaseClass
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl USERDN;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在此处放置用户代码以初始化页面
			string sortID = GetRequestData("sortID", "0").ToString();

			string strSql = @"SELECT * FROM SYS_USER_LOGON WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(sortID, true);

			XmlDocument doc = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSql));
			
			SetControlValue(doc.DocumentElement.FirstChild);
			
		}

		private void SetControlValue(XmlNode firstNode)
		{
			USER_LOGONNAME.InnerText = firstNode.SelectSingleNode("USER_LOGONNAME").InnerText;
			HOST_IP.InnerText = firstNode.SelectSingleNode("HOST_IP").InnerText;
			IE_VERSION.InnerText = firstNode.SelectSingleNode("IE_VERSION").InnerText;
			WINDOWS_VERSION.InnerText = firstNode.SelectSingleNode("WINDOWS_VERSION").InnerText;
			KILL_VIRUS.InnerText = firstNode.SelectSingleNode("KILL_VIRUS").InnerText;
			STATUS.InnerText = firstNode.SelectSingleNode("STATUS").InnerText;
			HOST_NAME.InnerText = firstNode.SelectSingleNode("HOST_NAME").InnerText;
			LOG_DATE.InnerText = firstNode.SelectSingleNode("LOG_DATE").InnerText;
			USER_DISTINCTNAME.InnerText = firstNode.SelectSingleNode("USER_DISTINCTNAME").InnerText;
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
