using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Web.Library;

namespace WorkflowDesigner.ModalDialog
{
	public partial class ServicesAddressEditor : System.Web.UI.Page
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
			if (!IsPostBack)
			{
				BindingData();
			}
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			try
			{
				string key = this.opKey.Value.Trim();
				string address = this.txtServiceAddress.Text.Trim();
				string requestMethod = this.dropSendType.SelectedValue;
				string credentialKey = this.dropCredential.SelectedValue;
				WfNetworkCredential credential = new WfNetworkCredential();

				WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(ParametersKey);
				WfServiceAddressDefinition serviceAddress = parameters.ServiceAddressDefs[key];

				if (serviceAddress == null)
				{
					serviceAddress = new WfServiceAddressDefinition();
				}

				serviceAddress.Key = key;
				serviceAddress.Address = address;
				serviceAddress.RequestMethod = (WfServiceRequestMethod)Enum.Parse(typeof(WfServiceRequestMethod), requestMethod);
				serviceAddress.ContentType = (WfServiceContentType)Enum.Parse(typeof(WfServiceContentType), this.ddlContentType.SelectedValue);
				serviceAddress.Credential = parameters.Credentials.FirstOrDefault(p => p.Key == credentialKey);

				parameters.ServiceAddressDefs.Remove(p => p.Key == serviceAddress.Key);
				parameters.ServiceAddressDefs.Add(serviceAddress);
				parameters.Update();
				WebUtility.RefreshParentWindow();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void SetHiddenJsonData()
		{
			try
			{
				string Key = MCS.Web.Library.WebUtility.GetRequestParamString("key", string.Empty);
				if (!string.IsNullOrEmpty(Key))
				{
					WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(ParametersKey);
					WfServiceAddressDefinition def = parameters.ServiceAddressDefs[Key];
					if (def == null) return;
					//hidResultData.Value = JSONSerializerExecute.Serialize(parameters.ServiceAddressDefs.Find(p=>p.Address==Key));
					this.opKey.Value = def.Key;
					this.opKey.Disabled = true;
					this.txtServiceAddress.Text = def.Address;
					this.dropCredential.SelectedValue = def.Credential == null ? "" : def.Credential.Key;
					this.dropSendType.SelectedValue = def.RequestMethod.ToString();
					this.ddlContentType.SelectedValue = def.ContentType.ToString();
				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		private void BindingData()
		{
			this.dropSendType.DataSource = Enum.GetNames(typeof(WfServiceRequestMethod));
			this.dropSendType.DataBind();
			this.ddlContentType.DataSource = Enum.GetNames(typeof(WfServiceContentType));
			this.ddlContentType.DataBind();

			WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(ParametersKey);
			//parameters.Credentials
			this.dropCredential.DataSource = parameters.Credentials;
			this.dropCredential.DataTextField = "LogOnName";
			this.dropCredential.DataValueField = "Key";
			this.DataBind();
			this.dropCredential.Items.Insert(0, new ListItem() { Text = "空", Value = "" });
		}
	}
}