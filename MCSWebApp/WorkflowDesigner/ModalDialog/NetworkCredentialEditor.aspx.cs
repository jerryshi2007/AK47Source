using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.Library.Resources;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner.ModalDialog
{
	public partial class NetworkCredentialEditor : System.Web.UI.Page
	{
        const string ParametersKey = "Default";
        string Key = string.Empty;
        protected override void OnPreRender(EventArgs e)
        {
            SetHiddenJsonData();

            base.OnPreRender(e);
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			//WebUtility.RequiredScript(typeof(ClientMsgResources));
		}

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string key = this.txtKey.Value;
            string logonName = this.txtLogonName.Value;
            string password = this.txtPassword.Value;

            WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(ParametersKey);
            WfNetworkCredential credential = parameters.Credentials.Find(obj => obj.Key == key);

            if (credential == null)
            {
                credential = new WfNetworkCredential();
                credential.Key = key;
            }

            credential.LogOnName = logonName;
            credential.Password = password;

            parameters.Credentials.Remove(obj => obj.Key == key);
            parameters.Credentials.Add(credential);
            parameters.Update();
        }

        private void SetHiddenJsonData()
        {
            try
            {
                string Key = MCS.Web.Library.WebUtility.GetRequestParamString("key", string.Empty);
                if (!string.IsNullOrEmpty(Key))
                {
                    WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(ParametersKey);
                    WfNetworkCredential obj = parameters.Credentials.Find(p => p.Key == Key);
                    this.txtKey.Value = obj.Key;
                    this.txtLogonName.Value = obj.LogOnName;
                }
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }
	}
}