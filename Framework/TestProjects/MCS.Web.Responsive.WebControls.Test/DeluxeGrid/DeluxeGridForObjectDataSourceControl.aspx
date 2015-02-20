<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeluxeGridForObjectDataSourceControl.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DeluxeGrid.DeluxeGridForObjectDataSourceControl" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>DeluxeGridForObjectDataSourceControl数据源类型展示</title>
</head>
<body>
    <form id="serverForm" runat="server">
    <div class="container">
        <div>
            <asp:DropDownList ID="prioritySelector" runat="server" AutoPostBack="true">
                <asp:ListItem Value="" Text="-" Selected="True" />
                <asp:ListItem Value="-1" Text="低" />
                <asp:ListItem Value="0" Text="中" />
                <asp:ListItem Value="1" Text="高" />
            </asp:DropDownList>
        </div>
        <div>
            <res:DeluxeGrid ID="DeluxeGrid1" runat="server" GridLines="None" AllowPaging="True"
                GridTitle="订单列表" EnableViewState="false" AutoGenerateColumns="False" UseAccessibleHeader="False"
                DataSourceID="objectDataSource" DataSourceMaxRow="0" ShowExportControl="True"
                DataKeyNames="Sort_ID" AllowSorting="True" ShowCheckBoxes="true" CheckBoxPosition="Left">
                <FooterStyle />
                <RowStyle />
                <EditRowStyle />
                <SelectedRowStyle />
                <PagerStyle />
                <HeaderStyle />
                <AlternatingRowStyle />
                <Columns>
                    <asp:BoundField DataField="ORDER_ID" Visible="False">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:HyperLinkField DataNavigateUrlFields="ORDER_ID" DataNavigateUrlFormatString="CallServiceOrderFormView.aspx?orderID={0}"
                        DataTextField="CUSTOMER_NAME" DataTextFormatString="{0}" HeaderText="CUSTOMER_NAME"
                        Target="_blank" />
                    <asp:BoundField DataField="PRIORITY" HeaderText="Priority" SortExpression="PRIORITY">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CREATE_TIME" HeaderText="CreateTime" SortExpression="CREATE_TIME" HeaderStyle-CssClass="hidden-sm hidden-xs">
                        <ItemStyle HorizontalAlign="Center" CssClass="hidden-sm hidden-xs"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="CREATE_USER" HeaderText="CreateUser" SortExpression="CREATE_USER" HeaderStyle-CssClass="hidden-sm hidden-xs">
                        <ItemStyle HorizontalAlign="Center" CssClass="hidden-sm hidden-xs"/>
                    </asp:BoundField>
                </Columns>
                <PagerSettings Position="TopAndBottom" Mode="NextPreviousFirstLast" />
            </res:DeluxeGrid>
            <asp:ObjectDataSource ID="objectDataSource" runat="server" SelectCountMethod="GetFilteredDataCount"
                SelectMethod="GetFilteredData" TypeName="MCS.Web.Responsive.WebControls.Test.DeluxeGrid.OrdersDataViewAdapter"
                EnablePaging="True" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="prioritySelector" PropertyName="SelectedValue" Name="priority"
                        Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
    </div>
    </form>
</body>
</html>
