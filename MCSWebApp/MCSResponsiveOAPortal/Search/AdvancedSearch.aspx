<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvancedSearch.aspx.cs"
    Inherits="MCSResponsiveOAPortal.AdvancedSearch" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 高级搜索" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <title>流程中心—高级搜索</title>
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
    <link rel="stylesheet" href="../../Responsive/portal/css/workflow.css" type="text/css" />
    <style type="text/css">
        .width-100
        {
            width: 80px;
        }
    </style>
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
                    <div class="well">
                        <form id="searchForm" action="" method="get" role="form" class="form-horizontal">
                        <input type="hidden" runat="server" id="sfSearch" value="1" />
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 input-group">
                                    <label for="sfDept" class="control-label input-group-addon width-100">
                                        申请部门</label>
                                    <input id="sfDept" runat="server" type="text" class="form-control" placeholder="请输入部门" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 input-group">
                                    <label for="sfSubject" class="control-label  input-group-addon width-100">
                                        表单标题</label>
                                    <input id="sfSubject" runat="server" type="text" class="form-control" placeholder="请输入标题" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 input-group">
                                    <label for="sfStart" class="control-label input-group-addon width-100">
                                        起草开始</label>
                                    <res:DateTimePicker runat="server" ID="sfStart" CssClass="" AsFormControl="True" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 input-group">
                                    <label for="sfEnd" class="control-label input-group-addon width-100">
                                        起草结束</label>
                                    <res:DateTimePicker runat="server" ID="sfEnd" CssClass="" AsFormControl="True" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3 input-group">
                                    <label for="sfApplicant" class="control-label  input-group-addon width-100">
                                        申请人</label>
                                    <input id="sfApplicant" runat="server" type="text" class="form-control" placeholder="请输入申请人" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-12 col-lg-6 input-group">
                                    <label for="sfAppName" class="control-label  input-group-addon width-100">
                                        表单类型</label>
                                    <div class="row">
                                        <div class="input-group col-xs-12 col-sm-6 col-md-6">
                                            <select id="sfAppName" runat="server" class="form-control">
                                            </select>
                                        </div>
                                        <div class="input-group col-xs-12 col-sm-6 col-md-6">
                                            <select class="form-control" id="sfProgramName" runat="server">
                                                <option value="">全部</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-lg-3 input-group">
                                    <button type="submit" class="btn btn-default">
                                        <i class="glyphicon glyphicon-search"></i>&nbsp;搜索
                                    </button>
                                </div>
                            </div>
                        </div>
                        </form>
                    </div>
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <form runat="server" id="form1">
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <res:DeluxeGrid runat="server" ID="gridMain" DataSourceID="ObjectDataSourceFormQuery"
                        PageCodeShowMode="Auto" ShowExportControl="False">
                        <Columns>
                            <%-- <asp:BoundField DataField="PROJECT_NAME" SortExpression="PROJECT_NAME" HeaderText="服务名称"
                                HeaderStyle-ForeColor="Black" Visible="false">
                                <HeaderStyle ForeColor="Black"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                            </asp:BoundField>--%>
                            <asp:TemplateField HeaderText="标题" SortExpression="SUBJECT" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <%
                                        if (this.gridMain.ExportingDeluxeGrid)
                                        { %>
                                    <%# Server.HtmlEncode(Eval("SUBJECT").ToString()) %>
                                    <%}
                                        else
                                        { %>
                                    <a href="javascript:void(0);" data-taskurl='<%# HttpUtility.HtmlAttributeEncode(this.GetTaskUrl(Container.DataItem)) %>'
                                        data-taskid='<%# HttpUtility.HtmlAttributeEncode(Eval("RESOURCE_ID").ToString()) %>'
                                        data-link="opentask">
                                        <%# Server.HtmlEncode(Eval("SUBJECT").ToString())%></a>
                                    <%} %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField SortExpression="CREATOR_NAME" HeaderText="拟单人">
                                <ItemTemplate>
                                    <%--<hbex:userpresence id="userPresence" runat="server" userid='<%# Eval("DraftUserID") %>'
                                            userdisplayname='<%# Eval("DraftUserName") %>' />--%>
                                    <%# Eval("CREATOR_NAME")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DRAFT_DEPARTMENT_NAME" HeaderText="单位" SortExpression="DRAFT_DEPARTMENT_NAME"
                                HeaderStyle-CssClass="hidden-sm hidden-xs" ItemStyle-CssClass="hidden-sm hidden-xs">
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="文件类别" HeaderStyle-CssClass="hidden-md hidden-sm hidden-xs"
                                ItemStyle-CssClass="hidden-md hidden-sm hidden-xs">
                                <ItemTemplate>
                                    <%#Server.HtmlEncode(Eval("APPLICATION_NAME").ToString())%>/<%#Server.HtmlEncode(Eval("PROGRAM_NAME_MCS").ToString())%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CREATE_TIME" SortExpression="CREATE_TIME" HeaderText="接收时间"
                                DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-CssClass="hidden-sm hidden-xs"
                                ItemStyle-CssClass="hidden-sm hidden-xs"></asp:BoundField>
                            <asp:TemplateField HeaderText="状态" SortExpression="STATUS">
                                <ItemTemplate>
                                    <a href="javascript:void(0);" data-processid='<%#Eval("PROCESS_ID") %>' data-resourceid='<%#Eval("RESOURCE_ID") %>'
                                        data-linktype="processstatus"><s class='<%# MCSResponsiveOAPortal.Util.GetStatusIconClass(int.Parse(Eval("Status").ToString())) %>'>
                                        </s>
                                        <%# MCSResponsiveOAPortal.Util.GetStatusDescription(int.Parse(Eval("STATUS").ToString()))%>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </res:DeluxeGrid>
                    <asp:ObjectDataSource ID="ObjectDataSourceFormQuery" runat="server" EnablePaging="True"
                        SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
                        TypeName="MCSResponsiveOAPortal.DataSources.AdvancedSearchDataSource" EnableViewState="False"
                        OnSelecting="ObjectDataSourceFormQuery_Selecting" OnSelected="ObjectDataSourceFormQuery_Selected">
                        <SelectParameters>
                            <asp:Parameter Name="where" DefaultValue="1=1" Type="String" />
                            <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="../../Responsive/portal/js/SidebarMenu.js"></script>
    <script src="../Scripts/AffairNotify.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#gridMain").on("click", "a[data-link=opentask]", function (e) {
                window.open($(this).attr("data-taskurl"));
                var id = $(this).attr("data-taskid");
                MCSResponsiveOAPortal.PortalServices.UpdateTaskReadTime(id);

                $(this).parent().parent().removeClass("unread");
            }).on("click", "button[data-link=openlog]", function () {
                var rid = $(this).attr("data-resourceid");
                var pid = $(this).attr("data-processid");
                window.open("TraceLog.aspx?resourceID=" + rid + "&processID=" + pid);
            }).on("click", "a[data-linktype=processstatus]", function () {
                var t = $(this);
                var pid = t.attr("data-processid");
                var rid = t.attr("data-resourceid");
                window.open("/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx?resourceID=" + rid + "&processID=" + pid);
            });

            $(function () {
                //高级查询中的应用和模块

                $("#sfAppName").on("change", function () {
                    var pg = document.getElementById("sfProgramName");
                    pg.innerHTML = '';
                    var option = document.createElement("option");
                    option.selected = true;
                    option.text = "全部";
                    option.value = "";
                    pg.add(option);

                    if (this.value) {
                        MCSResponsiveOAPortal.PortalServices.GetProgramsInApplication(this.value, function (data) {
                            for (var i = 0; i < data.length; i++) {
                                var option = document.createElement("option");
                                option.text = data[i].Name;
                                option.value = data[i].CodeName;
                                pg.add(option);
                            }

                        }, function (err) {

                        });
                    }

                });
            });
        });
    
    </script>
</body>
</html>
