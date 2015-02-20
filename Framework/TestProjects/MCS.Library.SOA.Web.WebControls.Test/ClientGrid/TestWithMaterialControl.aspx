<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWithMaterialControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.TestWithMaterialControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ClientGrid TestWithMaterialControl</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 400px">
        <asp:Button ID="btnPostBack" runat="server" Text="PostBack" OnClick="btnPostBack_Click" />
        <asp:Button ID="btnSetReadOnly" runat="server" Text="SetReadOnly" OnClick="btnSetReadOnly_Click" />

        <SOA:ClientGrid runat="server" ID="gridTest" ShowEditBar="true" AllowPaging="false"
            ReadOnly="False" AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true">
            <Columns>
                <SOA:ClientGridColumn DataField="Materials1" HeaderText="附件1" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate EditMode="Material"  TemplateControlID="MaterialControl1"
                    TemplateControlSettings="{rootPathName:'GenericProcess',materialUseMode:'DraftAndUpload',templateUrl:'~/MaterialControl/Templates/Test.xlsx',allowEdit:'true',allowEditContent:'true'}" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="Materials2" HeaderText="附件2" HeaderStyle="{Width: '400px'}"
                    ItemStyle="{textAlign:'center',Width: '400px'}" EditorStyle="{textAlign:'left'}">
                    <EditTemplate EditMode="Material" TemplateControlSettings="{rootPathName:'GenericProcess',materialUseMode:'UploadFile',templateUrl:'~/MaterialControl/Templates/Test.xlsx',allowEdit:'true',allowEditContent:'true'}" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>

        <div style="display: none">
            <SOA:MaterialControl ID="MaterialControl1" runat="server" RootPathName="GenericProcess" MaterialUseMode="UploadFile" TemplateUrl="~/MaterialControl/Templates/Test.xlsx"  Caption="编辑"/>
        </div>
       </div>
    </form>
</body>
</html>
