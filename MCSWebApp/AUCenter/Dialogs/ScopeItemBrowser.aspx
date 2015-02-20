<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScopeItemBrowser.aspx.cs"
    Inherits="AUCenter.Dialogs.ScopeItemBrowser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Styles/pccom.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/aumain.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/dlg.css" rel="stylesheet" type="text/css" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            选择范围对象
        </h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-search-box-wrapper">
            <soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
                HasCategory="False" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)" SearchField="S.SearchContent"
                OnSearching="SearchButtonClick" OnConditionClick="onconditionClick" CustomSearchContainerControlID="advSearchPanel"
                HasAdvanced="false">
            </soa:DeluxeSearch>
            <soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
                <ItemBindings>
                </ItemBindings>
            </soa:DataBindingControl>
            <div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
                <asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
                <table class="pc-search-grid-duo">
                    <tr>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="pc-grid-container">
            <mcs:DeluxeGrid ID="gridMain" runat="server" AutoGenerateColumns="False" DataSourceID="dataSourceMain"
                AllowPaging="True" AllowSorting="True" ShowCheckBoxes="True" Category="PermissionCenter"
                ShowExportControl="true" GridTitle="管理Schema" DataKeyNames="ID" CssClass="dataList pc-datagrid"
                TitleCssClass="title" PagerSettings-Position="Bottom" DataSourceMaxRow="0" TitleColor="141, 143, 149"
                TitleFontSize="Large">
                <EmptyDataTemplate>
                    暂时没有您需要的数据
                </EmptyDataTemplate>
                <HeaderStyle CssClass="head" />
                <Columns>
                    <asp:BoundField HeaderText="名称" SortExpression="S.AUScopeItemName" DataField="AUScopeItemName"
                        HtmlEncode="true" />
                    <asp:BoundField DataField="CreateDate" HtmlEncode="true" HeaderText="创建日期" SortExpression="S.CreateDate"
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
            <soa:DeluxeObjectDataSource runat="server" ID="dataSourceMain" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.AdminScopeItemDataSource"
                EnablePaging="True">
                <SelectParameters>
                    <asp:QueryStringParameter Type="String" ConvertEmptyStringToNull="true" Name="scopeType"
                        QueryStringField="scopeType" />
                </SelectParameters>
            </soa:DeluxeObjectDataSource>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" value="确定" class="pcdlg-button" onclick="okClick();" />
            <input type="button" value="取消" class="pcdlg-button" onclick="cancelClick();" />
        </div>
    </div>
    <script type="text/javascript">
        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function okClick() {
            var keys = $find("gridMain").get_clientSelectedKeys();
            if (keys.length) {
                if (typeof (window.dialogArguments) == 'object' && typeof (window.dialogArguments.fillElem) == "object") {
                    window.dialogArguments.fillElem.value = keys.join(",");
                    window.returnValue = true;
                } else {
                    window.returnValue = keys.join(",");
                }
                window.close();

            }
        }

        function cancelClick() {
            window.returnValue = false;
            window.close();

        }
    
    </script>
    </form>
</body>
</html>
