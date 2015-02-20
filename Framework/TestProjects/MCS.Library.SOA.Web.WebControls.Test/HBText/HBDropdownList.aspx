<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBDropdownList.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.HBText.HBDropdownList" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>HBDropdownList Test Page</title>
	<script type="text/javascript">
		function onChangeDropdownList() {
			var dl = document.getElementById("dropdownList");

			while(dl.options.length > 0)
				dl.options.remove(0);

			var opt1 = document.createElement("OPTION");
			opt1.value = "3";
			opt1.text = "Three";
			dl.options.add(opt1);

			var opt2 = document.createElement("OPTION");
			opt2.value = "4";
			opt2.text = "Four";
			dl.options.add(opt2);
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<HB:HBDropDownList ID="dropdownList" runat="server" ReadOnly="false">
			<%--<asp:ListItem Text="One" Value="1"></asp:ListItem>
			<asp:ListItem Text="Two" Value="2"></asp:ListItem>--%>
		</HB:HBDropDownList>
	</div>
	<div>
		Selected Text:
		<asp:Label runat="server" ID="selectedText" />
	</div>
	<div>
		<asp:Button runat="server" ID="postbackBtn" Text="Post Back" />
	</div>
	<div>
		<input type="button" value="Change dropdown list" onclick="onChangeDropdownList();" />
	</div>
	</form>
</body>
</html>
