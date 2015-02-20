<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserUploadFileHistory.aspx.cs"
    Inherits="MCS.OA.CommonPages.UserOperationLog.UserUploadFileHistory" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>日志查看</title>
    <base target="_self" />
    <script type="text/javascript">
        //验证日期起始时间小于结束时间
        function verifyTime(beginTimeID, endTimeID) {
            var begin = document.getElementById(beginTimeID).value;
            var end = document.getElementById(endTimeID).value;
            var result = true;
            //有一个为空就不用验证
            if ((begin != "") && (end != "")) {
                var arrBegin = begin.split("-");
                var arrEnd = end.split("-");
                //构造Date对象
                var dateBegin = new Date(arrBegin[0], arrBegin[1] - 1, arrBegin[2]);
                var dateEnd = new Date(arrEnd[0], arrEnd[1] - 1, arrEnd[2]);

                if (dateBegin > dateEnd) {
                    alert("起始时间不能大于结束时间");
                    result = false;
                }
            }

            return result;
        }

        function openwindow(obj) {
            event.returnValue = false;

            window.showModalDialog(obj.href, "", "dialogHeight: 580px; dialogWidth: 600px;  edge: Raised; center: Yes; help: No; status: No;scroll: No;");
            event.cancelBubble = true;
            return false;
        }

        function downloadFile() {
            event.returnValue = false;
            var downElement = event.srcElement;

            window.open(downElement.href, "down", "dialogHeight: 10px; dialogWidth: 10px;  edge: Raised; center: No; help: No; status: No;scroll: No;");

            event.cancelBubble = true;

            return false;
        }

        function onDocumentLoad() {
            var parData = document.getElementById("processUsers");
            if (parData.ReadOnly == true) {
                document.getElementById("tdUsersName").innerHTML = "";
            }
        }
    </script>
</head>
<body style="background-color: #f8f8f8;">
    <form id="serverForm" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <div id="container">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
            <tr>
                <td style="height: 48px">
                    <div>
                        <SOA:DataBindingControl runat="server" ID="bindingControl" AutoCollectDataWhenPostBack="false">
                            <ItemBindings>
                                <SOA:DataBindingItem ControlID="dr_Status" ControlPropertyName="SelectedValue" Direction="ControlToData"
                                    DataPropertyName="Status" />
                                <SOA:DataBindingItem ControlID="StartDate" ControlPropertyName="Value" Direction="ControlToData"
                                    DataPropertyName="StartDate" />
                                <SOA:DataBindingItem ControlID="EndDate" ControlPropertyName="Value" Direction="ControlToData"
                                    DataPropertyName="EndDate" />
                            </ItemBindings>
                        </SOA:DataBindingControl>
                    </div>
                    <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;
                        margin-right: -12px;">
                        <div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
                            line-height: 30px; padding-bottom: 0px">
                            日志查看
                        </div>
                    </div>
                    <div style="margin-top: 7px; margin-bottom: 7px" runat="server" id="divSearch">
                        <table style="background-color: Silver; width: 100%; text-align: left" cellspacing="1px"
                            border="0" cellpadding="0">
                            <tr style="background-color: White">
                                <td style="text-align: center; background-color: #f2f8f8; width: 80px;">
                                    上传状态：
                                </td>
                                <td id="td2" runat="server" style="text-align: left; background-color: #f7fbfa; width: 220px;">
                                    &nbsp;&nbsp;
                                    <asp:DropDownList runat="server" AutoPostBack="false" ID="dr_Status" Style="width: 200px">
                                    </asp:DropDownList>
                                </td>
                                <td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 80px;">
                                    操作时间：
                                </td>
                                <td style="text-align: center; background-color: #f7fbfa; width: 220px;">
                                    <MCS:DeluxeCalendar ID="StartDate" runat="server" Width="70px">
                                    </MCS:DeluxeCalendar>至
                                    <MCS:DeluxeCalendar ID="EndDate" runat="server" Width="70px">
                                    </MCS:DeluxeCalendar>
                                </td>
                            </tr>
                            <tr style="background-color: White">
                                <td id="tdUsersName" runat="server" style="text-align: center; background-color: #f2f8f8;
                                    height: 30px; width: 80px">
                                    操作人：
                                </td>
                                <td id="tdUsersValue" runat="server" style="background-color: #f7fbfa; width: 220px;">
                                    &nbsp;&nbsp;
                                    <SOA:OuUserInputControl ID="processUsers" runat="server" MultiSelect="True" SelectMask="User"
                                        Style="width: 200px" ClassName="inputStyle" />
                                </td>
                                <td style="text-align: left; background-color: #f2f8f8; height: 30px; width: 80px">
                                    &nbsp;&nbsp;
                                    <asp:LinkButton runat="server" ID="srarch_link" OnClick="Srarch_Click" OnClientClick="return verifyTime('StartDate','EndDate');">
						             <img src="../../images/16/search.gif" alt="查询" style="border:0;"/>查询
                                    </asp:LinkButton>
                                </td>
                                <td style="text-align: center; background-color: #f7fbfa; width: 220px;">
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top">
                    <div style="margin-top: 7px; margin-bottom: 7px;">
                        <MCS:DeluxeGrid ID="DeluxeGridUploadLog" runat="server" ShowExportControl="true"
                            PageSize="10" TitleFontSize="Small" Width="98%" DataSourceID="ObjectDataSourceUploadLogs"
                            AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false" DataKeyNames="ID"
                            CheckBoxPosition="Right" PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
                            PagerSettings-NextPageText="下一页" PagerSettings-Position="TopAndBottom" CaptionAlign="Right"
                            CssClass="dataList" TitleCssClass="title" GridTitle="上传文件日志浏览">
                            <HeaderStyle CssClass="head" />
                            <RowStyle CssClass="item" />
                            <AlternatingRowStyle CssClass="aitem" />
                            <SelectedRowStyle CssClass="selecteditem" />
                            <PagerStyle CssClass="pager" />
                            <EmptyDataTemplate>
                                没有查询到您所需要的数据
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="文件名称">
                                    <ItemTemplate>
                                        <a href="DownloadHandler.ashx?id=<%#Eval("ID")%>" onclick="return downloadFile()"
                                            style="cursor: hand; text-decoration: underline; color: Blue;">
                                            <%#HttpUtility.HtmlEncode((string)Eval("CurrentFileName"))%></a>
                                    </ItemTemplate>
                                    <HeaderStyle Width="12%" />
                                    <ItemStyle HorizontalAlign="Left" Width="12%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="标题">
                                    <ItemTemplate>
                                        <%#Server.HtmlEncode((string)Eval("ProgramName"))%></ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="应用程序名称">
                                    <ItemTemplate>
                                        <%# Server.HtmlEncode((string)Eval("ApplicationName"))%></ItemTemplate>
                                    <HeaderStyle Width="12%" />
                                    <ItemStyle HorizontalAlign="Left" Width="12%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="操作人员">
                                    <ItemTemplate>
                                        <%#((MCS.Library.OGUPermission.IUser)Eval("Operator")).DisplayName%>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="CreateTime" HeaderText="操作时间" HtmlEncode="False" ItemStyle-Width="150px"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" SortExpression="CreateTime"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                <asp:TemplateField HeaderText="详细">
                                    <ItemTemplate>
                                        <a href='UploadFileLog.aspx?id=<%#Eval("ID")%>' onclick="openwindow(this);" style="cursor: hand;
                                            text-decoration: underline;">
                                            <img src="../images/chakan.gif" alt="" style="cursor: hand" />
                                        </a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="6%" />
                                </asp:TemplateField>
                            </Columns>
                            <PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
                                PreviousPageText="上一页"></PagerSettings>
                        </MCS:DeluxeGrid>
                        <SOA:DeluxeObjectDataSource ID="ObjectDataSourceUploadLogs" runat="server" EnablePaging="True"
                            EnableViewState="False" TypeName="MCS.OA.CommonPages.UploadFileHistoryQuery">
                        </SOA:DeluxeObjectDataSource>
                        <%--  <asp:ObjectDataSource ID="ObjectDataSourceUploadLogs" runat="server" EnablePaging="True"
                            SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
                            TypeName="MCS.OA.CommonPages.UploadFileHistoryQuery" EnableViewState="False"
                            OnSelected="ObjectDataSourceLogs_Selected" OnSelecting="ObjectDataSourceLogs_Selecting">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                                    Type="String" />
                                <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                       
                        <input runat="server" type="hidden" id="whereCondition" />--%>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 32px">
                    <div id="divClose" runat="server" style="height: 32px; text-align: center; vertical-align: middle;">
                        <input accesskey="C" id="btnClose" class="formButton" onclick="top.close();" type="button" category="OACommons" runat="server"
                            value="关闭(C)" name="btnClose" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            onDocumentLoad();
        });
    </script>
</body>
</html>
