<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowList.aspx.cs" Inherits="PermissionCenter.FlowList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>相关流程列表</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <base target="_self" />
    <pc:HeaderControl ID="HeaderControl1" runat="server">
    </pc:HeaderControl>
    <style type="text/css">
        .pc-workflow
        {
            background-image: url('../images/flow.jpg');
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <%--<pc:SceneControl ID="SceneControl1" runat="server">
	</pc:SceneControl>--%>
    <pc:BannerNotice ID="notice" runat="server" RenderType="Auto"></pc:BannerNotice>
    <h1 class="pc-caption">
        <asp:Literal ID="appName" runat="server" Mode="Encode"></asp:Literal><span class="pc-timepointmark"><mcs:TimePointDisplayControl
            ID="TimePointDisplayControl1" runat="server">
        </mcs:TimePointDisplayControl>
        </span>
    </h1>
    <div class="pc-frame-container">
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchField="D.PROCESS_KEY" SearchMethod="PrefixLike" OnSearching="SearchButtonClick"
                OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="true" DefaultTip="搜索流程Key">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True"
                OnClientDataBinding="searchClientBind">
                <ItemBindings>
                    <soa:DataBindingItem ControlID="sfAppName" DataPropertyName="ApplicationName" />
                    <soa:DataBindingItem ControlID="sfProgramName" DataPropertyName="ProgramName" />
                    <soa:DataBindingItem ControlID="sfModifier" DataPropertyName="ModifierName" />
                    <soa:DataBindingItem ControlID="sfImporter" DataPropertyName="ImportUserName" />
                    <soa:DataBindingItem ControlID="sfCreationDateFrom" DataPropertyName="CreationDateFrom"
                        ClientPropName="get_value" FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfCreationDateTo" DataPropertyName="CreationDateTo"
                        ClientPropName="get_value" FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfModificationDateFrom" DataPropertyName="ModificationDateFrom"
                        ClientPropName="get_value" FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfModificationDateTo" DataPropertyName="ModificationDateTo"
                        ClientPropName="get_value" FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfImportDateFrom" DataPropertyName="ImportDateFrom"
                        ClientPropName="get_value" FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                    <soa:DataBindingItem ControlID="sfImportDateTo" DataPropertyName="ImportDateTo" ClientPropName="get_value"
                        FormatDefaultValueToEmpty="true" ClientSetPropName="set_value" />
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table border="0" cellpadding="0" cellspacing="0" class="pc-search-grid-duo">
                    <tr>
                        <td>
                            <label for="sfAppName" class="pc-label">
                                应用名</label><asp:TextBox runat="server" ID="sfAppName" MaxLength="56" CssClass="pc-textbox" />(前缀)
                        </td>
                        <td>
                            <label for="sfProgramName" class="pc-label">
                                项目名</label><asp:TextBox runat="server" ID="sfProgramName" MaxLength="56" CssClass="pc-textbox" />(前缀)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="sfModifier" class="pc-label">
                                修改者</label><asp:TextBox runat="server" ID="sfModifier" MaxLength="56" CssClass="pc-textbox" />(前缀)
                        </td>
                        <td>
                            <label for="sfImporter" class="pc-label">
                                导入者</label><asp:TextBox runat="server" ID="sfImporter" MaxLength="56" CssClass="pc-textbox" />(前缀)
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <label for="sfCreationDateFrom" class="pc-label">
                                创建日期</label><mcs:DeluxeCalendar runat="server" ID="sfCreationDateFrom">
                                </mcs:DeluxeCalendar>~<mcs:DeluxeCalendar runat="server" ID="sfCreationDateTo">
                                </mcs:DeluxeCalendar><asp:CustomValidator ID="validation1" ErrorMessage="开始日期不得大于结束日期"
                                    ClientValidationFunction="validateDate1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <label for="sfModificationDateFrom" class="pc-label">
                                修改日期</label><mcs:DeluxeCalendar runat="server" ID="sfModificationDateFrom">
                                </mcs:DeluxeCalendar>~<mcs:DeluxeCalendar runat="server" ID="sfModificationDateTo">
                                </mcs:DeluxeCalendar><asp:CustomValidator ID="validation2" ErrorMessage="开始日期不得大于结束日期"
                                    ClientValidationFunction="validateDate2" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <label for="sfImportDateFrom" class="pc-label">
                                导入日期</label><mcs:DeluxeCalendar runat="server" ID="sfImportDateFrom">
                                </mcs:DeluxeCalendar>~<mcs:DeluxeCalendar runat="server" ID="sfImportDateTo">
                                </mcs:DeluxeCalendar><asp:CustomValidator ID="validation3" ErrorMessage="开始日期不得大于结束日期"
                                    ClientValidationFunction="validateDate3" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="pc-container5">
            <div class="pc-listmenu-container">
                <ul class="pc-listmenu" id="listMenu">
                </ul>
            </div>
            <div class="pc-grid-container">
                <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                    AllowPaging="True" AllowSorting="True" ShowCheckBoxes="False" Category="PermissionCenter"
                    GridTitle="相关流程列表" DataKeyNames="PROCESS_KEY" CssClass="dataList pc-datagrid"
                    TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" ShowExportControl="true"
                    TitleColor="141, 143, 149" TitleFontSize="Large">
                    <EmptyDataTemplate>
                        暂时没有您需要的数据
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="head" />
                    <Columns>
                        <asp:TemplateField HeaderText="流程Key" SortExpression="D.PROCESS_KEY">
                            <ItemTemplate>
                                <div>
                                    <asp:HyperLink ID="lnk" runat="server" CssClass="pc-item-link" NavigateUrl='<%#Eval("PROCESS_KEY","~/Handlers/Transfer.ashx?id={0}&type=processKey") %>'
                                        Target="_blank"><i class="pc-icon16 pc-workflow"></i><%# Server.HtmlEncode((string)Eval("PROCESS_KEY"))%></asp:HyperLink></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="APPLICATION_NAME" HtmlEncode="true" HeaderText="应用名" SortExpression="D.APPLICATION_NAME" />
                        <asp:BoundField DataField="PROGRAM_NAME" HtmlEncode="true" HeaderText="项目名" SortExpression="D.PROGRAM_NAME" />
                        <asp:TemplateField HeaderText="修改者" SortExpression="D.MODIFIER_NAME">
                            <ItemTemplate>
                                <soa:UserPresence runat="server" ID="userPresence1" ShowUserDisplayName="true" ShowUserIcon="false"
                                    UserID='<%#Eval("MODIFIER_ID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("MODIFIER_NAME").ToString()) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="导入者" SortExpression="D.IMPORT_USER_NAME">
                            <ItemTemplate>
                                <soa:UserPresence runat="server" ID="up1" ShowUserDisplayName="true" ShowUserIcon="false"
                                    UserID='<%#Eval("IMPORT_USER_ID") %>' UserDisplayName='<%# Server.HtmlEncode(Eval("IMPORT_USER_NAME").ToString()) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CREATE_TIME" HtmlEncode="true" HeaderText="创建日期" SortExpression="D.CREATE_TIME"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <asp:BoundField DataField="MODIFY_TIME" HtmlEncode="true" HeaderText="修改日期" SortExpression="D.MODIFY_TIME"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <asp:BoundField DataField="IMPORT_TIME" HtmlEncode="true" HeaderText="导入日期" SortExpression="D.IMPORT_TIME"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
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
    </div>
    <soa:DeluxeObjectDataSource ID="dataSourceMain" runat="server" EnablePaging="True"
        TypeName="PermissionCenter.DataSources.ProcessTemplateSource" EnableViewState="false"
        OnSelecting="dataSourceMain_Selecting">
        <SelectParameters>
            <asp:QueryStringParameter DbType="String" Name="id" QueryStringField="id" />
        </SelectParameters>
    </soa:DeluxeObjectDataSource>
    <pc:Footer ID="footer" runat="server" />
    </form>
    <script type="text/javascript">
        $pc.ui.listMenuBehavior("listMenu");
        $pc.ui.gridBehavior("gridMain", "hover");
        $pc.ui.traceWindowWidth();

        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function validateDate(a, b, e) {
            var val1 = $find(a).get_value();
            var val2 = $find(b).get_value();
            if (val1 != Date.minDate && val2 != Date.minDate) {
                e.IsValid = val1 <= val2;
            }
        }

        function validateDate1(s, e) {
            validateDate("sfCreationDateFrom", "sfCreationDateTo", e);
        }

        function validateDate2(s, e) {
            validateDate("sfModificationDateFrom", "sfModificationDateTo", e);
        }

        function validateDate3(s, e) {
            validateDate("sfImportDateFrom", "sfImportDateTo", e);
        }

        function searchClientBind(sender, e) {
            switch (e.bindingItem.ControlID) {
                case "sfCreationDateFrom":
                case "sfCreationDateTo":
                case "sfModificationDateFrom":
                case "sfModificationDateTo":
                case "sfImportDateFrom":
                case "sfImportDateTo":
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
    </script>
</body>
</html>
