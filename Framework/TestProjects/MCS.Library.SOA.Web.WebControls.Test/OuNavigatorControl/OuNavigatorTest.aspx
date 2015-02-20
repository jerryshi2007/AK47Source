<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OuNavigatorTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OuNavigatorControl.OuNavigatorTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Ou Navigator Control Test</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:OuNavigator runat="server" ID="navigator" LinkTemplate="OguTarget.aspx" Target="_blank"/>
	</div>
	</form>
</body>
</html>
