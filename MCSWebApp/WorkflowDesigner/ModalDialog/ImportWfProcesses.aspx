<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportWfProcesses.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.ImportWfProcesses" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>导入流程模板</title>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请输入流程模板的XML</span>
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
							<td>
								<textarea id="ttaWfProcess" cols="20" runat="server" rows="1" style="width: 100%;
									height: 50%"></textarea>
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
								OnClick="confirmButton_Click" />
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
