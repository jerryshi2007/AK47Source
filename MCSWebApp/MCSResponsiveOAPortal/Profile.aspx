<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="MCSResponsiveOAPortal.Profile" %>

<!DOCTYPE html>
<html class="full-height">
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 个人设置" />
    <meta name="msapplication-starturl" content="Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="favicon.ico">
    <link rel="icon" type="image/ico" href="favicon.ico">
    <title>流程中心—个人设置</title>
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
    <style type="text/css">
        .full-height
        {
            height: 100%;
        }
    </style>
</head>
<body class="">
    <res:DeluxePlaceHolder runat="server" ID="bannerHolder" LoadingMode="AsText" TemplatePath="~/../Responsive/Templates/Banner.htm" />
    <res:DeluxePlaceHolder runat="server" ID="SearchBar" LoadingMode="AsText" TemplatePath="~/../Responsive/Templates/SearchBar.htm" />
    <!-- 导航条结束 -->
    <!-- 主体内容开始 -->
    <div class="main clearfix ">
        <div class="sidebar">
            <op:ResSiteMenu ID="menu" runat="server">
            </op:ResSiteMenu>
            <div class="cutover ">
                <div class="menu-minifier text-center ">
                    <i class="glyphicon glyphicon-transfer text-muted"></i>
                </div>
            </div>
        </div>
        <div id="main" class="main-content cutover-visibility main-width ">
            <div class="panel panel-default full-height">
                <div class="panel-heading head-height">
                    <op:ResSiteMapPath runat="server" ID="navPath" SiteMapProvider="BreadcrumbSiteMapProvider" />
                </div>
                <div class="">
                    <op:LogOutLink ID="logOutLink" runat="server" />
                    <form runat="server" id="form1" class="">
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <!-- ContentGoes Here -->
                    <div class="full-height" style="position: relative; min-height: 500px;">
                        <iframe src="../MCSOAPortal/UserPanel/UserSettingsManager.aspx" class="" style="width: 100%;
                            height: 100%; position: absolute; border: 0"></iframe>
                    </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="../Responsive/portal/js/SidebarMenu.js"></script>
    <script src="Scripts/AffairNotify.js" type="text/javascript"></script>
</body>
</html>
