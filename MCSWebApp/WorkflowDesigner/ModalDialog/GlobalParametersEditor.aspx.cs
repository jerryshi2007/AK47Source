using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ModalDialog
{
	public partial class GlobalParametersEditor : System.Web.UI.Page
	{

		protected void Page_Load(object sender, EventArgs e)
		{
            PropertyEditorRegister();
		}

        private void PropertyEditorRegister()
        {
            PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
            PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
        }

        protected override void OnPreRender(EventArgs e)
        {
            propertyGrid.Properties.Clear();
            propertyGrid.Properties.CopyFrom(WfGlobalParameters.LoadProperties("Default").Properties);

            base.OnPreRender(e);
        }

		protected void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				WfGlobalParameters parameters = WfGlobalParameters.LoadDefault();

				parameters.Properties.Clear();
				parameters.Properties.CopyFrom(propertyGrid.Properties);

				parameters.Update();

				WebUtility.ResponseCloseWindowScriptBlock();
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}
	}
}