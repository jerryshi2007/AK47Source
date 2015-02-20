<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyFormTestForClientGridEditor.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PropertyFormTestForClientGridEditor" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="display:none">
        <SOA:HBDropDownList ID="ddlForGrid" runat="server">
            <Items>
                <asp:ListItem Value="1">1</asp:ListItem>
                <asp:ListItem Value="2">2</asp:ListItem>
                <asp:ListItem Value="3">3</asp:ListItem>
            </Items>
        </SOA:HBDropDownList>
       <%-- <SOA:MaterialControl ID="MaterialControl1" runat="server" RootPathName="GenericProcess" MaterialUseMode="UploadFile" TemplateUrl="~/MaterialControl/Templates/Test.xlsx"  Caption="编辑"/>
         <SOA:ClientGrid runat="server" ID="gridTest" ShowEditBar="true" AllowPaging="false"
            ReadOnly="False" AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true">
            <Columns>
                <SOA:ClientGridColumn DataField="Materials1" HeaderText="附件1" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate EditMode="Material" 
                    TemplateControlSettings="{rootPathName:'GenericProcess',materialUseMode:'DraftAndUpload',templateUrl:'~/MaterialControl/Templates/Test.xlsx',allowEdit:'true',allowEditContent:'true'}" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="Materials2" HeaderText="附件2" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate EditMode="Material" TemplateControlClientID="MaterialControl1" />

                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>--%>
    </div>
      <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%"/>
          <asp:Button ID="btnPostBack" runat="server" Text="PostBack" 
              onclick="btnPostBack_Click" />
    </div>
    
    </form>
</body>
</html>
