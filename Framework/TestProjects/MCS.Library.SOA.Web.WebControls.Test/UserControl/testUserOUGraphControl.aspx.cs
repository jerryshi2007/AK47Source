using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.UserControl
{
	using MCS.Library.OGUPermission;
	using System.Text;

	public partial class testUserOUGraphControl : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			UserOUGraphDialogControl.Root = OguMechanismFactory.GetMechanism().GetRoot();
		}

		protected void userSelector_LoadingObjectToTreeNode(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel)
		{
			if (oguObj.ObjectType == SchemaType.Groups)
				cancel = true;
		}

		protected override void OnPreRender(EventArgs e)
		{
			StringBuilder strB = new StringBuilder();

			AddSelectedData(this.userSelector, strB);
			AddSelectedData(this.UserOUGraphDialogControl, strB);
			AddSelectedData(this.userMultiSelector, strB);
			AddSelectedData(this.UserMultiSelectOUGraphDialogControl, strB);

			this.result.InnerHtml = strB.ToString();

			base.OnPreRender(e);
		}

		private void AddSelectedData(UserOUGraphControl ctrl, StringBuilder strB)
		{
			for (int i = 0; i < ctrl.SelectedOuUserData.Count; i++ )
			{
				if (strB.Length > 0)
					strB.Append("<BR/>");

				strB.Append(ctrl.SelectedOuUserData[i].FullPath);
			}
		}
	}
}
