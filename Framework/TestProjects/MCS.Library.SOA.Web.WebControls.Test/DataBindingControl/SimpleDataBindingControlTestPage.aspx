<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleDataBindingControlTestPage.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.SimpleDataBindingControlTestPage" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SimpleDataBindingControlTestPage</title>
    <script type="text/javascript">
        function checkdata() {
            var reValue = $HBRootNS.DataBindingControl.checkBindingControlDataByGroup(2); //$HBRootNS.DataBindingControl.checkBindingControlData();

            if (!reValue.isValid)	//返回是否通过
                throw reValue.errorMessages;
        }

        function onPostClick() {
            try {
                checkdata();
                //$get("postButton").click();
            }
            catch (e) {
                alert(e);
            }
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <HB:DataBindingControl runat="server" ID="bindingControl" IsValidateOnSubmit="true"
            AutoBinding="true" ValidateUnbindProperties="false" AllowClientCollectData="false"
            AutoValidate="true">
            <ItemBindings>
                <HB:DataBindingItem ControlID="timeInput" DataPropertyName="TimeInput" IsValidateOnBlur="False" ValidationGroup="1"
                    ControlPropertyName="Value" ClientIsHtmlElement="false" ClientPropName="get_value"
                    ClientSetPropName="set_value" />
                <HB:DataBindingItem ControlID="dateInput" DataPropertyName="DateInput" ControlPropertyName="Value"  ValidationGroup="2"
                    AutoFormatOnBlur="true" ClientIsHtmlElement="false" ClientPropName="get_value"
                    ClientSetPropName="set_value" />
                <HB:DataBindingItem ControlID="integerInput" DataPropertyName="IntegerInput" IsValidateOnBlur="true"  ValidationGroup="2"
                    AutoFormatOnBlur="true" />
                <HB:DataBindingItem ControlID="OuUserInputControl1" DataPropertyName="User" ControlPropertyName="SelectedSingleData"
                    ClientPropName="get_selectedOuUserData" ClientSetPropName="set_selectedOuUserData"  ValidationGroup="2"
                    AutoFormatOnBlur="true" ClientIsHtmlElement="false" />
                <HB:DataBindingItem ControlID="nullableFloat" DataPropertyName="NullableFloat" Format="{0:#,##0.00}"  ValidationGroup="2"
                    AutoFormatOnBlur="true" />
                <HB:DataBindingItem ControlID="simpleDataType" DataPropertyName="SimpleDataType"
                    AutoFormatOnBlur="true" />
                <HB:DataBindingItem ControlID="CheckBoxList1" DataPropertyName="SimpleDataType" AutoFormatOnBlur="true"  ValidationGroup="2"/>
                <HB:DataBindingItem ControlID="ddlUsers" DataPropertyName="IntegerInput" ControlPropertyName="SelectedValue"  ValidationGroup="2"
                    FormatDefaultValueToEmpty="true" AutoFormatOnBlur="true" />
            </ItemBindings>
        </HB:DataBindingControl>
    </div>
    <div>
        <div>
            Time Input:</div>
        <div>
            <MCS:DeluxeDateTime ID="timeInput" runat="server" TimeAutoComplete="true" TimeMask="99:99:99"
                TimeAutoCompleteValue="00:00:00"></MCS:DeluxeDateTime>
        </div>
        <div>
            Date Input:</div>
        <div>
            <MCS:DeluxeCalendar ID="dateInput" runat="server">
            </MCS:DeluxeCalendar>
        </div>
        <div>
            <asp:Label runat="server" ID="postedDateTime"></asp:Label>
        </div>
        <div>
            Decimal:</div>
        <div>
            <asp:TextBox runat="server" ID="integerInput"></asp:TextBox>
        </div>
        <div>
            Nullable Float:</div>
        <div>
            <asp:TextBox runat="server" ID="nullableFloat"></asp:TextBox>
        </div>
        <div>
            SimpleDataType :</div>
        <div>
            User:
            <br />
            <HB:OuUserInputControl ID="OuUserInputControl1" runat="server" ReadOnly="false" />
        </div>
    </div>
    <div>
        <HB:HBDropDownList ID="simpleDataType" runat="server">
        </HB:HBDropDownList>
        <br />
        <asp:Label ID="lblsimpleDataType" runat="server" Text=""></asp:Label>
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" OnDataBound="CheckBoxList1_DataBound"
            RepeatDirection="Horizontal">
        </asp:CheckBoxList>
    </div>
    <div>
        <HB:HBDropDownList ID="ddlUsers" runat="server">
            <asp:ListItem Value="1" Text="1"></asp:ListItem>
        </HB:HBDropDownList>
    </div>
    <div>
        <input type="button" value="Post Back" style="width: 120px" onclick="onPostClick();" />
        <asp:Button runat="server" ID="postButton" Text="Post Back Server" Style="display: none"
            OnClick="postButton_Click" />
    </div>
    </form>
</body>
</html>
