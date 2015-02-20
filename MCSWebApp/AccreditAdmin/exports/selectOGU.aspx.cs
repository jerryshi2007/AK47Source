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
	/// selectOGU ��ժҪ˵����
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
