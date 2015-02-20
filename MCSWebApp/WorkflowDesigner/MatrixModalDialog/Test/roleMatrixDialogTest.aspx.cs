using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace WorkflowDesigner.MatrixModalDialog.Test
{
	public partial class roleMatrixDialogTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			schemaMatrixEntryControl.RoleID = TestConst.SchemaID;

			instanceMatrixEntryControl.RoleID = TestConst.InstanceID;
			instanceMatrixEntryControl.DefinitionID = schemaMatrixEntryControl.RoleID;

			instanceReadOnlyMatrixEntryControl.RoleID = TestConst.InstanceID;
			instanceReadOnlyMatrixEntryControl.DefinitionID = schemaMatrixEntryControl.RoleID;

			emptyMatrixEntryControl.RoleID = UuidHelper.NewUuidString();
			emptyMatrixEntryControl.DefinitionID = UuidHelper.NewUuidString();
		}
	}
}