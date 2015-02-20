<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WithDeluxeCalendar.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.WithDeluxeCalendar" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<%--<HB:DataBindingControl runat="server" ID="bindingControl" IsValidateOnSubmit="true"
            AutoBinding="true" ValidateUnbindProperties="false" AllowClientCollectData="true"
            AutoValidate="true">--%>
		<HB:DataBindingControl runat="server" ID="bindingControl" AutoBinding="true" AutoValidate="false">
			<ItemBindings>
				<HB:DataBindingItem ControlID="DeluxeCalendar1" DataPropertyName="DateInput" ControlPropertyName="Value"  IsValidate="false"
                 Direction="Both" 
                />
			</ItemBindings>
		</HB:DataBindingControl>
	</div>
	<%--ClientPropName="get_value" ClientSetPropName="set_value"--%>
	<div>
		<MCS:DeluxeCalendar runat="server" Enabled="false" ReadOnly="true" ID="DeluxeCalendar1">
		</MCS:DeluxeCalendar>
	</div>
	<br />
	<div>
		Calendar Value in Server:
		<asp:Label runat="server" ID="calendarValueInServer"></asp:Label>
	</div>
	<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
	</form>
</body>
</html>
