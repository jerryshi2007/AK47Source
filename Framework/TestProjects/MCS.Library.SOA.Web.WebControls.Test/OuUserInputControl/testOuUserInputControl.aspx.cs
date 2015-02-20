using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//using MCS.Library.Workflow.Configuration;
//using MCS.Library.Workflow.Descriptors;
//using MCS.Library.Workflow.OA.Descriptors;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

using System.Web.UI.HtmlControls;
using System.Reflection;

using MCS.Library.SOA.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl
{
	public partial class testOuUserInputControl : System.Web.UI.Page
	{
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			/*
			this.WfResourceInputControl1.CompareFieldName = ("name").Split(',');
			this.WfResourceInputControl1.MinimumPrefixLength = 1;
			this.WfResourceInputControl1.DataValueField = "id";
			this.WfResourceInputControl1.DataTextFieldList = ("name").Split(',');

			WfResourceInputControl foIC = new WfResourceInputControl();
			foIC.CompareFieldName = ("name").Split(',');
			foIC.MinimumPrefixLength = 1;
			foIC.DataValueField = "id";
			foIC.DataTextFieldList = ("name").Split(',');
			foIC.ID = "newOBJ";
			foIC.Width = Unit.Pixel(60);
			foIC.Attributes.Add("OnGetData", "WfResourceInputControl1_GetData");
			foIC.Attributes.Add("OnValidateInput", "WfResourceInputControl1_ValidateInput");

			foIC.GetData += new WfResourceInputControl.GetDataEventHandler(WfResourceInputControl1_GetData);
			foIC.ValidateInput += new WfResourceInputControl.ValidateInputEventHandler(WfResourceInputControl1_ValidateInput);

			this.form1.Controls.Add(foIC);
			 */

            
		}

		protected void postBackAndSetReadOnly_Click(object sender, EventArgs e)
		{
			//object a=this.OuUserInputControl1.SelectedOuUserData;
			this.OuUserInputControl1.ReadOnly = true;
		}

		/*
		protected void WfResourceInputControl1_GetData(WfResourceInputControl resourceInput, WfResourceInputControlAutoCompleteEventArgs args)
		{
			args.ReturnValue = InnerBuildResourceCollection();
		}

		private WfResourceDataCollection InnerBuildResourceCollection()
		{
			WfResourceDataCollection collection = new WfResourceDataCollection();

			ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications("WorkflowTest");

			ExceptionHelper.FalseThrow(apps.Count > 0, "Cannot find this applicaiton");
			ExceptionHelper.FalseThrow(apps[0].Roles.ContainsKey("ROLE1"), "Cannot find this role");

			ExtendedRole role = new ExtendedRole(apps[0].Roles["ROLE1"]);
			role.Scope = new MCS.OA.DataObjects.OguDataCollection<IOrganization>(OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath,
				"中国海关\\青岛海关\\法规处",
				"中国海关\\青岛海关\\缉私局\\办公室"));

			collection.Add(role);

			//石正进是兼职的人员，另一FullPath为 中国海关\青岛海关\办公室\室领导\石正进 
			OguObjectCollection<IOguObject> list = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.FullPath,
				"中国海关\\青岛海关\\办公室\\办公自动化科\\石云峰",
				"中国海关\\青岛海关\\流亭机场海关\\关领导\\石正进",
				"中国海关\\青岛海关\\人事处",
				"中国海关\\青岛海关\\缉私局");

			ExceptionHelper.TrueThrow(list.Count == 0, "未按照输入查找出与之符合的任一个机构人员对象");

			for (int i = 0; i < list.Count; i++)
			{
				object o = null;
				if (list[0].ObjectType == MCS.Library.OGUPermission.SchemaType.Users)
					o = new OguUser((IUser)list[0]);
				else
					o = new OguOrganization((IOrganization)list[0]);
				collection.Add(o);
			}
			return collection;

		}

		protected void WfResourceInputControl1_ValidateInput(WfResourceInputControl resourceInput, WfResourceInputControlValidateInputEventArgs args)
		{
			args.ReturnValue = InnerBuildResourceCollection();
		}
		*/
		protected void Button2_Click(object sender, EventArgs e)
		{
			if (OuUserInputControl1.SelectedOuUserData.Count > 0)
				Response.Write(this.OuUserInputControl1.SelectedOuUserData[0].DisplayName);
		}
	}
}
