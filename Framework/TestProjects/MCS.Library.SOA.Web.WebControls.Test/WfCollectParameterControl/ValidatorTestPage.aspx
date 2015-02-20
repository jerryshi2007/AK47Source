<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidatorTestPage.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.WfCollectParameterControl.ValidatorTestPage" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls.Test" Namespace="MCS.Library.SOA.Web.WebControls.Test.WfCollectParameterControl"
	TagPrefix="Test" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:Button runat="server" ID="postButton" Text="Post Back" />
		<Test:ValidatorWrapperControl runat="server" ID="validatorWrapper" />
	</div>
	<div>
		<MCS:DeluxeCalendar runat="server" ID="DeluxeCalendar1">
		</MCS:DeluxeCalendar>
	</div>
	</form>
</body>
</html>
