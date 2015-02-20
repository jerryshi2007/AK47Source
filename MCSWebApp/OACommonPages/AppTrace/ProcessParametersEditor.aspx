<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessParametersEditor.aspx.cs"
	Inherits="MCS.OA.CommonPages.AppTrace.ProcessParametersEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title runat="server" category="OACommons">流程参数编辑器</title>
	<base target="_self" />
	<script type="text/javascript">
		function notifyTopWindowDataChange() {
			top.document.getElementById("responseData").value = $get("responseData").value;
			top.document.getElementById("notifyButton").click();
		}

		function onDataChange() {
			SubmitButton.resetAllStates();

			var data = $get("responseData").value;

			if (data.length > 0) {
				if ($get("autoClose").value.toLowerCase() == "true") {
					top.returnValue = true;
					top.close();
				}

				var propertyGrid = $find("propertyGrid");

				propertyGrid.set_properties(Sys.Serialization.JavaScriptSerializer.deserialize(data));
				propertyGrid.dataBind();
			}

			window.setTimeout(function () {
				$get("frameContainer").innerHTML = "<iframe name='innerFrame'></iframe>";
			}, 10);
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<div>
		<asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="True">
		</asp:ScriptManager>
		<input runat="server" id="autoClose" type="hidden" />
		<input runat="server" id="responseData" type="hidden" />
		<table style="height: 100%; width: 100%">
			<tr>
				<td class="gridHead">
					<div class="dialogTitle">
					    <mcs:TranslatorLabel runat="server" CssClass="dialogLogo" Category="OACommons" Text="流程参数编辑"></mcs:TranslatorLabel>
					</div>
				</td>
			</tr>
			<tr>
				<td style="border-right: solid 1px #ccc; vertical-align: top">
					<soa:PropertyGrid runat="server" ID="propertyGrid" Width="100%" Height="100%" DisplayOrder="ByCategory" />
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
							<td style="text-align: center;">
								<soa:SubmitButton runat="server" ID="btnSave" AccessKey="S" CssClass="formButton"
									PopupCaption="正在保存..." Visible="false" Text="保存(S)" OnClick="btnSave_Click" category="OACommons" />
								<input type="button" style="display: none" id="notifyButton" onclick="onDataChange();" />
							</td>
							<td style="text-align: center;">
								<input type="button" runat="server" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel" category="OACommons"
									accesskey="C" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		<div style="display: none" id="frameContainer">
			<iframe name="innerFrame"></iframe>
		</div>
	</div>
	</form>
</body>
</html>
