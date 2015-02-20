<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="columnVisibleTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.columnVisibleTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function rebind() {
            var grid = $find("clientGrid1");
            var columns = grid.get_columns();
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].dataField == "RMB" || columns[i].dataField == "Totle") {
                    columns[i].visible = false;
                }
            }
            var ds = grid.get_dataSource();
            Array.remove(ds, grid.get_selectedData()[3]);
            grid.rebind();
        }

        function getSelectedData() {
            var grid = $find("clientGrid1");
            var l = grid.get_selectedData().length;
            alert(l);
        }

        //在客户端绑定数据渲染前，可以通过这个事件来处理 打开注释~
        function OnBeforeDataBind(grid, e) {
            //var columns = grid.get_columns();  //var columns = e.columns;
            //for (var i = 0; i < columns.length; i++) {
            //    if (columns[i].dataField == "RMB" || columns[i].dataField == "Totle") {
            //        columns[i].visible = false;
            //    }
            //}
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="column visible test" Width="100%" ShowEditBar ="true"
            OnBeforeDataBind="OnBeforeDataBind" ><%--ReadOnly="true"--%>
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',height:'30'}" />
                <HB:ClientGridColumn DataField="index" HeaderText="行" SortExpression="index" DataType="Integer"
                    EditorStyle="{border: '1px solid #ccc',textAlign:'left',width:'20px'}" ItemStyle="{width:'100px',height:'30px'}"
                    Visible="false" />
                <HB:ClientGridColumn DataField="PaymentItem" HeaderText="费用项目" EditorTooltips="费用项目费用项目"
                    ItemStyle="{width:'200px',height:'30px'}">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="DropDownList1" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn MaxLength="5" DataField="RMB" HeaderText="金额" DataType="Decimal"
                    EditorStyle="{width:'100px',border: '5px solid #ccc',textAlign:'right'}" FormatString="{0:N2}"
                    HeaderStyle="{width:'200px'}" ItemStyle="{width:'200px',height:'30px'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="INT" HeaderText="INT" DataType="Integer" HeaderStyle="{width:'250px'}"
                    ItemStyle="{width:'250px'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Currency" HeaderText="币种" FormatString="{0:N2}" HeaderStyle="{width:'250px'}"
                    ItemStyle="{width:'250px'}">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="ddlCurrencyList" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="ExchangeRate" HeaderText="汇率" DataType="Decimal"
                    SortExpression="ExchangeRate" FormatString="{0:N2}" EditorStyle="{textAlign:'left'}"
                    EditorTooltips="这是汇率" HeaderStyle="{width:'100px'}" ItemStyle="{width:'100px',height:'30px'}">
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Totle" HeaderText="金额（￥）" DataType="Decimal" FormatString="{0:N2}"
                    HeaderStyle="{width:'300px'}" ItemStyle="{width:'300px',height:'30px'}">
                </HB:ClientGridColumn>
                <HB:ClientGridColumn HeaderText="href" HeaderStyle="{width:'300px'}" ItemStyle="{width:'300px',height:'30px'}"
                    DataField="a">
                    <EditTemplate EditMode="A" DefaultTextOfA="fefefefefe" DefaultHrefOfA="http://www.baidu.com"
                        TargetOfA="_self" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <div style="display: none">
        <asp:DropDownList ID="ddlCurrencyList" runat="server">
            <asp:ListItem Text="" Value=""></asp:ListItem>
            <asp:ListItem Text="美元" Value="6.50"></asp:ListItem>
            <asp:ListItem Text="日元" Value="0.17"></asp:ListItem>
            <asp:ListItem Text="韩元" Value="0.002"></asp:ListItem>
            <asp:ListItem Text="港币" Value="0.91"></asp:ListItem>
            <asp:ListItem Text="人民币" Value="1"></asp:ListItem>
            <asp:ListItem Text="英镑" Value="11.20"></asp:ListItem>
            <asp:ListItem Text="欧元" Value="8.20"></asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="DropDownList1" runat="server">
            <asp:ListItem Text="华盛家园一期工程项目" Value="华盛家园一期工程项目"></asp:ListItem>
            <asp:ListItem Text="垂虹园二期项目" Value="垂虹园二期项目"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <asp:Button ID="Button1" runat="server" Text="PostBack" OnClick="Button1_Click" />
    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    <br />
    <br />
    <input type="button" onclick="rebind();" value="重绑[去除“金额”和“金额（￥）列”]" />
    <input type="button" onclick="getSelectedData();" value="获取绑定的值" />
    </form>
</body>
</html>
