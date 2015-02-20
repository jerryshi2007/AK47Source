<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompletedTaskList.aspx.cs"
    Inherits="MCS.OA.Portal.TaskList.CompletedTaskList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCXC" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCSOAPortal" Namespace="MCS.OA.Portal" TagPrefix="Portal" %>
<%@ Register TagPrefix="Portal" Namespace="MCS.OA.Portal.Common" Assembly="MCSOAPortal" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>已办列表</title>
    <link href="../css.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../JavaScript/taskQuery.js"></script>
    <script src="../JavaScript/taskLink.js" type="text/javascript"></script>
    <script type="text/javascript" src="../JavaScript/Grid.js"></script>
    <script type="text/javascript">
        function onAdvancedSearchClick(mode) {
            event.returnValue = false;

            var sFeature = "dialogHeight: 300px; dialogWidth: 400px; edge: Raised; center: Yes; help: No; status: No;scroll: No;";
            var arg = new Object();
            setControlValuesToObject(arg, "SinoOceanFormType_categorySelect", "SinoOceanFormType_programSelect", "subjectInput", "beginTimeInput", "endTimeInput", "emergencySelector");

            var result = window.showModalDialog("CompletedTaskAdvancedSearch.aspx?mode=" + mode, arg, sFeature);
            if (result) {
                setObjectToControlValues(result, "SinoOceanFormType_categorySelect", "SinoOceanFormType_programSelect", "subjectInput", "beginTimeInput", "endTimeInput", "emergencySelector");

                $get("ButtonAdvanced").click();
            }
        }

        //按回车时点击查询按钮
        function document.onkeydown() {
            if (event.keyCode == 13) {
                if (event.srcElement.id != "GridViewTask_ctl01_txtPageCode") {
                    document.getElementById("queryBtn").focus();
                    document.getElementById("queryBtn").click();
                }
            }
        }

        function ShowingPosition(sender, e) {
            var cssId = document.getElementsByName("ajax__calendar").CssClass;
            //            if (event.srcElement.style.marginBottom >= document.body.style.marginBottom) 
            //            {
            //                document.location.event.srcElement.style.marginBottom = document.body.style.marginBottom;
            //            }
        }

        function createCols(gridViewTask) {
            gridViewTask = gridViewTask.firstChild;

            var cols = new Array();

            var width, col;

            for (var i = 0; i < gridViewTask.childNodes[2].childNodes.length; i++) {
                width = gridViewTask.childNodes[2].childNodes[i].style.width;

                if (width != null)
                    col = window.document.createElement("<col style='width:" + width + "'/>");
                else
                    col = window.document.createElement("<col/>");

                Array.add(cols, col);
            }

            return cols;
        }
        function onLogclick(resourceID, processID) {
            var strLink = "../../OACommonPages/UserOperationLog/UserOperationLogView.aspx?resourceID=" + resourceID + "&processID=" + processID;
            window.showModalDialog(strLink, "", "dialogHeight: 500px; dialogWidth: 800px; resizable:yes; edge: Raised; center: Yes; help: No; status: No;scroll: No;");
        }
    </script>
</head>
<body class="portal" style="background-color: #f8f8f8;">
    <form id="serverForm" runat="server">
    <asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="True">
        <Services>
            <asp:ServiceReference Path="../Services/PortalServices.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="container">
        <div>
            <input type="hidden" id="emergencySelector" runat="server" value="-1" />
            <input type="hidden" id="TextBoxPurpose" runat="server" />
            <input type="hidden" id="PersonID" runat="server" />
            <input type="hidden" id="FromPerson" runat="server" />
            <input type="hidden" id="OrgID" runat="server" />
            <input type="hidden" id="FormCategory" runat="server" />
            <input type="hidden" id="SinoOceanFormType_categorySelect" runat="server" />
            <input type="hidden" id="SinoOceanFormType_programSelect" runat="server" />
        </div>
        <div>
            <HB:DataBindingControl runat="server" ID="bindingControl">
                <ItemBindings>
                    <HB:DataBindingItem ControlID="subjectInput" ControlPropertyName="Text" DataPropertyName="TaskTitle"
                        Direction="ControlToData" />
                    <HB:DataBindingItem ControlID="beginTimeInput" ControlPropertyName="Value" DataPropertyName="CompletedTimeBegin"
                        Direction="ControlToData" />
                    <HB:DataBindingItem ControlID="endTimeInput" ControlPropertyName="Value" DataPropertyName="CompletedTimeEnd"
                        Direction="ControlToData" />
                    <HB:DataBindingItem ControlID="TBDraftDept" ControlPropertyName="Text" DataPropertyName="DraftDepartmentName"
                        Direction="ControlToData" />
                </ItemBindings>
            </HB:DataBindingControl>
        </div>
        <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;
            margin-right: -12px;">
            <div style="text-indent: 1.5em; font-size: 11pt; background: url(../images/h2_icon.gif) no-repeat left center;
                line-height: 30px; width: 99%;">
                <asp:Label ID="LblTitle" runat="server" Text="已办事项"></asp:Label>
            </div>
        </div>
        <!--下面是要修改的部分-->
        <div style="margin: 5px;">
            <CCXC:DeluxeGrid ID="GridViewTask" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSourceTask"
                AllowPaging="True" AllowSorting="True" PageSize="20" ShowExportControl="False"
                OnExportData="GridViewTask_ExportData" GridTitle="已办列表" CheckBoxPosition="Right"
                DataKeyNames="TaskID" OnRowDataBound="GridViewTask_RowDataBound" OnPreRender="GridViewTask_PreRender"
                TitleFontSize="Small" CssClass="dataList" TitleCssClass="title" Width="99%" DataSourceMaxRow="0"
                ExportingDeluxeGrid="False" TitleColor="141, 143, 149">
                <HeaderStyle CssClass="headbackground" />
                <RowStyle CssClass="titem" />
                <AlternatingRowStyle CssClass="taitem" />
                <SelectedRowStyle CssClass="selecteditem" />
                <PagerStyle CssClass="pager" />
                <EmptyDataTemplate>
                    暂时没有您需要的数据
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
                    <asp:BoundField DataField="CompletedTime" HtmlEncode="False" SortExpression="COMPLETED_TIME"
                        HeaderText="办理时间" Visible="false">
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="状态">
                        <HeaderStyle ForeColor="Black" />
                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                        <ItemTemplate>
                            <HB:WfStatusControl ID="WfStatusControl1" runat="server" ProcessID='<%# Eval("ProcessID")%>'
                                DisplayMode="CurrentUsers" EnableUserPresence="true">
                            </HB:WfStatusControl>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="DRAFT_USER_NAME" HeaderText="拟单人" Visible="false">
                        <ItemStyle HorizontalAlign="Left" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                            <span style="margin-left: 16px">
                                <HB:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("DraftUserID") %>'
                                    UserDisplayName='<%# Eval("DraftUserName") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="详细信息" HeaderStyle-ForeColor="Black">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                            <a href="#" onclick="onLogclick('<%# Eval("ResourceID")%>','<%# Eval("ProcessID")%>')">查看</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                    NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
            </CCXC:DeluxeGrid>
            <asp:ObjectDataSource ID="ObjectDataSourceTask" runat="server" EnablePaging="True"
                SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
                TypeName="MCS.OA.Portal.Common.CompleteListQuery" EnableViewState="False" OnSelecting="ObjectDataSourceTask_Selecting"
                OnSelected="ObjectDataSourceTask_Selected">
                <SelectParameters>
                    <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                        Type="String" />
                    <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
        <div id="divSearch" style="margin: 5px;">
            <table style="background-color: Silver; width: 99%; text-align: left" cellspacing="1px"
                border="0" cellpadding="0">
                <tr>
                    <td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 80px;">
                        单位：
                    </td>
                    <td style="text-align: center; background-color: #f7fbfa; width: 130px;">
                        <asp:TextBox ID="TBDraftDept" runat="server" MaxLength="100" Width="120px" />
                    </td>
                    <td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 60px;">
                        标题：
                    </td>
                    <td style="text-align: center; background-color: #f7fbfa; width: 130px;">
                        <asp:TextBox ID="subjectInput" runat="server" MaxLength="100" Width="120px" />
                    </td>
                    <td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 80px;">
                        办理时间：
                    </td>
                    <td style="text-align: center; background-color: #f7fbfa; width: 210px;">
                        <CCXC:DeluxeCalendar ID="beginTimeInput" runat="server" Width="70px">
                        </CCXC:DeluxeCalendar>
                        至
                        <CCXC:DeluxeCalendar ID="endTimeInput" runat="server" Width="70px">
                        </CCXC:DeluxeCalendar>
                    </td>
                    <td style="text-align: left; background-color: #f2f8f8; height: 30px;">
                        &nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="queryBtn" OnClick="QueryBtnClick" OnClientClick="return verifyTime('beginTimeInput','endTimeInput'); ">
						<img src="../../Images/16/search.gif" alt="查询" style="border:0;"/>查询
                        </asp:LinkButton>
                    </td>
                </tr>
                <%-- <tr style="background-color: White; height: 30px;">
					<td style="background-color: #f7fbfa; text-align: right;" align="right" colspan="7">
						<Portal:ApplicationNameDropDownList ID="DropDownListApplicationName" runat="server"
							Visible="False" />
						<asp:Button runat="server" ID="ButtonAdvanced" Text="高级查询" OnClick="ButtonAdvanced_Click"
							CssClass="hidden" />
						<a id="advancedSearch" href="#" runat="server">
							<img src="../../Images/16/find.gif" style="border: 0; margin-bottom: -3px;" alt="高级查询" />高级查询</a>
					</td>
				</tr>--%>
            </table>
        </div>
        <script type="text/javascript">
            var urlstr = location.href.split("?")[1];
            urlstr = urlstr.split("process_status=")[1]
            var seo = document.getElementById("divSearch");
            if (urlstr == "Running")
                seo.style.visibility = "hidden";
        </script>
        <div>
            <input runat="server" type="hidden" id="whereCondition" />
            <HB:CommandInput ID="CommandInputReceiver" runat="server" OnClientCommandInput="onCommandInput">
            </HB:CommandInput>
            <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
        </div>
    </div>
    </form>
</body>
</html>
