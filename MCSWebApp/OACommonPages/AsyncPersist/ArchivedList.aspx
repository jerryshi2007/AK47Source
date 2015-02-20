<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArchivedList.aspx.cs" Inherits="MCS.OA.CommonPages.AsyncPersist.ArchivedList"
    Theme="Platform" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>已持久化流程数据</title>
    <link href="../../CSS/overrides.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/templatecss.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <script type="text/javascript">
        function onConditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }
    </script>
    <form id="form1" runat="server">
    <div class="t-container">
        <div class="t-dialog-caption">
            <a href="Monitor.aspx" class="t-dialog-caption">未持久化流程数据监控 </a><span class="t-dialog-caption t-dialog-caption-active">
                已持久化流程数据监控</span>
        </div>
        <div class="t-search-area">
            <soa:DeluxeSearch runat="server" ID="search1" HasCategory="false" CustomSearchContainerControlID="advSearchPanel"
                OnConditionClick="onConditionClick" SearchMethod="Like" SearchField="PROCESS_NAME"
                OnSearching="SearchButtonClick" HasAdvanced="True">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
                <ItemBindings>
                    <soa:DataBindingItem ControlID="sfProcessName" DataPropertyName="ProcessName" />
                    <soa:DataBindingItem ControlID="sfDepartment" DataPropertyName="Department" />
                    <soa:DataBindingItem ControlID="sfCreator" DataPropertyName="CreatorName" />
                    <soa:DataBindingItem ControlID="sfCreationTimeFrom" DataPropertyName="CreationTimeFrom"
                        ClientPropName="get_value" ClientSetPropName="set_value" ClientIsHtmlElement="false" />
                    <soa:DataBindingItem ControlID="sfCreationTimeTo" DataPropertyName="CreationTimeTo"
                        ClientPropName="get_value" ClientSetPropName="set_value" ClientIsHtmlElement="false" />
                    <soa:DataBindingItem ControlID="sfStartTimeFrom" DataPropertyName="StartTimeFrom"
                        ClientPropName="get_value" ClientSetPropName="set_value" ClientIsHtmlElement="false" />
                    <soa:DataBindingItem ControlID="sfStartTimeTo" DataPropertyName="StartTimeTo" ClientPropName="get_value"
                        ClientSetPropName="set_value" ClientIsHtmlElement="false" />
                </ItemBindings>
            </soa:DataBindingControl>
        </div>
        <div id="advSearchPanel" style="display: none">
            <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
            <table>
                <tr>
                    <td>
                        <label for="sfProcessName" class="t-label">
                            流程名称</label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="sfProcessName" MaxLength="56" CssClass="pc-textbox" />
                    </td>
                    <td>
                        <label for="sfDepartment" class="t-label">
                            单位</label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="sfDepartment" MaxLength="56" CssClass="pc-textbox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="sfCreator" class="t-label">
                            创建人</label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="sfCreator" MaxLength="56" CssClass="pc-textbox" />
                    </td>
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <tr>
                        <td>
                            <label for="sfCreationTimeFrom" class="t-label">
                                创建时间
                            </label>
                        </td>
                        <td colspan="3">
                            <mcs:DeluxeDateTime ID="sfCreationTimeFrom" runat="server" />
                            ~
                            <mcs:DeluxeDateTime ID="sfCreationTimeTo" runat="server" />
                            <asp:CustomValidator ID="CustomValidator1" ErrorMessage="×" runat="server" ClientValidationFunction="validateCreationTime"
                                ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="sfStartTimeFrom" class="t-label">
                                开始时间
                            </label>
                        </td>
                        <td colspan="3">
                            <mcs:DeluxeDateTime ID="sfStartTimeFrom" runat="server" />
                            ~
                            <mcs:DeluxeDateTime ID="sfStartTimeTo" runat="server" />
                            <asp:CustomValidator ID="CustomValidator2" ErrorMessage="×" runat="server" ForeColor="Red"
                                ClientValidationFunction="validateStartTime" />
                        </td>
                    </tr>
            </table>
        </div>
    </div>
    <div class="t-grid-container">
        <mcs:DeluxeGrid ID="gridViewTask" runat="server" AutoGenerateColumns="False" DataSourceID="src1"
            AllowPaging="True" AllowSorting="True" PageSize="10" ShowExportControl="False"
            OnExportData="GridViewTask_ExportData" GridTitle="流程列表" ShowCheckBoxes="False"
            DataKeyNames="PROCESS_ID" OnRowDataBound="GridViewTask_RowDataBound" CssClass="dataList gtasks"
            TitleCssClass="title" Width="100%" DataSourceMaxRow="0" TitleColor="141, 143, 149"
            TitleFontSize="Large" CascadeControlID="" SkinID="gridSkin">
            <CheckBoxTemplateHeaderStyle Width="16px" />
            <EmptyDataTemplate>
                暂时没有您需要的数据
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="PROCESS_NAME" SortExpression="PROCESS_NAME" HeaderText="流程名称">
                </asp:BoundField>
                <asp:BoundField DataField="DEPARTMENT_NAME" HeaderText="单位" SortExpression="DEPARTMENT_NAME">
                </asp:BoundField>
                <asp:BoundField DataField="CREATE_TIME" SortExpression="B.CREATE_TIME" HeaderText="流程创建时间"
                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundField>
                <asp:TemplateField SortExpression="CREATOR_NAME" HeaderText="创建人">
                    <ItemTemplate>
                        <span style="margin-left: 16px">
                            <soa:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("CREATOR_ID") %>'
                                UserDisplayName='<%# Eval("CREATOR_NAME") %>' />
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="START_TIME" SortExpression="START_TIME" HeaderText="流程开始时间"
                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundField>
                <asp:BoundField DataField="UPDATE_TAG" SortExpression="A.UPDATE_TAG" HeaderText="TAG">
                </asp:BoundField>
                <asp:BoundField DataField="GEN_TIME" SortExpression="A.CREATE_TIME" HeaderText="更新时间"
                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundField>
            </Columns>
            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                NextPageText="下一页" Position="TopAndBottom" PreviousPageText="上一页"></PagerSettings>
            <SelectedRowStyle CssClass="selecteditem" />
        </mcs:DeluxeGrid>
        <soa:DeluxeObjectDataSource runat="server" ID="src1" EnablePaging="true" TypeName="MCS.OA.CommonPages.AsyncPersist.AchivedDataSource"
            OnSelecting="ObjectDataSourceSelecting">
            <SelectParameters>
                <asp:Parameter DbType="String" DefaultValue="" Name="where"></asp:Parameter>
            </SelectParameters>
        </soa:DeluxeObjectDataSource>
    </div>
    </form>
    <script type="text/javascript">
        function validateCreationTime(s, e) {
            var date1 = $find("sfCreationTimeFrom").get_value();
            var date2 = $find("sfCreationTimeTo").get_value();

            if (date1 != Date.minDate && date2 != Date.minDate) {
                e.IsValid = date1 <= date2;
            }
        }

        function validateStartTime(s, e) {
            var date1 = $find("sfStartTimeFrom").get_value();
            var date2 = $find("sfStartTimeTo").get_value();

            if (date1 != Date.minDate && date2 != Date.minDate) {
                e.IsValid = date1 <= date2;
            }
        }
    </script>
</body>
</html>
