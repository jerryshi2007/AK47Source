<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnCompletedTaskList.aspx.cs"
    Inherits="MCS.OA.Portal.TaskList.UnCompletedTaskList" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HBEX" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<%@ Register Assembly="MCSOAPortal" TagPrefix="Portal" Namespace="MCS.OA.Portal.Common" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>待办列表</title>
    <link href="../css.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../JavaScript/Grid.js"></script>
    <script type="text/javascript" src="../JavaScript/taskLink.js"></script>
    <script type="text/javascript" src="../JavaScript/replaceAssignee.js"></script>
    <script type="text/javascript">
        //                function onAdvancedSearchClick(mode) {
        //                    event.returnValue = false;
        //                    var sFeature = "dialogHeight: 460px; dialogWidth: 460px; edge: Raised; center: Yes; help: No; status: No;scroll: No;";
        //                    var arg = new Object();

        //                    setControlValuesToObject(arg, "DDLFormCategory", "subjectInput", "beginTimeInput", "endTimeInput", "emergencySelector"
        //            	, "TextBoxPurpose", "DeluxeCalendarExpireTimeBegin"
        //            	, "DeluxeCalendarExpireTimeEnd", "FromPerson");
        //                    var result = window.showModalDialog("UnCompletedTaskAdvancedSearch.aspx?mode=" + mode, arg, sFeature);

        //                    if (result) {
        //                        setObjectToControlValues(result, "DDLFormCategory", "subjectInput", "beginTimeInput", "endTimeInput", "emergencySelector"
        //            			, "TextBoxPurpose", "DeluxeCalendarExpireTimeBegin"
        //            			, "DeluxeCalendarExpireTimeEnd", "FromPerson");

        //                        //反序列化为人员信息数组
        //                        var fromPersonData = Sys.Serialization.JavaScriptSerializer.deserialize(result["FromPerson"]);

        //                        var personIDString = "";
        //                        var orgIDString = "";

        //                        //将人员信息数组中人员ID取出，用逗号分割，准备构造IN查询
        //                        //将部门的ID取出，准备在服务端转换为部门中的人员ID
        //                        if (fromPersonData) {
        //                            for (i = 0; i < fromPersonData.length; i++) {
        //                                if (fromPersonData[i].objectType == 1) {
        //                                    if (orgIDString != "") {
        //                                        orgIDString += ",";
        //                                    }
        //                                    orgIDString += fromPersonData[i].id;
        //                                }
        //                                else {
        //                                    if (personIDString != "") {
        //                                        personIDString += ",";
        //                                    }
        //                                    personIDString += "'";
        //                                    personIDString += fromPersonData[i].id;
        //                                    personIDString += "'";
        //                                }
        //                            }
        //                        }

        //                        //将ID串暂存在页面	    	
        //                        $get("PersonID")["value"] = personIDString;
        //                        $get("OrgID")["value"] = orgIDString;

        //                        $get("ButtonAdvanced").click();

        //                    }
        //                }
        //按回车时点击查询按钮
        function document.onkeydown() {
            if (event.keyCode == 13) {
                document.getElementById("GridViewTask").focus();
                document.getElementById("GridViewTask").click();
            }
        }
        function onLogclick(resourceID, processID) {
            var strLink = "../../OACommonPages/UserOperationLog/UserOperationLogView.aspx?resourceID=" + resourceID + "&processID=" + processID;
            window.showModalDialog(strLink, "", "dialogHeight: 530px; dialogWidth: 800px; resizable:yes; edge: Raised; center: Yes; help: No; status: No;scroll: No;");
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
            <input type="hidden" id="emergencySelector" runat="server" value="-1" />
            <input type="hidden" id="TextBoxPurpose" runat="server" />
            <input type="hidden" id="PersonID" runat="server" />
            <input type="hidden" id="OrgID" runat="server" />
            <input type="hidden" id="FromPerson" runat="server" />
            <input type="hidden" id="DDLFormCategory" runat="server" />
            <!--存储原始待办人的控件-->
            <input type="hidden" id="hiddenOriginalUserID" runat="server" />
        </div>
        <div>
            <HB:DataBindingControl runat="server" ID="bindingControl">
                <ItemBindings>
                    <HB:DataBindingItem ControlID="DDLFormCategory" ControlPropertyName="Value" DataPropertyName="ApplicationName"
                        Direction="ControlToData" />
                    <HB:DataBindingItem ControlID="TextBoxPurpose" ControlPropertyName="Value" DataPropertyName="Purpose"
                        Direction="ControlToData" />
                    <HB:DataBindingItem ControlID="PersonID" ControlPropertyName="Value" DataPropertyName="SourceID"
                        Direction="ControlToData" />
                </ItemBindings>
            </HB:DataBindingControl>
        </div>
        <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;
            margin-right: -12px;">
            <table style="text-indent: 1.5em; background: url(../images/h2_icon.gif) no-repeat left center;
                line-height: 30px; width: 100%;">
                <tr>
                    <td style="font-size: 11pt;">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
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
                OnExportData="GridViewTask_ExportData" GridTitle="待办列表" ShowCheckBoxes="true"
                CheckBoxPosition="Left" DataKeyNames="TaskID" OnRowDataBound="GridViewTask_RowDataBound"
                CssClass="dataList gtasks" TitleCssClass="title" Width="100%" DataSourceMaxRow="0" ExportingDeluxeGrid="False"
                TitleColor="141, 143, 149" TitleFontSize="Large">
                <HeaderStyle CssClass="headbackground" />
                <RowStyle CssClass="titem" />
                <CheckBoxTemplateItemStyle Width="5%" />
                <CheckBoxTemplateHeaderStyle Width="5%" />
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
                    <asp:TemplateField SortExpression="DRAFT_USER_NAME" HeaderText="拟单人">
                        <ItemStyle HorizontalAlign="Left" Width="10%" CssClass="bg_td1" />
                        <ItemTemplate>
                            <span style="margin-left: 16px">
                                <HBEX:UserPresence ID="userPresence" runat="server" UserID='<%# Eval("DraftUserID") %>'
                                    UserDisplayName='<%# Eval("DraftUserName") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DeliverTime" SortExpression="DELIVER_TIME" HeaderText="接收时间"
                        DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-ForeColor="Black">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ExpireTime" SortExpression="EXPIRE_TIME" HeaderText="计划完成时间"
                        Visible="false" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-ForeColor="Black">
                        <HeaderStyle ForeColor="Black"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                    </asp:BoundField>
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
        <%--   <div style="margin: 5px; visibility: hidden">
			<table style="background-color: Silver; width: 100%; text-align: left" cellspacing="1px"
				border="0" cellpadding="0">
				<tr style="background-color: White">
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
					<td style="text-align: left; background-color: #f2f8f8; height: 30px">
						&nbsp;&nbsp;
						<asp:LinkButton runat="server" ID="queryBtn" OnClick="QueryBtnClick">
							<img src="../../Images/16/search.gif" alt="查询" style="border:0;"/>查询</asp:LinkButton>
					</td>
				</tr>
				<tr style="background-color: White">
					<td style="text-align: right; background-color: #f7fbfa; height: 30px" colspan="7">
						<Portal:ApplicationNameDropDownList ID="DropDownListApplicationName" runat="server"
							Visible="false" />
						<Portal:CategoryDropDownList ID="DropDownListCategory" runat="server" Visible="False" />
						<asp:Button runat="server" ID="ButtonAdvanced" Text="高级查询" OnClick="ButtonAdvanced_Click"
							CssClass="hidden" />
						<asp:Literal ID="LiteralAdvancedSearch" runat="server" Text="<a href='#' onclick='onAdvancedSearchClick({0});'><img src='../../Images/16/find.gif' style='border:0;margin-bottom:-3px;' alt='高级查询'/>高级查询</a>"></asp:Literal>
					</td>
				</tr>
			</table>
		</div>--%>
        <div>
            <!--where查询条件-->
            <input runat="server" type="hidden" id="whereCondition" />
            <HB:CommandInput ID="CommandInputReceiver" runat="server" OnClientCommandInput="onCommandInput">
            </HB:CommandInput>
            <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
            <!--目的待办人选择控件-->
            <HB:UserSelector runat="server" ID="targetUserSelector" MultiSelect="true" DialogHeaderText="选择目的待办人"
                DialogTitle="选择目的待办人" ShowingMode="Normal" ListMask="All" />
            <!--进度条控件-->
            <HB:MultiProcessControl runat="server" ID="multiProcess" DialogTitle="正在提交修改" ControlIDToShowDialog="btnModify"
                OnClientBeforeStart="onBeforeStart" OnClientFinishedProcess="onFinished" OnExecuteStep="multiProcess_ExecuteStep" />
        </div>
        <div style="display:none">
            <!--查询按钮-->
            <asp:Button ID="btnQuery" OnClick="QueryBtnClick" runat="server" />
        </div>
    </div>
    </form>
</body>
</html>
