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

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Data;
using MCS.Applications.AccreditAdmin.Properties;
#endregion

namespace MCS.Applications.AccreditAdmin.exports
{
	/// <summary>
	/// selectObjsToRole ��ժҪ˵����
	/// </summary>
	public partial class selectObjsToRole : WebBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (false == IsPostBack)
			{
				InitPageObject();
			}
		}

		private void InitPageObject()
		{
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				DataSet ds = OGUReader.GetRankDefine(2, 1);

				USER_RANK_SEARCH.DataSource = new DataView(ds.Tables[0]);
				USER_RANK_SEARCH.DataTextField = "NAME";
				USER_RANK_SEARCH.DataValueField = "CODE_NAME";
				USER_RANK_SEARCH.DataBind();
				USER_RANK_SEARCH.Items.Insert(0, new ListItem("--", ""));

				USER_RANK.DataSource = new DataView(ds.Tables[0]);
				USER_RANK.DataTextField = "NAME";
				USER_RANK.DataValueField = "CODE_NAME";
				USER_RANK.DataBind();
				USER_RANK.Items.Insert(0, new ListItem("--", ""));

				ds = OGUReader.GetRankDefine(1, 1);
				ORGANIZATION_RANK_SEARCH.DataSource = new DataView(ds.Tables[0]);
				ORGANIZATION_RANK_SEARCH.DataTextField = "NAME";
				ORGANIZATION_RANK_SEARCH.DataValueField = "CODE_NAME";
				ORGANIZATION_RANK_SEARCH.DataBind();
				ORGANIZATION_RANK_SEARCH.Items.Insert(0, new ListItem("--", ""));
			}
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
