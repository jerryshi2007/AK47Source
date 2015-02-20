<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pageChangeTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.pageChangeTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //获取选中项数据
        function showselectedData() {
            var grid = $find("clientGrid1");

            var selecteddata = grid.get_selectedData();
            var dataSource = grid.get_dataSource();

            alert("selecteddata" + $Serializer.serialize(selecteddata) + "\n\ndatasource:" + $Serializer.serialize(dataSource));

            //debugger;
        }
    </script>
    <script type="text/javascript">
        function getDataSource(pageIndex, pageSize) {
            //var grid = $find("clientGrid1");
            //var dataSource_root = grid.get_dataSource();
            //var dataSource = [];
            //for (var i = 0; i < pageSize; i++) {
            //    var num = pageIndex * pageSize + i;
            //    var data = dataSource_root[num];
            //    Array.add(dataSource, data);
            //}
            //return dataSource;
        }

        function OnClientPageIndexChanged(grid, e) {
            var pageIndex = grid.get_pageIndex();
            var pageSize = grid.get_pageSize();

            var grid = $find("clientGrid1");
            var dataSource = grid.get_dataSource();

            var dataSource2 = [];
            for (var i = 0; i < pageSize; i++) {
                var num = pageIndex * pageSize + i;
                var data = dataSource[num];

                data.name = "newName====";

                Array.add(dataSource2, data);
            }

            grid.set_currentPageDataSource(dataSource2);
        }
    </script>
    <script type="text/javascript">
        function OnCellCreatingEditor(grid, e) {
            if (e.column.dataField == "age") {
                if (e.rowData["age"] == 0) {
                    e.autoFormat = false;
                    e.showValueTobeChange = "";
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="人员列表-分页测试" Width="100%" ShowEditBar="true"
            AllowPaging="true" PageSize="5" AutoPaging="true" OnClientPageIndexChanged="OnClientPageIndexChanged"
            OnCellCreatingEditor="OnCellCreatingEditor">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" />
                <HB:ClientGridColumn DataField="index" HeaderText="行" SortExpression="index" DataType="Integer"
                    ItemStyle="{textAlign: 'center'}" />
                <HB:ClientGridColumn DataField="name" HeaderText="姓名" DataType="String" ItemStyle="{textAlign: 'center'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="sex" HeaderText="性别" DataType="String" ItemStyle="{textAlign: 'center'}">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="ddlSex" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="age" HeaderText="年龄" DataType="Integer" ItemStyle="{textAlign: 'center'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
        <br />
        <div style="display: none">
            <asp:DropDownList ID="ddlSex" runat="server">
                <asp:ListItem Text="" Value=""></asp:ListItem>
                <asp:ListItem Text="男" Value="M"></asp:ListItem>
                <asp:ListItem Text="女" Value="W"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <br />
        <input type="button" value="getvalue" onclick="showselectedData();" style="text-align: center" />
    </div>
    </form>
</body>
</html>
