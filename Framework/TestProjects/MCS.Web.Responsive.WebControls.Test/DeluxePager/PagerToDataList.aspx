<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PagerToDataList.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DeluxePager.PagerToDataList" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Pager For DataList</title>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <div class="dataTables_wrapper" role="grid">
            <div class="row">
                <res:DeluxePager ID="pager" runat="server" PagerSettings-Mode="Numeric" DataBoundControlID="dataList" CssClass=""
                    PageSize="10">
                </res:DeluxePager>
            </div>
        </div>
        <asp:DataList ID="dataList" runat="server" CellPadding="2" ForeColor="#333333" Width="100%"
            DataSourceID="sqlDataSource">
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <SelectedItemStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SeparatorTemplate>
                <hr />
            </SeparatorTemplate>
            <ItemTemplate>
                <table style="width: 100%">
                    <tr>
                        <td style="text-align: right">
                            用户:
                        </td>
                        <td>
                            <asp:TextBox ID="CREATE_USER" runat="server" Text='<%# Bind("CREATE_USER") %>'>
                            </asp:TextBox>
                        </td>
                        <td style="text-align: right">
                            PRIORITY:
                        </td>
                        <td>
                            <asp:TextBox ID="PRIORITY" runat="server" Text='<%# Bind("PRIORITY") %>'>
                            </asp:TextBox>
                        </td>
                        <td style="text-align: right">
                            产品名称:
                        </td>
                        <td>
                            <asp:TextBox ID="CUSTOMER_NAME" runat="server" Text='<%# Bind("CUSTOMER_NAME") %>'>
                            </asp:TextBox>
                        </td>
                        <td style="text-align: right">
                            时间:
                        </td>
                        <td>
                            <asp:TextBox ID="CREATE_TIME" runat="server" Text='<%# Bind("CREATE_TIME") %>'>
                            </asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
            <AlternatingItemStyle BackColor="White" />
            <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        </asp:DataList>
    </div>
    <asp:SqlDataSource ID="sqlDataSource" runat="server" OnSelected="SqlDataSource1_Selected"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT TOP (@PageSize) ORDER_ID, SORT_ID, CUSTOMER_NAME, (CASE PRIORITY WHEN 0 THEN 'Normal' WHEN 1 THEN 'High' WHEN - 1 THEN 'Low' END) AS PRIORITY, CREATE_USER, CREATE_TIME, UPDATE_TAG FROM ORDERS WHERE (ORDER_ID NOT IN (SELECT TOP (@PageIndex) ORDER_ID FROM ORDERS AS ORDERS_1 ORDER BY ORDER_ID DESC)) ORDER BY ORDER_ID DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="pager" Name="PageSize" DefaultValue="10" PropertyName="PageSize" />
            <asp:ControlParameter ControlID="pager" Name="PageIndex" DefaultValue="0" PropertyName="PageIndex" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
