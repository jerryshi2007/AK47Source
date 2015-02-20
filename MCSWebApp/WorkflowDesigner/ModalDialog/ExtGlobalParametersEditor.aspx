<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtGlobalParametersEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.ExtGlobalParametersEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>全局参数</title>
	<script type="text/javascript">
		var m_currentParameters = null;
		var m_originalParameters = null;

		function propertiesModified() {
			var result = false;

			if (m_currentParameters != null && m_originalParameters != null) {
				for (var i = 0; i < m_currentParameters.Properties.length; i++) {
					var cp = m_currentParameters.Properties[i];
					var op = findOriginalProperty(cp, m_originalParameters.Properties);

					if (op != null) {
						if (comparePropertyValue(cp.value, op.value, cp.dataType) == false) {
							result = true;
							break;
						}
					}
				}
			}

			return result;
		}

		function comparePropertyValue(currentValue, originalValue, dataType) {
			var result = false;

			switch (dataType) {
				case $HGRootNS.PropertyDataType.Boolean:
					result = currentValue.toString().toLowerCase() == originalValue.toString().toLowerCase();
					break;
				default:
					result = currentValue == originalValue;
			}

			return result;
		}

		function findOriginalProperty(cp, originalProperties) {
			var result = null;

			for (var i = 0; i < originalProperties.length; i++) {
				if (originalProperties[i].name == cp.name) {
					result = originalProperties[i];
					break;
				}
			}

			return result;
		}

		function treeNodeSelecting(sender, e) {
			e.cancel = true;

			if (e.node.get_extendedData() == "Program") {
				if (m_currentParameters == null || m_currentParameters.Key != e.node.get_value()) {
					var needAbort = true;

					if (propertiesModified()) {
						needAbort = window.confirm("属性值已经改变，是否放弃？");

						e.cancel = needAbort == false;
					}

					if (needAbort == true)
						WorkflowDesigner.Services.ServiceForClient.GetGlobalParametersJSON(e.node.get_value(), onGetPropertiesCompleted, onServiceError, e.node);
				}
			}
			else
				e.node.set_expanded(!e.node.get_expanded());
		}

		function saveProperties(currentParameters, userContext) {
			var propertiesJson = Sys.Serialization.JavaScriptSerializer.serialize(currentParameters.Properties);

			WorkflowDesigner.Services.ServiceForClient.UpdateGlobalParameters(currentParameters.Key, propertiesJson, onUpdateGlobalParametersCompleted, onServiceError, userContext);

			setInvokeServiceStatus(true);
		}

		function onUpdateGlobalParametersCompleted(result, userContext) {
			var data = Sys.Serialization.JavaScriptSerializer.serialize(m_currentParameters);
			m_originalParameters = Sys.Serialization.JavaScriptSerializer.deserialize(data);

			var node = userContext;

			if (node) {
				node.set_selected(true);

				window.setTimeout(function () {
					WorkflowDesigner.Services.ServiceForClient.GetGlobalParametersJSON(node.get_value(), onGetPropertiesCompleted, onServiceError, userContext);
				}, 1);
			}

			setInvokeServiceStatus(false);
		}

		function onGetPropertiesCompleted(result, userContext) {
			bindCurrentData(result);

			var node = userContext;

			if (node) {
				node.set_selected(true);
			}
		}

		function bindCurrentData(json) {
			if (json != null && json != "") {
				var pe = $find("propertyGrid");

				var data = Sys.Serialization.JavaScriptSerializer.deserialize(json);

				pe.set_caption(data.Name);
				pe.set_properties(data.Properties);
				pe.dataBind();

				m_currentParameters = data;
				m_originalParameters = Sys.Serialization.JavaScriptSerializer.deserialize(json);
			}
		}

		function onServiceError(e, userContext) {
			$showError(e);
			setInvokeServiceStatus(false);
		}

		function onSaveButtonClick() {
			saveProperties(m_currentParameters, null);
		}

		function setInvokeServiceStatus(invoking) {
			if (invoking) {
				$get("btnSave").disabled = true;
			}
			else {
				$get("btnSave").disabled = false;
			}
		}

		function openCredentialDialog() {
			var sFeature = "dialogWidth:800px; dialogHeight:560px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			window.showModalDialog("NetworkCredentialList.aspx", null, sFeature);
		}

		function openServicesAddressDialog() {
			var sFeature = "dialogWidth:800px; dialogHeight:560px;center:yes;help:no;resizable:no;scroll:no;status:no";

			window.showModalDialog("ServicesAddressList.aspx", null, sFeature);
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="True">
		<Services>
			<asp:ServiceReference Path="../Services/ServiceForClient.asmx" />
		</Services>
	</asp:ScriptManager>
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">全局参数</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="border-right: solid 1px #ccc; vertical-align: top">
				<table style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
					<tr>
						<td style="width: 280px; border-right: 1px solid silver">
							<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
								height: 100%; overflow: auto">
								<MCS:DeluxeTree ID="tree" runat="server" BorderStyle="None" NodeCloseImg="../images/closeImg.gif"
									NodeOpenImg="../images/openImg.gif" OnGetChildrenData="tree_GetChildrenData"
									OnNodeSelecting="treeNodeSelecting">
								</MCS:DeluxeTree>
							</div>
						</td>
						<td>
							<SOA:PropertyGrid runat="server" ID="propertyGrid" Width="100%" Height="100%" DisplayOrder="ByCategory" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="width: 50%">
							<table style="width: 100%; height: 100%">
								<tr>
									<td style="text-align: center;">
										<a href="#" onclick="openServicesAddressDialog();">服务地址定义</a>
									</td>
									<td style="text-align: center;">
										<a href="#" onclick="openCredentialDialog();">网络凭据定义</a>
									</td>
								</tr>
							</table>
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="onSaveButtonClick();" value="保存(S)"
								id="btnSave" accesskey="S" />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<div style="display: none">
		<iframe name="innerFrame"></iframe>
	</div>
	<script type="text/javascript">
		Sys.Application.add_load(function () {
			var json = $get("initProperties").value;

			bindCurrentData(json);
		});
	</script>
	</form>
</body>
</html>
