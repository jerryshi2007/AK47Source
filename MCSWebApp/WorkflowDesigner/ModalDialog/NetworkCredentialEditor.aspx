<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NetworkCredentialEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.NetworkCredentialEditor" %>
    
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>网络凭据</title>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Cache-Control" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="0">
	<base target="_self" />
	<script type="text/javascript">
		var _credential = null;
		var _allKeys = null;
		var _op = "";

		function onDocumentInit() {
			var arg = window.dialogArguments;

			if (arg) {
				var data = Sys.Serialization.JavaScriptSerializer.deserialize(arg.data);

				_credential = data.credential;
				_allKeys = data.allKeys;
				_op = data.op;

				if (_op == "insert") {
				    $get("txtKey").disabled = true;
				}
			}
		}

		function checkData(cred) {
			checkStringEmpty(cred.Key, "Key不能为空");
			checkStringEmpty(cred.LogOnName, "登录名不能为空");

			if (_op == "insert") {
				if (Array.contains(_allKeys, cred.key))
					throw Error.create("Key值" + cred.key + "已存在");
			}
		}

		function checkStringEmpty(value, message) {
			if (value == null || value == "")
				throw Error.create(message);
		}

		function collectData() {
		    if ($get("txtPassword").value != $get("txtConfirmedPassword").value)
				throw Error.create("密码和密码确认的值不一致");

			_credential = {
			    Key: $get("txtKey").value,
			    LogOnName: $get("txtLogonName").value,
			    Password: $get("txtPassword").value
			};

			checkData(_credential);
		}

		function checkForm() {
		    if ($get("txtKey").value == "") {
		        alert("Key不能为空");
		        return false;
		    }
		    if ($get("txtLogonName").value == "") {
		        alert("登录名不能为空");
		        return false;
		    }
		    if (($get("txtPassword").value != $get("txtConfirmedPassword").value) || $get("txtPassword").value=="") {
		        alert("密码和密码确认的值不一致");
		        return false;
            }
		    return true;
        }

        function onConfirmButtonClick() {
            if (checkForm()) {
                document.getElementById("btnConfirm").click();
                window.returnValue = true;
                top.close();
            }
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:ScriptManager runat="server" ID="scriptManager">
	</asp:ScriptManager>
	<div>
		<table style="height: 100%; width: 100%">
			<tr>
				<td class="gridHead">
					<div class="dialogTitle">
						<span class="dialogLogo">网络凭据</span>
					</div>
				</td>
			</tr>
			<tr>
				<td style="vertical-align: middle">
					<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
						height: 100%; overflow: auto">
						<table style="height: 100%; width: 100%">
							<tr>
								<td class="label">
									凭据Key
								</td>
								<td>
									<input type="text" id="txtKey" style="text-transform: uppercase" maxlength="32" runat="server"/>
								</td>
							</tr>
							<tr>
								<td class="label">
									登录名
								</td>
								<td>
									<input type="text" id="txtLogonName" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="label">
									密码
								</td>
								<td>
									<input type="password" id="txtPassword" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="label">
									密码确认
								</td>
								<td>
									<input type="password" id="txtConfirmedPassword"  runat="server"/>
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
								<input type="button" class="formButton" onclick="onConfirmButtonClick();" value="确定(O)" id="btnOK"
									accesskey="O" />
                                    <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
                                    RelativeControlID="btnOK" PopupCaption="正在保存..." />
							</td>
							<td style="text-align: center;">
								<input type="button" class="formButton" onclick="top.close();return false;" value="取消(C)" id="btnCancel"
									accesskey="C" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
