<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitButtonWithProgress.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.SubmitButton.SubmitButtonWithProgress" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>向Frame提交，并且存在过程变化</title>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<div>
		<asp:ScriptManager ID="ScriptManager1" runat="server">
		</asp:ScriptManager>
	</div>
	<div>
		<HB:SubmitButton runat="server" ID="submitWithProcess" Text="带进度的提交" PopupCaption="正在提交..."
			OnClick="submitWithProcess_Click" ProgressMode="BySteps" />
	</div>
	<div>
		<iframe id="innerFrame" name="innerFrame"></iframe>
	</div>
	</form>
</body>
</html>
