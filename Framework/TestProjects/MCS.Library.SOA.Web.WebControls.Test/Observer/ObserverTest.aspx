<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ObserverTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Observer.ObserverTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Observer Test</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server">
			<Scripts>
				<asp:ScriptReference Path="~/Observer/observer.js" />
			</Scripts>
			<CompositeScript>
				<Scripts>
					<asp:ScriptReference Name="MicrosoftAjax.js" />
					<asp:ScriptReference Path="~/Observer/observer.js" />
				</Scripts>
			</CompositeScript>
		</asp:ScriptManager>
	</div>
	</form>
</body>
</html>
