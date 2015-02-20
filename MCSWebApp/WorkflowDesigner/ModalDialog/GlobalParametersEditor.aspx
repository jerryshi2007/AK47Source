<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GlobalParametersEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.GlobalParametersEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>全局参数</title>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
    <script type="text/javascript" src="../js/common.js"></script>
    <script type="text/javascript" src="../js/wfweb.js"></script>
	<base target="_self" />
    <script type="text/javascript">
        function openCredentialDialog() {
            var sFeature = "dialogWidth:800px; dialogHeight:560px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result;
            result = window.showModalDialog("NetworkCredentialList.aspx", null, sFeature);
        }

        function openServicesAddressDialog() {
            var sFeature = "dialogWidth:800px; dialogHeight:560px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            result = window.showModalDialog("ServicesAddressList.aspx", null, sFeature);
        }
    </script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">全局参数</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<SOA:PropertyGrid runat="server" ID="propertyGrid" Width="100%" Height="100%" DisplayOrder="ByCategory" />
				</div>
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
							<a href="#" onclick="openServicesAddressDialog();">服务地址定义</a>
						</td>
						<td style="text-align: center;">
							<a href="#" onclick="openCredentialDialog();">网络凭据定义</a>
						</td>
					</tr>
				</table> 
            </td>
        </tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<asp:Button AccessKey="O" runat="server" CssClass="formButton" Text="确定(O)" 
								ID="btnOK" onclick="btnOK_Click" />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel"
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
	</form>
</body>
</html>
