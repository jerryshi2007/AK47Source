<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWithOuUserInputEditor.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.TestWithOuUserInputEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    
    <div style="width: 400px">
        <asp:Button ID="btnPostBack" runat="server" Text="PostBack" OnClick="btnPostBack_Click" />
        <SOA:ClientGrid runat="server" ID="gridTest" ShowEditBar="true" AllowPaging="false" ReadOnly="False"
            AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true">
            <Columns>
                <SOA:ClientGridColumn DataField="User" HeaderText="User" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate ControlClientPropName="get_selectedSingleData" ControlClientSetPropName="set_selectedSingleData" 
                    EditMode="OuUserInput" TemplateControlSettings="{ multiSelect:'false',selectMask:'User',showCheckButton:'false',showSelector:'true',enableUserPresence:'true'}"/>
                </SOA:ClientGridColumn>
          
                <SOA:ClientGridColumn DataField="Organizations" HeaderText="Organizations" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate EditMode="OuUserInput" TemplateControlID="OuUserInputControl1" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
        <div style="display:none">
            <SOA:OuUserInputControl ID="OuUserInputControl1" runat="server" MultiSelect="True" 
                SelectMask="Organization" ShowCheckButton="False" />
        </div>
    </div>
    <div id="aa" runat="server" > </div>
    
    </form>
    
</body>
</html>
