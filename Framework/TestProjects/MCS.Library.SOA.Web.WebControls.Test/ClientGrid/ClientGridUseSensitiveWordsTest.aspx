<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGridUseSensitiveWordsTest.aspx.cs" ValidateRequest="true"  Inherits="MCS.Library.SOA.Web.WebControls.Test.ClientGridUseSensitiveWordsTest" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls" 
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ClientGridUseSensitiveWordsTest</title>
    <script type="text/javascript">
        function onckeck(sender, e) {

        }

        function test() {
            alert($find("gridTest").get_dataSource()[0].CheckNo);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnPostBack" runat="server" Text="PostBack" 
            onclick="btnPostBack_Click" />
        <table id="table1" runat="server" style="width: 100%;">
            <tr>
                <td>
                  <SOA:ClientGrid runat="server" ID="gridTest" ShowEditBar="true" AllowPaging="false"
            AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true" OnSelectCheckboxClick="onckeck">
            <Columns>
                 <SOA:ClientGridColumn DataField="CheckNo" SelectColumn="true" ShowSelectAll="true"
                HeaderStyle="{width:'30px',textAlign:'center',fontWeight:'bold'}" ItemStyle="{width:'30px',textAlign:'center'}">
            </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="Input" HeaderText="输入测试"
                    HeaderStyle="{Width: '400px'}" ItemStyle="{textAlign:'center',Width: '400px'}"
                    EditorStyle="{width:'400px',textAlign:'center'}">
                    <EditTemplate EditMode="TextBox"/>
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
                </td>
             
            </tr>
        </table>
         <input type="button" onclick="test();"/>
    </div>
    </form>
</body>
</html>
