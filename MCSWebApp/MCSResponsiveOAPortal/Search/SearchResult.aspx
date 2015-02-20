<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResult.aspx.cs" Inherits="MCSResponsiveOAPortal.SearchResult" %>

<!DOCTYPE html>
<html>
<head id="head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="application-name" content="流程中心" />
    <meta name="msapplication-tooltip" content="流程中心 - 搜索结果" />
    <meta name="msapplication-starturl" content="../Default.aspx" />

    <link rel="shortcut icon" type="image/x-icon" href="../favicon.ico">
    <link rel="icon" type="image/ico" href="../favicon.ico">
    <title>流程中心—搜索结果</title>
    <!--[if lt IE 9]>
		<script src="http://cdn.bootcss.com/html5shiv/3.7.0/html5shiv.min.js"></script>
		<script src="http://cdn.bootcss.com/respond.js/1.3.0/respond.min.js"></script>
	<![endif]-->
    <style type="text/css">
        .highlight
        {
            color: #f00;
            font-weight: bold;
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
                <div class="panel-body">
                    <op:LogOutLink runat="server" ID="logOutLink" />
                    <form runat="server" id="form1">
                    <!-- ContentGoes Here -->
                    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
                        <Services>
                            <asp:ServiceReference Path="~/Services/PortalServices.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <h3>
                        应用中的查询结果列表<small>
                            <asp:Label Text="" runat="server" CssClass="small" ID="txtSummary" /></small>
                    </h3>
                    <res:DeluxeDataPager runat="server" ID="pager1" QueryStringField="page" PageSize="20"
                        PagedControlID="listMain">
                        <Fields>
                            <res:DeluxeNumericPagerField ButtonCount="5" />
                        </Fields>
                    </res:DeluxeDataPager>
                    <asp:ListView runat="server" ID="listMain" DataSourceID="dsMain" GroupItemCount="1"
                        EnableViewState="false">
                        <LayoutTemplate>
                            <div class="row">
                                <div runat="server" id="groupPlaceholder">
                                </div>
                            </div>
                        </LayoutTemplate>
                        <GroupTemplate>
                            <div runat="server" id="itemPlaceholder">
                            </div>
                        </GroupTemplate>
                        <ItemTemplate>
                            <div runat="server" class="col-xs-12 col-md-6 col-lg-4">
                                <h3>
                                    <a href='<%# GetNormalizedUrl((string)Eval("ApplicationName"), (string)Eval("ProgramName"), (string)Eval("Url")) %>'
                                        target="_blank" class="search-result">
                                        <%# HttpUtility.HtmlEncode( Eval("Subject").ToString()) %></a>
                                </h3>
                                <div class="search-result">
                                    <%# HttpUtility.HtmlEncode(Eval("Content").ToString())%>
                                </div>
                                <div>
                                    <%#Eval("CreateTime", "{0:yyyy年MM月dd日}") %>
                                </div>
                                <div>
                                    <%--  状态<a href="javascript:void(0);" data-processid='<%#Eval("ProcessID") %>' data-resourceid='<%#Eval("ResourceID") %>'
                                        data-linktype="processstatus"><s class='<%# MCSResponsiveOAPortal.Util.GetStatusIconClass(int.Parse(Eval("Status").ToString())) %>'>
                                        </s>
                                        <%# MCSResponsiveOAPortal.Util.GetStatusDescription(int.Parse(Eval("STATUS").ToString()))%>
                                    </a>--%>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                    <res:DeluxeDataPager runat="server" ID="pager2" QueryStringField="page" PageSize="20"
                        PagedControlID="listMain">
                        <Fields>
                            <res:DeluxeNumericPagerField ButtonCount="5" />
                        </Fields>
                    </res:DeluxeDataPager>
                    <asp:ObjectDataSource ID="dsMain" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
                        SelectMethod="Query" SortParameterName="orderBy" TypeName="MCSResponsiveOAPortal.DataSources.SearchSource, MCSResponsiveOAPortal"
                        EnableViewState="False" OnSelecting="dsMain_Selecting" OnSelected="dsMain_Selected">
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

            var txtToSearch = null;
            var search = window.location.search;
            if (search) {
                var match = search.match(/\??(query=([^&]+))/i);
                if (match && match.length >= 3) {
                    txtToSearch = decodeURIComponent(match[2].replace('+', ' '));
                    $("input[data-banneritem=search]").val(txtToSearch);
                }
            }

            if (txtToSearch) {
                var parts = txtToSearch.split(' ');
                for (var i = parts.length - 1; i >= 0; i--) {
                    parts[i] = "(" + parts[i] + ")"; // doto 考虑转义其中的字符
                }

                var reg = new RegExp("(" + parts.join("|") + ")", "igm");
                $(".search-result").each(function () {
                    var txt = $(this).text();
                    txt = txt.replace(reg, '<span class="highlight">$1</span>');

                    $(this).html(txt);

                });
            }
        });
    
    </script>
</body>
</html>
