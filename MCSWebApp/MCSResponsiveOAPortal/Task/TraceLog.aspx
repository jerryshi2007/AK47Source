<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraceLog.aspx.cs" Inherits="MCSResponsiveOAPortal.TraceLog" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 操作日志" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <title>流程中心—操作日志</title>
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
                <div>
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <form runat="server" id="form1">
                    <!-- ContentGoes Here -->
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <res:DeluxeGrid runat="server" ID="gridMain" DataSourceID="dsMain" ShowCheckBoxes="false"
                        DataKeyNames="ID" PageCodeShowMode="Auto" AutoGenerateColumns="False">
                        <Columns>
                            <%--<asp:BoundField DataField="PROJECT_NAME" SortExpression="PROJECT_NAME" HeaderText="服务名称" />--%>
                            <asp:BoundField DataField="Subject" HeaderText="标题" HtmlEncode="true" />
                            <asp:BoundField DataField="ApplicationName" HeaderText="表单类别" HtmlEncode="true" />
                            <asp:BoundField DataField="TopDepartment" HeaderText="操作人部门" Visible="false" HtmlEncode="true" />
                            <asp:BoundField DataField="Operator" HtmlEncode="true" HeaderText="操作人员" />
                            <asp:BoundField DataField="ActivityName" HeaderText="环节名称" HtmlEncode="true" />
                            <asp:BoundField DataField="OperationName" HeaderText="操作人" HtmlEncode="true" />
                            <asp:BoundField DataField="TargetDescription" HeaderText="待办人列表" HtmlEncode="true" />
                            <asp:BoundField DataField="OperationDateTime" HeaderText="操作时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
                                HtmlEncode="true" />
                            <asp:TemplateField HeaderText="详细">
                                <ItemTemplate>
                                    <button class="btn btn-xs btn-info" type="button" data-link="openlog" data-id='<%#Eval("ID") %>'>
                                        <i class=" glyphicon glyphicon-comment bigger-120"></i>
                                    </button>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </res:DeluxeGrid>
                    <asp:ObjectDataSource ID="dsMain" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
                        SelectMethod="Query" SortParameterName="orderBy" TypeName="MCSResponsiveOAPortal.DataSources.UserOperationLogDataSource, MCSResponsiveOAPortal"
                        EnableViewState="False" OnSelecting="dsMain_Selecting">
                        <SelectParameters>
                            <asp:Parameter Name="where" Type="String" />
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
            $("#gridMain").on("click", "button[data-link=openlog]", function () {
                var id = $(this).attr("data-id");
                window.open("/MCSWebApp/OACommonPages/UserOperationLog/LogDetail.aspx?id=" + encodeURIComponent(id));
            });

        });
    </script>
</body>
</html>
