<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompletedList.aspx.cs"
    Inherits="MCSResponsiveOAPortal.CompletedList" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 已办结" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <title>流程中心—已办结</title>
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
    <link rel="stylesheet" href="../../Responsive/portal/css/workflow.css" type="text/css" />
</head>
<body>
    <res:DeluxePlaceHolder runat="server" ID="bannerHolder" LoadingMode="AsText" TemplatePath="~/../Responsive/Templates/Banner.htm" />
    <res:DeluxePlaceHolder runat="server" ID="SearchBar" LoadingMode="AsText" TemplatePath="~/../Responsive/Templates/SearchBar.htm" />
    <!-- 导航条结束 -->
    <!-- 主体内容开始 -->
    <div class="main clearfix">
        <div class="sidebar">
            <op:ResSiteMenu ID="menu" runat="server">
            </op:ResSiteMenu>
            <div class="cutover ">
                <div class="menu-minifier text-center ">
                    <i class="glyphicon glyphicon-transfer text-muted"></i>
                </div>
            </div>
        </div>
        <div id="main" class="main-content cutover-visibility main-width">
            <div class="panel panel-default ">
                <div class="panel-heading head-height">
                    <op:ResSiteMapPath runat="server" ID="navPath" SiteMapProvider="BreadcrumbSiteMapProvider" />
                </div>
                <div>
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <div id="divSearch" class="well">
                        <form id="searchForm" action="" method="get" role="form" class="form-horizontal">
                        <div class="form-group">
                            <div class="col-lg-3 col-sm-6 input-group">
                                <label for="sfDept" class="control-label input-group-addon">
                                    申请单位</label>
                                <input id="sfDept" runat="server" type="text" class="form-control" placeholder="请输入部门" />
                            </div>
                            <div class="col-lg-3 col-md-6 input-group">
                                <label for="sfSubject" class="control-label  input-group-addon">
                                    表单标题</label>
                                <input id="sfSubject" runat="server" type="text" class="form-control" placeholder="请输入标题" />
                            </div>
                            <div class="col-lg-3 col-md-6 input-group">
                                <label for="sfStart" class="control-label input-group-addon">
                                    起始时间</label>
                                <res:DateTimePicker runat="server" ID="sfStart" CssClass="" AsFormControl="True" />
                            </div>
                            <div class="col-lg-3 col-md-6 input-group">
                                <label for="sfEnd" class="control-label input-group-addon">
                                    结束时间</label>
                                <res:DateTimePicker runat="server" ID="sfEnd" CssClass="" AsFormControl="True" />
                            </div>
                        </div>
                        <div class="from-group">
                            <button type="submit" class="btn btn-default">
                                <i class="glyphicon glyphicon-search"></i>搜索
                            </button>
                        </div>
                        </form>
                    </div>
                    <form runat="server" id="form1">
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <res:DeluxeGrid runat="server" ID="gridMain" DataSourceID="dsMain" ShowCheckBoxes="true"
                        CheckBoxPosition="Left" DataKeyNames="TASK_GUID" OnRowDataBound="gridMain_RowDataBound"
                        PageCodeShowMode="Auto" AutoGenerateColumns="False" EnableViewState="false" EnablePersistedSelection="true">
                        <Columns>
                            <%--<asp:BoundField DataField="PROJECT_NAME" SortExpression="PROJECT_NAME" HeaderText="服务名称" />--%>
                            <asp:TemplateField HeaderText="标题" SortExpression="TASK_TITLE" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <%
                                        if (this.gridMain.ExportingDeluxeGrid)
                                        { %>
                                    <%# Server.HtmlEncode(Eval("TASK_TITLE").ToString()) %>
                                    <%}
                                        else
                                        { %>
                                    <s class="glyphicon "></s><a href="javascript:void(0);" data-taskurl='<%# HttpUtility.HtmlAttributeEncode(this.GetTaskUrl(Container.DataItem)) %>'
                                        data-taskid='<%# HttpUtility.HtmlAttributeEncode(Eval("TASK_GUID").ToString()) %>'
                                        data-link="opentask">
                                        <%# Server.HtmlEncode(Eval("TASK_TITLE").ToString()) %></a>
                                    <%} %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="文件类别" HeaderStyle-CssClass="hidden-md hidden-sm hidden-xs"
                                ItemStyle-CssClass="hidden-md hidden-sm hidden-xs">
                                <ItemTemplate>
                                    <%#Server.HtmlEncode(Eval("APPLICATION_NAME").ToString())%>/<%#Server.HtmlEncode(Eval("PROGRAM_NAME").ToString())%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DRAFT_DEPARTMENT_NAME" HeaderText="单位" SortExpression="DRAFT_DEPARTMENT_NAME"
                                HeaderStyle-CssClass="hidden-sm hidden-xs" ItemStyle-CssClass="hidden-sm hidden-xs">
                            </asp:BoundField>
                            <asp:BoundField DataField="COMPLETED_TIME" HtmlEncode="False" SortExpression="COMPLETED_TIME"
                                HeaderText="办理时间" Visible="false" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                            <asp:TemplateField SortExpression="DRAFT_USER_NAME" HeaderText="拟单人">
                                <ItemTemplate>
                                    <%--<hbex:userpresence id="userPresence" runat="server" userid='<%# Eval("DraftUserID") %>'
                                            userdisplayname='<%# Eval("DraftUserName") %>' />--%>
                                    <%# Eval("Draft_User_Name") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="状态" SortExpression="STATUS">
                                <ItemTemplate>
                                    <a href="javascript:void(0);" data-processid='<%#Eval("PROCESS_ID") %>' data-resourceid='<%#Eval("RESOURCE_ID") %>'
                                        data-linktype="processstatus"><s class='<%# MCSResponsiveOAPortal.Util.GetStatusIconClass((Eval("PStatus").ToString())) %>'>
                                        </s>
                                        <%# MCSResponsiveOAPortal.Util.GetStatusDescription((Eval("PSTATUS").ToString()))%>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="详细">
                                <ItemTemplate>
                                    <div class="btn-group">
                                        <button class="btn btn-xs btn-info" type="button" data-link="openlog" data-resourceid='<%#Eval("RESOURCE_ID") %>'
                                            data-processid='<%#Eval("PROCESS_ID") %>'>
                                            <i class=" glyphicon glyphicon-comment bigger-120"></i>
                                        </button>
                                    </div>
                                    <%--  <a href="#" onclick="onLogclick('<%# Eval("RESOURCE_ID")%>','<%# Eval("PROCESS_ID")%>')">
                                        查看</a>--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </res:DeluxeGrid>
                    <asp:ObjectDataSource ID="dsMain" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
                        SelectMethod="Query" SortParameterName="orderBy" TypeName="MCSResponsiveOAPortal.DataSources.ActivatedListDataSource, MCSResponsiveOAPortal"
                        EnableViewState="False" OnSelecting="dsMain_Selecting" OnSelected="dsMain_Selected">
                        <SelectParameters>
                            <asp:Parameter Name="where" Type="String" />
                            <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <res:CommandInput ID="CommandInputReceiver" runat="server" OnClientCommandInput="onCommandInput" />
                    <asp:LinkButton Text="" ID="btnRefresh" runat="server" OnClick="RefreshClick" />
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="../../Responsive/portal/js/SidebarMenu.js"></script>
    <script src="../Scripts/AffairNotify.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#gridMain").on("click", "a[data-linktype=processstatus]", function () {
                var t = $(this);
                var pid = t.attr("data-processid");
                var rid = t.attr("data-resourceid");
                window.open("/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx?resourceID=" + rid + "&processID=" + pid);
            }).on("click", "button[data-link=openlog]", function () {
                var t = $(this);
                var rid = t.attr("data-resourceid");
                var pid = t.attr("data-processid");
                window.open("TraceLog.aspx?resourceID=" + rid + "&processID=" + pid);
            });
        });

        function onCommandInput(commandInputControl, e) {
            switch (e.commandValue) {
                case "refreshUserTasks":
                case "refresh":
                    e.stopCommand = true; //设置后，不再执行默认的处理
                    document.getElementById("btnRefresh").click();
                    break;
            }
        }
    
    </script>
</body>
</html>
