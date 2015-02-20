<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreRowAddEventTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.PreRowAddEventTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function OnPreRowAdd(grid, e) {
            //alert("开始您的操作");
            //如弹出等相关方式您都可以在这做，修改你要修改的指就可以了
            e.rowData["PaymentItem"] = "垂虹园二期项目";
            e.rowData["RMB"] = 1929;
            e.rowData["INT"] = 9876;
            e.rowData["Currency"] = "11.20";
        }    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="column visible test" Width="100%"
            ShowEditBar="true" OnPreRowAdd="OnPreRowAdd">
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
    </form>
</body>
</html>
