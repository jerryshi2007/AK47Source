<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemAdmin.aspx.cs" Inherits="MCSResponsiveOAPortal.SystemAdmin" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <title>流程中心—系统管理</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 系统管理" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
    <style type="text/css">
        
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
                <div class="">
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <!-- ContentGoes Here -->
                    <form runat="server" id="form1">
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <div class="nav-panel" id="navPanel" runat="server" data-buildin-function="categorylink"
                        data-post-pattern="SystemAdmin.aspx?path=">
                        <div class="nav-panel-toolbar">
                            <button type="button" data-toggle="upward" class="btn btn-primary hidden">
                                <i class="glyphicon glyphicon-chevron-up"></i>返回上层</button>
                            <small data-role="caption" class="text-muted"></small>
                        </div>
                        <div data-category-link-role="buttons">
                        </div>
                    </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="../../Responsive/portal/js/SidebarMenu.js"></script>
    <script src="../Scripts/AffairNotify.js" type="text/javascript"></script>
    <script src="../Scripts/CategoryLink.js" type="text/javascript"></script>
</body>
</html>
