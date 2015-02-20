<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="withDataTimeClientGrid.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.withDataTimeClientGrid" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //获取选中项数据
        function showSelectedData() {
            var grid = $find("clientGrid1");

            var selecteddata = grid.get_selectedData();
            var dataSource = grid.get_dataSource();

            alert("selecteddata" + $Serializer.serialize(selecteddata) + "\n\ndatasource:" + $Serializer.serialize(dataSource));
        }
    </script>
    <script type="text/javascript">
        function OnDataChanged(grid, e) {
            if (e.column.dataField == "Date") {

                //e.rowData["Date"] = Date.minDate;

                //alert(e.editor.get_gridCell().get_displayData());

                e.gridRow.rowDataBind(e.rowData);
            }
            if (e.column.dataField == "Money") {
                //e.rowData["Money"] = 0;
                //e.editor.get_gridRow().rowDataBind(e.rowData);
            }
        }   
    </script>
    <script type="text/javascript">
        function rebind() {
            $find("clientGrid1").set_readOnly(true);
            $find("clientGrid1").dataBind();
        }

        function OnAfterDataRowCreate(grid, e) {
            if (e.rowData["rowIndex"] == 1) {
                e.htmlRow.htmlRowLeft.style["backgroundColor"] = "red";
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div_test">
    </div>
    <div id="div_test1" >
    </div>
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="带日期、时间控件的clientGrid" Width="570px"
            ShowEditBar="true" OnDataChanged="OnDataChanged" OnAfterDataRowCreate="OnAfterDataRowCreate">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" HeaderStyle="{width:'20px'}"
                    ItemStyle="{width:'20px'}" />
                <HB:ClientGridColumn DataField="Index" HeaderText="行" SortExpression="Index" DataType="Integer"
                    HeaderStyle="{width:'50px'}" ItemStyle="{width:'50px',textAlign:'center'}" />
                <HB:ClientGridColumn DataField="Date" HeaderText="Date" SortExpression="Date" DataType="DateTime"
                    EditorTooltips="我是日期控件" HeaderStyle="{width:'140px'}" ItemStyle="{width:'140px',textAlign:'center'}"
                    EditorStyle="{width:'70px'}" FormatString="{0:yyyy-MM-dd}">
                    <EditTemplate EditMode="DateInput" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="DateTime" HeaderText="DateTime" SortExpression="DateTime"
                    DataType="DateTime" EditorTooltips="我是日期时间控件" HeaderStyle="{width:'180px'}" ItemStyle="{width:'180px',textAlign:'center'}"
                    EditorStyle="{width:'70px',height:'16px'}" FormatString="{0:yyyy-MM-dd HH:mm:ss}"
                    EditorReadOnly="true">
                    <EditTemplate EditMode="DateTimeInput" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Money" HeaderText="Money" SortExpression="Money"
                    DataType="Decimal" FormatString="{0:N2}" HeaderStyle="{width:'180px'}" ItemStyle="{width:'180px',background-color:'#e6e6e6',textAlign:'right'}"
                    EditorStyle="{width:'80px',textAlign:'right',paddingRight:'10px'}" MaxLength="12"
                    EditorReadOnly="true">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <div>
        <asp:Button ID="Button1" runat="server" Text="postback" OnClick="Button1_Click" />
    </div>
    <div>
        <input type="button" value="getvalue" onclick="showSelectedData();" />
        <input type="button" value="rebind" onclick="rebind();" />
    </div>
    <br />
    <div>
        <CCIC:DeluxeCalendar runat="server" ID="DeluxeCalendar1" ReadOnly="true">
        </CCIC:DeluxeCalendar>
    </div>
    </form>
</body>
</html>
