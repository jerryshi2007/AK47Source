<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dropdownListBindingTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.dropdownListBindingTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Dropdown list binding test</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:DataBindingControl runat="server" ID="bindingControl" AllowClientCollectData="true">
			<ItemBindings>
				<SOA:DataBindingItem ControlID="candidates" DataPropertyName="Users" ControlPropertyName="DataSource" />
				<SOA:DataBindingItem ControlID="candidates" DataPropertyName="CandidateID" ControlPropertyName="SelectedValue" />
			</ItemBindings>
		</SOA:DataBindingControl>
	</div>
	<div>
		Please select candidates
	</div>
	<div>
		<SOA:HBDropDownList runat="server" ID="candidates" DataValueField="ID" DataTextField="Name">
		</SOA:HBDropDownList>
	</div>
	<div>
		<asp:Button runat="server" ID="postBackBtn" Text="Post Back" />
	</div>
	<div>
		<asp:Literal runat="server" ID="selectedCadidateID" />
	</div>
	</form>
</body>
</html>
