<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServicesAddressEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.ServicesAddressEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>服务地址编辑</title>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Cache-Control" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="0">
	<base target="_self" />
	<script type="text/javascript">
	    function checkForm() {
	        if ($get("txtServiceAddress").value.trim() == "") {
	            alert("服务地址不能为空");
	            $get("txtServiceAddress").focus();
	            return false;
	        }

	        if ($get("opKey").value.trim() == "") {
	            alert("请输入服务地址KEY");
	            $get("opKey").focus();
	            return false;
	        }
	        return true;
	    }

	    function onClick() {
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
						<span class="dialogLogo">服务地址编辑</span>
					</div>
				</td>
			</tr>
			<tr>
                <td style="vertical-align: middle">
                    <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
						height: 100%; overflow: auto">
						<table style="height: 100%; width: 100%">
                        <tr>
                            <td class="label" valign="middle">
                                Key:
                            </td>
                            <td valign="middle">
                                <input id="opKey" runat="server" style="width:70%"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                服务地址
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtServiceAddress"  />&nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                发送方式
                            </td>
                            <td>
                                <SOA:HBDropDownList ID="dropSendType" runat="server" />
                                &nbsp;&nbsp;
                            </td>
                        </tr>
						<tr>
                            <td class="label">
                                内容格式
                            </td>
                            <td>
                                <SOA:HBDropDownList ID="ddlContentType" runat="server" />
                                &nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                凭据
                            </td>
                            <td>
                                <SOA:HBDropDownList ID="dropCredential" runat="server"/>
                                &nbsp;&nbsp;
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
                                <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onClick();"
                                    class="formButton" />
                                <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
                                    RelativeControlID="btnOK" PopupCaption="正在保存..." />
                            </td>
                            <td style="text-align: center;">
                                <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();return false;"
                                    class="formButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
		</table>
	</div>
    <input id="hidResultData" type="hidden" runat="server"/>
    <input type="hidden" id="hiddenKey" runat="server"/>
	</form>
</body>
</html>
