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

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.Configuration;

namespace AccreditAdmin.itemList
{
	/// <summary>
	/// appObjectList ��ժҪ˵����
	/// </summary>
	public partial class appObjectList : WebBaseClass 
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			secFrm.Value = Request.QueryString["secFrm"];

			int iMaxCount = AccreditSection.GetConfig().AccreditSettings.AppListMaxCount;

			listMaxCount.Value = iMaxCount.ToString();
			roleCodeName.Value = Request.QueryString["roleCodeName"];
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN���õ����� ASP.NET Web ���������������ġ�
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
