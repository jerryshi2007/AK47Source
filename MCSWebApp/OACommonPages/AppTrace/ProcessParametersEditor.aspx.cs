using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Library.MVC;
using MCS.Library.Principal;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class ProcessParametersEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (CurrentProcess != null)
			{
				PropertyValueCollection properties = ObjectToPropertiesHelper.ToProperties<string, object>(CurrentProcess.ApplicationRuntimeParameters);

				this.propertyGrid.Properties.Clear();
				this.propertyGrid.Properties.CopyFrom(properties);

				btnSave.Visible = WfClientContext.IsProcessAdmin(DeluxeIdentity.CurrentUser, CurrentProcess);
			}

			autoClose.Value = WebUtility.GetRequestQueryValue("autoClose", false).ToString();
		}

		private IWfProcess CurrentProcess
		{
			get
			{
				IWfProcess process = null;

				string processID = WebUtility.GetRequestParamString("processID", string.Empty);

				if (processID.IsNotEmpty())
					process = WfRuntime.GetProcessByProcessID(processID);

				return process;
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (CurrentProcess != null)
				{
					this.propertyGrid.Properties.FillDictionary(CurrentProcess.ApplicationRuntimeParameters);

					WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(CurrentProcess);
					WfRuntime.PersistWorkflows();

					PropertyValueCollection properties = ObjectToPropertiesHelper.ToProperties<string, object>(CurrentProcess.ApplicationRuntimeParameters);

					responseData.Value = JSONSerializerExecute.Serialize(properties);

					Page.ClientScript.RegisterStartupScript(this.GetType(), "notifyTop", "notifyTopWindowDataChange();", true);
				}
			}
			catch (System.Exception ex)
			{
				responseData.Value = string.Empty;

				Page.ClientScript.RegisterStartupScript(this.GetType(), "notifyTop", "notifyTopWindowDataChange();", true);

				WebUtility.RegisterClientErrorMessage(ex);
			}
		}
	}
}