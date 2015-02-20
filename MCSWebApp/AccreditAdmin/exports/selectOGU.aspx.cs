#region using 

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
using MCS.Library.Core;
#endregion

namespace MCS.Applications.AccreditAdmin.exports
{
	/// <summary>
	/// selectOGU 的摘要说明。
	/// </summary>
	public partial class selectOGU : WebBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.LoadXml("<Params />");

			foreach (string strKey in Request.QueryString.Keys)
			{
				XmlHelper.AppendNode(xmlDoc.DocumentElement, strKey, Request.QueryString[strKey]);
			}

			if (xmlDoc.DocumentElement.ChildNodes.Count > 0)
				requestParam.Value = xmlDoc.OuterXml;
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
