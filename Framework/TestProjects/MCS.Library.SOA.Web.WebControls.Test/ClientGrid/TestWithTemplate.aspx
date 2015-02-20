<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWithTemplate.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.HBGrid.TestWithTemplate" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function onCellCreatedEditor(grid, e) {
            if (e.column.dataField === "HasCashPoolChoice") {
                var inputs = e.editor._editorElement.getElementsByTagName("input");
                var labels = e.editor._editorElement.getElementsByTagName("label");
                for (var i = 0; i < inputs.length; i++) {
                    inputs[i].id = "id" + i + e.rowData.rowIndex;
                    labels[i].attributes["for"].value = "id" + i + e.rowData.rowIndex;
                    inputs[i].name = "name" + e.rowData.rowIndex;
                }
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOA:ClientGrid runat="server" ID="gridWorkPlan" ShowEditBar="true" AllowPaging="false" ReadOnly="True"
            AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true" OnCellCreatedEditor="onCellCreatedEditor">
            <Columns>
                <SOA:ClientGridColumn DataField="HasCashPoolChoice" HeaderText="是º?否¤?纳¨¦入¨?现?金e池?"
                    HeaderStyle="{Width: '100px'}" ItemStyle="{textAlign:'center',Width: '100px'}"
                    EditorStyle="{width:'100px',textAlign:'center'}">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="CashPoolChoice" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
        <SOA:HBRadioButtonList runat="server" ID="CashPoolChoice" RepeatDirection="Horizontal"
            KeepControlWhenReadOnly="True">
            <asp:ListItem runat="server" Text="是º?" Value="true">
            </asp:ListItem>
            <asp:ListItem runat="server" Text="否¤?" Value="false">
            </asp:ListItem>
        </SOA:HBRadioButtonList>
    </div>
    </form>
</body>
</html>
