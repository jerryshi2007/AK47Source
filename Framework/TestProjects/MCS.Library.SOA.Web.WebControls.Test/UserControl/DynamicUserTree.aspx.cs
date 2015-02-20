using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.UserControl
{
	public partial class DynamicUserTree : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			userTreeContainer.Controls.Add(CreateNormalControl());
			userTreeContainer.Controls.Add(CreateDialogControl());

			base.OnPreRender(e);
		}

		protected void userSelector_LoadingObjectToTreeNode(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel)
		{
			if (oguObj.ObjectType == SchemaType.Groups)
				cancel = true;
		}

		private static UserOUGraphControl CreateNormalControl()
		{
			UserOUGraphControl control = new UserOUGraphControl();

			control.Width = Unit.Pixel(300);
			control.Height = Unit.Pixel(400);
			control.ShowingMode = ControlShowingMode.Normal;
			control.RootExpanded = true;
			control.BorderStyle = BorderStyle.Solid;
			control.BorderWidth = Unit.Pixel(1);
			control.SelectMask = UserControlObjectMask.Organization;
			control.ListMask = UserControlObjectMask.All;

			return control;
		}

		private static UserOUGraphControl CreateDialogControl()
		{
			UserOUGraphControl control = new UserOUGraphControl();

			control.ShowingMode = ControlShowingMode.Dialog;
			control.RootExpanded = true;
			control.SelectMask = UserControlObjectMask.Organization;
			control.ListMask = UserControlObjectMask.All;
			control.ControlIDToShowDialog = "showDialogBtn";

			return control;
		}
	}
}