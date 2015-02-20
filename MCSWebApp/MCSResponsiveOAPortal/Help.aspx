<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="MCSResponsiveOAPortal.Help" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 帮助" />
    <meta name="msapplication-starturl" content="Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="favicon.ico">
    <link rel="icon" type="image/ico" href="favicon.ico">
    <title>流程中心—帮助</title>
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
                <div class="panel-body">
                    <op:LogOutLink ID="logOutLink" runat="server" />
                    <div>
                        <form name="form1" runat="server">
                        <!-- 这个表单没什么实际用途，仅放ScriptManger -->
                        <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                            <Services>
                                <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                            </Services>
                        </asp:ScriptManager>
                        <div class="list-group">
                            <a href="/SinoOceanWebApp/SinoOceanOAPortal/frames/流程中心操作手册.pdf" target="_blank"
                                class="list-group-item">帮助手册</a><a href="/SinoOceanWebApp/Diagnostics/clientcheck/check.aspx"
                                    target="_blank" class="list-group-item">客户端检测</a>
                        </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="../Responsive/portal/js/SidebarMenu.js"></script>
    <script src="Scripts/AffairNotify.js" type="text/javascript"></script>
    <script type="text/javascript">
        var navHeight = $('.navbar').outerHeight(true) + 10
        $(document.body).scrollspy({
            target: '.bs-sidebar',
            offset: navHeight
        })
        $(window).on('load', function () {
            $(document.body).scrollspy('refresh')
        }) 
    </script>
</body>
</html>
