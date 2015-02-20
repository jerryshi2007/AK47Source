<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReplaceAssigneeList.aspx.cs"
    Inherits="MCS.OA.Portal.TaskList.ReplaceAssigneeList" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>待办列表</title>
    <link href="../css.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../JavaScript/Grid.js"></script>
    <script type="text/javascript" src="../JavaScript/taskLink.js"></script>
    <script type="text/javascript" src="../JavaScript/replaceAssignee.js"></script>
    <script type="text/javascript">
        //按下回车时默认执行btnQuery按钮
        function document.onkeydown() {
            if (event.keyCode == 13) {
                document.getElementById("GridViewTask").focus();
                document.getElementById("btnQuery").click();
            }
        }

        //执行查询前的数据检查
        function queryClick() {
            //检查是否有用户被选中，如果没有用户被选中则提示错误信息
            var originalCtl = $find("originalUserSelector");
            if (originalCtl.get_selectedOuUserData().length == 0) {
                $showError("请选中原始待办人后再执行查询");
                return false;
            }

            return true;
        }
    </script>
</head>
<body class="portal" style="background-color: #f8f8f8;">
    <form id="serverForm" runat="server" style="width: 100%;">
    <asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="True">
        <Services>
            <asp:ServiceReference Path="../Services/PortalServices.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="container">
        <div>
            <!--存储原始待办人的控件-->
            <input type="hidden" id="hiddenOriginalUserID" runat="server" />
        </div>
        <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;
            margin-right: -12px;">
            <div style="text-indent: 1.5em; font-size: 11pt; background: url(../images/h2_icon.gif) no-repeat left center;
                line-height: 30px; width: 100%;">
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </div>
        </div>
        <div style="border-bottom: groove 1px Silver; overflow: hidden; width: 100%">
            <table style="width: 100%">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <span style="font-size: 11pt;">请选择原始待办人：</span>
                                </td>
                                <td>
                                    <!--原始待办人选择控件-->
                                    <HB:OuUserInputControl ID="originalUserSelector" runat="server" InvokeWithoutViewState="true"
                                        MultiSelect="false" Width="150" ListMask="All" SelectMask="User" CanSelectRoot="false" />
                                </td>
                                <td>
                                    <asp:Button runat="server" CssClass="portalButton" ID="btnQuery" Text="查找待办" OnClick="btnQuery_Click"
                                        OnClientClick="return queryClick()" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="text-align: right;">
                        <input type="button" class="portalButton" id="btnModify" runat="server" value="修改待办人" />
                    </td>
                </tr>
            </table>
        </div>
        <!--下面是要修改的部分-->
        <div style="margin: 5px;">
            <CCIC:DeluxeGrid ID="GridViewTask" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSourceTask"
                AllowPaging="True" AllowSorting="True" PageSize="20" ShowExportControl="False"
                ShowCheckBoxes="true" GridTitle="待办列表" CheckBoxPosition="Left" DataKeyNames="TaskID"
                OnRowDataBound="GridViewTask_RowDataBound" CssClass="dataList" TitleCssClass="title"
                Width="100%" DataSourceMaxRow="0" ExportingDeluxeGrid="False" TitleColor="141, 143, 149"
                TitleFontSize="Large">
                <HeaderStyle CssClass="headbackground" />
                <RowStyle CssClass="item" />
                <AlternatingRowStyle CssClass="aitem" />
                <SelectedRowStyle CssClass="selecteditem" />
                <PagerStyle CssClass="pager" />
                <CheckBoxTemplateItemStyle Width="5%" />
                <CheckBoxTemplateHeaderStyle Width="5%" />
                <EmptyDataTemplate>
                    请选择待办人以查询数据！
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="ProjectName" SortExpression="PROJECT_NAME" HeaderText="服务名称"
                        HeaderStyle-ForeColor="Black" Visible="false">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="文件类别" HeaderStyle-ForeColor="Black">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                            <%#Server.HtmlEncode((string)Eval("ApplicationName"))%>/<%#Server.HtmlEncode((string)Eval("ProgramName"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="标题" SortExpression="TASK_TITLE">
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DraftDepartmentName" Visible="true" HeaderText="单位" SortExpression="DRAFT_DEPARTMENT_NAME">
                        <HeaderStyle ForeColor="Black" />
                        <ItemStyle CssClass="bg_td1" HorizontalAlign="Center" Width="10%" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="待办人" HeaderStyle-ForeColor="Black">
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                            <label>
                                <%# Eval("SendToUserName")%></label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DeliverTime" SortExpression="DELIVER_TIME" HeaderText="接收时间"
                        DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-ForeColor="Black">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                    </asp:BoundField>
                </Columns>
                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                    NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
                <SelectedRowStyle CssClass="selecteditem" />
            </CCIC:DeluxeGrid>
            <asp:ObjectDataSource ID="ObjectDataSourceTask" runat="server" EnablePaging="True"
                SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
                TypeName="MCS.OA.Portal.Common.UnCompleteListQuery" EnableViewState="False" OnSelecting="ObjectDataSourceTask_Selecting"
                OnSelected="ObjectDataSourceTask_Selected">
                <SelectParameters>
                    <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                        Type="String" />
                    <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
        <div>
            <!--where查询条件-->
            <input runat="server" type="hidden" id="whereCondition" />
            <!--目的待办人选择控件-->
            <HB:UserSelector runat="server" ID="targetUserSelector" MultiSelect="true" DialogHeaderText="选择目的待办人"
                DialogTitle="选择目的待办人" ShowingMode="Normal" ListMask="All" />
            <!--进度条控件-->
            <HB:MultiProcessControl runat="server" ID="multiProcess" DialogTitle="正在提交修改" ControlIDToShowDialog="btnModify"
                OnClientBeforeStart="onBeforeStart" OnClientFinishedProcess="onFinished" OnExecuteStep="multiProcess_ExecuteStep"  />
        </div>
    </div>
    </form>
</body>
</html>
