<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchList.aspx.cs" Inherits="MCS.OA.Portal.Search.SearchList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="HB" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>全文检索</title>
    <style type="text/css">
        .top
        {
            background-color: #F2F2F2;
            height: 26px;
        }
        .line
        {
            border-bottom-width: 1px;
            border-bottom-style: solid;
            border-bottom-color: #D4D0C8;
        }
    </style>
    <script type="text/javascript">
        function getDefaultTaskFeature() {
            var width = 820;
            var height = 700;

            var left = (window.screen.width - width) / 2;
            var top = (window.screen.height - height) / 2;

            return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server" style="width: 100%; height: 100%">
    <div id="container">
        <asp:ObjectDataSource ID="ObjectDataSourceFiles" runat="server" EnablePaging="True"
            SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
            TypeName="MCS.OA.Portal.Common.SearchFiles" EnableViewState="False" OnSelected="ObjectDataSourceFiles_Selected"
            OnSelecting="ObjectDataSourceFiles_Selecting">
            <SelectParameters>
                <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                    Type="String" />
                <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td align="center">
                    <table width="100%" cellspacing="0" cellpadding="0" border="0">
                        <tr class="top">
                            <td class="line" style="font-weight: bold;">
                                &nbsp;&nbsp; "<asp:Literal ID="LiteralApplicationName" runat="server"></asp:Literal>"中的检索结果：
                            </td>
                            <td class="line" style="text-align: right;" id="SearchIntroduction">
                                约有<asp:Label ID="LabelNum" runat="server" Text="?" Font-Bold="true"></asp:Label>项符合<asp:Label
                                    ID="LabelContent" runat="server" Font-Bold="true"></asp:Label>的查询结果
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <HB:DeluxeGrid ID="GridQuery" runat="server" Width="100%" DataSourceID="ObjectDataSourceFiles"
                        DataSourceMaxRow="0" ExportingDeluxeGrid="False" GridTitle="" ShowExportControl="False"
                        TitleColor="141, 143, 149" TitleFontSize="Large" AutoGenerateColumns="False"
                        AllowPaging="True" AllowSorting="True">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <table width="98%">
                                        <tr>
                                            <td style="width: 810px; word-wrap: break-word; overflow: hidden;">
                                                <a style="font-size: 16px;" href="<%# GetNormalizedUrl((string)Eval("ApplicationName"), (string)Eval("ProgramName"), (string)Eval("Url")) %>" target="MCSOAPortal"
                                                    onclick="window.open('', 'MCSOAPortal', getDefaultTaskFeature());">
                                                    <%# RenderQueryWordsTitle(Server.HtmlEncode(Eval("Subject").ToString()))%>
                                                </a>
                                                <div>
                                                    <%# RenderQueryWords(Server.HtmlEncode(Eval("Content").ToString()))%>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%# ((DateTime)Eval("CreateTime")).ToString("yyyy年MM月dd日") %>
                                                <asp:Literal ID="LiteralAppName" runat="server" Visible="false" />
                                                <div>
                                                    状态：<HBEX:WfStatusControl ID="WfStatusControl" runat="server" DisplayMode="ProcessStatus"
                                                        EnableUserPresence="true" ProcessID='<%# GetProcessID((string)Eval("ResourceID"),(string)Eval("Url"))%>' />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr style="height: 4px;">
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </HB:DeluxeGrid>
                </td>
            </tr>
            <tr>
                <td style="display: none; text-align: center;" id="NoResultTips">
                    <table width="98%">
                        <tr>
                            <td>
                                <br />
                                找不到和您的查询<asp:Label ID="LabelContent2" runat="server" Font-Bold="true"></asp:Label>相符的文件。
                                <br />
                                <br />
                                建议：
                                <ul>
                                    <li>请检查输入字词有无错误。</li>
                                    <li>请换用另外的查询字词。</li>
                                </ul>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <input runat="server" type="hidden" id="whereCondition" />
		<input runat="server" type="hidden" id="queryValue" />
    </div>
    </form>
</body>
</html>
