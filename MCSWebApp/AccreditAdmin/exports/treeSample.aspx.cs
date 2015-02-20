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
	/// treeSample ��ժҪ˵����
	/// </summary>
	public partial class treeSample : WebBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (false == IsPostBack)
			{
				using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
				{
					DataSet ds = OGUReader.GetRankDefine(1, 1);
					orgSelectAccessLevel.DataSource = new DataView(ds.Tables[0]);
					orgSelectAccessLevel.DataTextField = "NAME";
					orgSelectAccessLevel.DataValueField = "CODE_NAME";
					orgSelectAccessLevel.DataBind();
					orgSelectAccessLevel.Items.Insert(0, new ListItem("--", string.Empty));

					ds = OGUReader.GetRankDefine(2, 1);
					userSelectAccessLevel.DataSource = new DataView(ds.Tables[0]);
					userSelectAccessLevel.DataTextField = "NAME";
					userSelectAccessLevel.DataValueField = "CODE_NAME";
					userSelectAccessLevel.DataBind();
					userSelectAccessLevel.Items.Insert(0, new ListItem("--", string.Empty));
				}
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
