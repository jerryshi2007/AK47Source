<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessKeyEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.ProcessKeyEditor" EnableEventValidation="false" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self">
	<title>请输入流程模板的Key</title>
    <script type="text/javascript" src="../js/common.js"></script>
	<script type="text/javascript" src="../js/wfweb.js"></script>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.validate.js"></script>
	<script type="text/javascript">
    function onSubmit() {
        if (window.dialogArguments != null) {
            var procKey = document.getElementById("processKey").value;

            if (!WFWeb.Utils.checkInputKey(procKey)) {
            	alert('流程模板Key只能由字母、数字、下划线组成');
            	return false;
            }

            if (procKey.length > 64) {
                alert('流程模板Key长度不能大于64');
                return false;
            }

            if (window.dialogArguments.has(procKey)) {
                alert("流程模板Key已存在！");
                return false;
            }
        }
    }

    </script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame" onsubmit="return onSubmit();">
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请输入流程模板的Key</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: center">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
					<table width="100%" style="height: 100%; width: 100%">
						<tr>
							<td class="label">
								流程模板的Key
							</td>
							<td>
								<input type="text" maxlength="64" id="processKey" runat="server" />
							</td>
						</tr>
                        <tr>
							<td class="label">
								所属的应用
							</td>
							<td>
								<asp:DropDownList ID="dropApp" Width="150" runat="server"/>
							</td>
						</tr>
					</table>
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
							<asp:Button runat="server" ID="confirmButton" Text="确定(O)" AccessKey="O" CssClass="formButton"
								OnClick="confirmButton_TextChanged" />
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"
								class="formButton" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<div>
		<iframe style="display: none" name="innerFrame" />
	</div>
	</form>
</body>
</html>
