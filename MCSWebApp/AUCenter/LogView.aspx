<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogView.aspx.cs" Inherits="AUCenter.LogView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>日志详情</title>
    <base target="_self" />
    <%--<au:HeaderControl ID="HeaderControl1" runat="server" />--%>
    <style type="text/css">
        .col-chekbox input, .checkbox input
        {
            visibility: hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <%--<au:BannerNotice ID="notice" runat="server"></au:BannerNotice>--%>
    <div class="pc-search-box-wrapper">
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)"
                CssClass="deluxe-search deluxe-left" SearchField="Subject" OnSearching="SearchButtonClick"
                OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="true">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True"
                OnClientDataBinding="searchClientBind">
                <ItemBindings>
                    <soa:DataBindingItem ControlID="sfOp" DataPropertyName="OperatorName" />
                    <soa:DataBindingItem ControlID="sfOpReal" DataPropertyName="RealOperatorName" />
                    <soa:DataBindingItem ControlID="sfStart" DataPropertyName="AfterForBind" ClientPropName="get_value"
                        FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfEnd" DataPropertyName="BeforeForBind" ClientPropName="get_value"
                        FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table border="0" cellpadding="0" cellspacing="0" class="pc-search-grid-duo">
                    <tr>
                        <td>
                            <label for="sfOp" class="pc-label">
                                操作人
                            </label>
                            <asp:TextBox runat="server" ID="sfOp" MaxLength="10" CssClass="pc-textbox" />(精确)
                        </td>
                        <td>
                            <label for="sfOpActual" class="pc-label">
                                实际操作人
                            </label>
                            <asp:TextBox runat="server" ID="sfOpReal" MaxLength="10" CssClass="pc-textbox" />(精确)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="sfOp" class="pc-label">
                                晚于
                            </label>
                            <mcs:DeluxeDateTime runat="server" ID="sfStart" />
                        </td>
                        <td>
                            <label for="sfOpActual" class="pc-label">
                                早于
                            </label>
                            <mcs:DeluxeDateTime runat="server" ID="sfEnd" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <div class="pc-container5">
        <div class="pc-listmenu-container">
        </div>
        <div class="pc-grid-container">
            <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                AllowPaging="True" AllowSorting="True" ShowCheckBoxes="false" Category="PermissionCenter"
                GridTitle="操作日志" DataKeyNames="ID" CssClass="dataList pc-datagrid" TitleCssClass="title"
                ShowExportControl="true" PagerSettings-Position="Bottom" DataSourceMaxRow="0"
                TitleColor="141, 143, 149" TitleFontSize="Large" OnRowCommand="HandleRowCommand">
                <EmptyDataTemplate>
                    暂时没有您需要的数据
                </EmptyDataTemplate>
                <HeaderStyle CssClass="head" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="checkbox" />
                        <HeaderTemplate>
                            <input type="checkbox" />
                        </HeaderTemplate>
                        <ItemStyle />
                        <ItemTemplate>
                            <div class="col-chekbox">
                                <input type="checkbox" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="时间" SortExpression="CreateTime">
                        <ItemTemplate>
                            <div>
                                <asp:Label ID="lblTime" runat="server" Text='<%# Bind("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label>
                            </div>
                            <div>
                                <div id="d" class="pc-action-tray" runat="server" visible='<%# this.gridMain.ExportingDeluxeGrid == false %>'>
                                    <asp:LinkButton ID="lnkTimeTrip" runat="server" Text="时间穿梭" CssClass="pc-item-cmd"
                                        CommandName="Shuttle" CommandArgument='<%#Eval("CreateTime") %>' data-time='<%# ((DateTime)Eval("CreateTime")).ToBinary() %>'
                                        OnClientClick="return shuttle(this);"></asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作类别" SortExpression="OperationType">
                        <ItemTemplate>
                            <i class='<%#Eval("SchemaType","pc-icon16 {0}") %>'></i>
                            <au:LogOperationLabel runat="server" OperationType='<%#Eval("OperationType") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <mcs:GridColumnSorter runat="server" DefaultOrderName="操作人">
							<SortItems>
							<mcs:SortItem SortExpression="OperatorName" Text="操作人" />
							<mcs:SortItem SortExpression="OperatorName" Text="实际操作人" />
							</SortItems>
                            </mcs:GridColumnSorter>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div>
                                <soa:UserPresence runat="server" UserDisplayName='<%# Server.HtmlEncode((string)Eval("OperatorName").ToString()) %>'
                                    UserID='<%#Eval("OperatorID") %>' ShowUserDisplayName="true" StatusImage="Ball" />
                                (实际操作人：
                                <soa:UserPresence runat="server" UserDisplayName='<%# Server.HtmlEncode(Eval("RealOperatorName").ToString()) %>'
                                    UserID='<%#Eval("RealOperatorID") %>' ShowUserDisplayName="true" StatusImage="Ball" />
                                )
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Subject" HeaderText="标题" SortExpression="Subject" HtmlEncode="true" />
                </Columns>
                <PagerStyle CssClass="pager" />
                <RowStyle CssClass="item" />
                <CheckBoxTemplateItemStyle CssClass="checkbox" />
                <AlternatingRowStyle CssClass="aitem" />
                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                    NextPageText="下一页" Position="TopAndBottom" PreviousPageText="上一页"></PagerSettings>
                <SelectedRowStyle CssClass="selecteditem" />
                <CheckBoxTemplateHeaderStyle CssClass="checkbox" />
                <PagerTemplate>
                </PagerTemplate>
            </mcs:DeluxeGrid>
        </div>
    </div>
    <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
        TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.SchemaLogDataSource"
        EnableViewState="false" OnSelecting="dataSourceMain_Selecting">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="ou" Type="String" Name="catelog" DefaultValue="" />
        </SelectParameters>
    </soa:DeluxeObjectDataSource>
    </form>
    <script type="text/javascript">
        if (window.parent.showLoader)
            window.parent.showLoader(false);
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.listMenuBehavior("listMenu");

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function searchClientBind(sender, e) {
            switch (e.bindingItem.ControlID) {
                case "sfStart":
                case "sfEnd":
                    e.cancel = true;
                    var v = e.data[e.bindingItem.ClientDataPropertyName];
                    if (v.getTime() === Date.minDate.getTime()) {
                        $find(e.bindingItem.ControlID).set_value(null);
                    } else {
                        $find(e.bindingItem.ControlID).set_value(v);
                    }


                    break;
                default:
                    break;
            }
        }

        function shuttle(elem) {
            var time = $pc.getAttr(elem, "data-time");
            if (time) {
                window.top.shuttle.apply(window.top, [time]);
            }

            return false;
        }
    </script>
</body>
</html>
