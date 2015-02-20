<%@ Page Language="C#" CodeBehind="TodoList.aspx.cs" Inherits="MCSResponsiveOAPortal.TodoList" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 待办列表" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <title>流程中心-待办列表</title>
    <link rel="stylesheet" href="../../Responsive/portal/css/theme.css">
    <style type="text/css">
        .button-mode-toggle
        {
            width: 16px;
            height: 16px;
            position: relative;
            border: 1px solid #CCCCCC;
            border-radius: 50%;
            cursor: pointer;
            display: inline-block;
            text-align: center;
        }
        
        .label-range:after
        {
            position: absolute;
            display: inline;
            content: "~";
            top: 10px;
        }
        
        .width-100
        {
            width: 100px;
        }
        
        .more-only, .more-only-block
        {
            display: none;
        }
        
        .mode-more .less-only
        {
            display: none;
        }
        
        .mode-more .more-only
        {
            display: inline-block;
        }
        
        .mode-more .more-only-block
        {
            display: block;
        }
    </style>
    <%--<link rel="stylesheet" href="../Responsive/bootstrap/css/bootstrap.css">--%>
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
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
                <div class="">
                    <!-- ContentGoes Here -->
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <form runat="server" id="form1">
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <div id="searchFields" class="" runat="server">
                        <div class="form-horizontal well">
                            <div class="form-group row">
                                <div class="col-sm-12 col-lg-8 input-group">
                                    <label for="sfTitle" class="control-label input-group-addon width-100">
                                        标题</label>
                                    <asp:TextBox runat="server" ID="sfTitle" CssClass="form-control" />
                                    <span class="input-group-btn less-only">
                                        <button type="button" class="btn btn-default btn" role="button" onclick="javascript:__doPostBack('btnSearch','');">
                                            <i class="glyphicon glyphicon-search btn-main-search"></i>&nbsp;搜索
                                        </button>
                                    </span><span class="input-group-btn" id="moreModeToggler"><span class="btn btn-more">
                                        <i class="icon-angle-down button-mode-toggle less-only"></i><i class="icon-angle-up button-mode-toggle more-only">
                                        </i><span class="less-only">更多查询条件</span><span class="more-only">较少查询条件</span></span></span>
                                </div>
                            </div>
                            <div class="form-group row more-only-block">
                                <div class="col-xs-12 col-sm-12 col-md-6 input-group">
                                    <label for="sfAppName" class="control-label input-group-addon width-100">
                                        文件类别</label>
                                    <div class="row">
                                        <div class="input-group col-xs-12 col-sm-6 col-md-6">
                                            <asp:DropDownList runat="server" ID="sfAppName" CssClass="form-control" DataSourceID="dsApps"
                                                DataValueField="CodeName" DataTextField="Name" AppendDataBoundItems="true">
                                                <asp:ListItem Text="全部" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="input-group col-xs-12 col-sm-6 col-md-6">
                                            <select class="form-control" id="sfProgramName" runat="server">
                                                <option value="">全部</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-6 input-group">
                                    <label for="sfReceiveStart" class="control-label input-group-addon width-100">
                                        接收时间</label>
                                    <div class="row">
                                        <res:DateTimePicker runat="server" ID="sfReceiveStart" AsFormControl="True" CssClass="label-range col-xs-12 col-sm-6 col-md-6" />
                                        <res:DateTimePicker runat="server" ID="sfReceiveEnd" AsFormControl="True" CssClass="col-xs-12 col-sm-6 col-md-6" />
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3 input-group">
                                    <label for="sfUnit" class="control-label input-group-addon width-100">
                                        单位</label>
                                    <asp:TextBox runat="server" ID="sfUnit" CssClass="form-control" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3 input-group">
                                    <label for="sfDrafter" class="control-label input-group-addon width-100">
                                        拟单人</label>
                                    <asp:TextBox runat="server" ID="sfDrafter" CssClass="form-control" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3 input-group">
                                    <label for="sfPreviousMan" class="control-label input-group-addon width-100">
                                        前环节人</label>
                                    <asp:TextBox runat="server" ID="sfPreviousMan" CssClass="form-control" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-3 text-right">
                                    <button type="button" class="btn btn-default btn more-only" role="button" onclick="javascript:__doPostBack('btnSearchMore','');">
                                        <i class="glyphicon glyphicon-search"></i>&nbsp;搜索
                                    </button>
                                </div>
                            </div>
                            <div style="display: none">
                                <asp:LinkButton runat="server" ID="btnSearch" OnCommand="SearchClick" CommandName="Search">
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" ID="btnSearchMore" OnCommand="SearchClick" CommandName="Search"
                                    CommandArgument="More">
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <div class="table-header text-right">
                        <asp:LinkButton runat="server" CssClass="btn btn-default" ID="changeMan" OnClientClick="alert('不要点这个'); return false;"><i class="glyphicon glyphicon-pencil"></i> 修改待办人</asp:LinkButton>
                    </div>
                    <res:DeluxeGrid runat="server" ID="gridMain" DataSourceID="dsMain" ShowCheckBoxes="true"
                        CheckBoxPosition="Left" DataKeyNames="TASK_GUID" OnRowDataBound="gridMain_RowDataBound"
                        PageCodeShowMode="Auto">
                        <Columns>
                            <%-- <asp:BoundField DataField="PROJECT_NAME" SortExpression="PROJECT_NAME" HeaderText="服务名称"
                                HeaderStyle-ForeColor="Black" Visible="false">
                                <HeaderStyle ForeColor="Black"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="10%" CssClass="bg_td1" />
                            </asp:BoundField>--%>
                            <asp:TemplateField HeaderStyle-CssClass="col-icon">
                                <ItemTemplate>
                                    <s class=" icon-read"></s>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="标题" SortExpression="TASK_TITLE" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <%
                                        if (this.gridMain.ExportingDeluxeGrid)
                                        { %>
                                    <%# Server.HtmlEncode(Eval("TASK_TITLE").ToString()) %>
                                    <%}
                                        else
                                        { %>
                                    <a href="javascript:void(0);" data-taskurl='<%# HttpUtility.HtmlAttributeEncode(this.GetTaskUrl(Container.DataItem)) %>'
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
                            <asp:TemplateField SortExpression="DRAFT_USER_NAME" HeaderText="拟单人">
                                <ItemTemplate>
                                    <%--<hbex:userpresence id="userPresence" runat="server" userid='<%# Eval("DraftUserID") %>'
                                            userdisplayname='<%# Eval("DraftUserName") %>' />--%>
                                    <%# Eval("Draft_User_Name") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DELIVER_TIME" SortExpression="DELIVER_TIME" HeaderText="接收时间"
                                DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-CssClass="hidden-sm hidden-xs"
                                ItemStyle-CssClass="hidden-sm hidden-xs"></asp:BoundField>
                            <asp:BoundField DataField="EXPIRE_TIME" SortExpression="EXPIRE_TIME" HeaderText="计划完成时间"
                                Visible="false" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundField>
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
                        SelectMethod="Query" SortParameterName="orderBy" TypeName="MCSResponsiveOAPortal.DataSources.TodoListDataSource, MCSResponsiveOAPortal"
                        EnableViewState="False" OnSelecting="dsMain_Selecting" OnSelected="dsMain_Selected">
                        <SelectParameters>
                            <asp:Parameter Name="where" Type="String" />
                            <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource runat="server" ID="dsApps" SelectMethod="GetAllApplications"
                        TypeName="MCSResponsiveOAPortal.DataSources.WfApplicationDataSource" />
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
            var loadTime = new Date();
            $("#gridMain").on("click", "a[data-link=opentask]", function (e) {
                window.open($(this).attr("data-taskurl"));
                var id = $(this).attr("data-taskid");
                MCSResponsiveOAPortal.PortalServices.UpdateTaskReadTime(id);

                $(this).parent().parent().removeClass("unread");
            }).on("click", "button[data-link=openlog]", function () {
                var rid = $(this).attr("data-resourceid");
                var pid = $(this).attr("data-processid");
                window.open("TraceLog.aspx?resourceID=" + rid + "&processID=" + pid);
            });

            var refresh = function () {
                var loc = window.location.href, ind = loc.indexOf("?"), path, querys, meet = false;
                if (ind > 0) {
                    path = loc.slice(0, ind);
                    querys = loc.substring(ind + 1).split("&");
                } else {
                    path = loc;
                    querys = [];
                }

                for (var i = querys.length - 1; i >= 0; i--) {
                    if (querys[i].indexOf("refreshTime=") == 0) {
                        querys[i] = "refreshTime=" + new Date().getTime();
                        meet = true;
                    }
                }

                if (!meet) {
                    querys.push("refreshTime=" + new Date().getTime());
                }

                window.location.replace(path + "?" + querys.join("&"));
            };

            $(document).on("taskarrived.mcsroap", function (e) {
                if (Math.abs(new Date().getTime() - loadTime.getTime()) > 1000) {
                    window.setTimeout(refresh, 1000);
                }
            });

            // 顶部搜索条
            $("#moreModeToggler").click(function () {
                $("#searchFields").toggleClass("mode-more");
            });


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
