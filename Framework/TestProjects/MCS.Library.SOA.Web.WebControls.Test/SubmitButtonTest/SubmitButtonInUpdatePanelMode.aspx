<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonInUpdatePanelMode.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.SubmitButton.SubmitButtonInUpdatePanelMode" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试在UpdatePanel模式下的SubmitButton</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<h1>
			测试UpdatePanel</h1>
	</div>
	<div>
		<input type="text" id="serverResult" />
	</div>
	<div>
		<asp:UpdatePanel runat="server" ID="panel">
			<ContentTemplate>
				<soa:SubmitButton runat="server" Text="提交..." PopupCaption="正在提交..." 
					ID="submitBtn" onclick="submitBtn_Click" />
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>
	</form>
</body>
</html>
