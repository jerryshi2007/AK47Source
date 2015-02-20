<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditorPassword.aspx.cs"
	Inherits="PermissionCenter.Dialogs.EditorPassword" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>修改口令</title>
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
</head>
<body style="min-height: 0; min-width: 0" class="pcdlg">
	<form id="form1" runat="server" target="innerFrame">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			修改口令
		</h1>
	</div>
	<div class="pcdlg-content">
		<asp:ScriptManager runat="server" ID="ScriptManager" EnableScriptGlobalization="true">
		</asp:ScriptManager>
		<asp:UpdatePanel runat="server" ID="passwordUpdatePanel">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
			</Triggers>
			<ContentTemplate>
				<div class="pc-container5">
					<div class="pc-vf-list">
						<div class="pc-vf-item" style="white-space: normal">
							<div>
								<span runat="server" id="passwordresult" style="color: Red; white-space: normal;" />
							</div>
						</div>
						<div class="pc-vf-item">
							<span class="pc-vf-label">账户名</span>
							<asp:TextBox runat="server" ID="txtLogOnName" CssClass="pc-vf-input"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="RequiredFieldValidator3" ErrorMessage="用户名不能为空" Display="Dynamic"
								ControlToValidate="txtLogOnName">*</asp:RequiredFieldValidator>
						</div>
						<div class="pc-vf-item" runat="server">
							<span class="pc-vf-label" id="lblCurrentPass" runat="server">当前口令</span><input type="password"
								size="20" id="tb_OldPassword" class="pc-vf-input" runat="server" />
							<asp:RequiredFieldValidator runat="server" ID="va_password" ErrorMessage="密码不能为空"
								ControlToValidate="tb_OldPassword" Enabled="false" Display="Dynamic">*</asp:RequiredFieldValidator>
							<div style="display: none">
								<input type="hidden" runat="server" id="isSuperVisior" />
								<input type="hidden" runat="server" id="currentUserName" />
							</div>
						</div>
						<div class="pc-vf-item">
							<span class="pc-vf-label">新口令</span><input type="password" size="20" id="tb_NewPassword"
								runat="server" class="pc-vf-input" />
							<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ErrorMessage="新码不能为空"
								ControlToValidate="tb_NewPassword" Enabled="false" Display="Dynamic">*</asp:RequiredFieldValidator>
						</div>
						<div class="pc-vf-item">
							<span class="pc-vf-label">确认新口令</span><input type="password" size="20" id="tb_CommitPassword"
								runat="server" class="pc-vf-input" /><asp:CompareValidator ID="CompareValidator1"
									runat="server" ControlToCompare="tb_NewPassword" ControlToValidate="tb_CommitPassword"
									Type="String" ErrorMessage="两次输入密码不一致" Operator="Equal" Display="Dynamic">*</asp:CompareValidator>
							<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ErrorMessage="确认密码不能为空"
								ControlToValidate="tb_CommitPassword" Enabled="false" Display="Dynamic">*</asp:RequiredFieldValidator>
						</div>
					</div>
				</div>
				</span>
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<asp:Button CssClass="pcdlg-button" runat="server" Text="确定(O)" ID="Button1" OnClick="OK_Click"
				AccessKey="O" />
			<input type="button" class="pcdlg-button" onclick="top.close();" value="取消(C)" id="Button2"
				accesskey="C" />
		</div>
	</div>
	<asp:UpdateProgress runat="server" ID="upProgress">
		<ProgressTemplate>
			正在处理……
		</ProgressTemplate>
	</asp:UpdateProgress>
	<div style="display: none">
		<iframe name="innerFrame"></iframe>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.traceWindowWidth();


		function fun() {
			$pc.setText("lblCurrentPass", document.getElementById("currentUserName").value === document.getElementById("txtLogOnName").value ? "当前口令" : "管理员口令");
		}

		if (document.getElementById("isSuperVisior").value == "1") {
			if (navigator.userAgent.indexOf("MSIE") > 0) {
				document.getElementById('txtLogOnName').attachEvent("onpropertychange", fun);
			} else if (navigator.userAgent.indexOf("Firefox") > 0) {
				document.getElementById('txtLogOnName').addEventListener("input", fun, false);
			}
		}
	</script>
</body>
</html>
