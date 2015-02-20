<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="header.aspx.cs" Inherits="MCS.OA.Portal.frames.header" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<%@ Register Assembly="MCSOAPortal" Namespace="MCS.OA.Portal" TagPrefix="PORTAL" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../css.css" rel="Stylesheet" type="text/css" />
    <style type="text/css">
        #logo
        {
            background: url(../img/bg_logo.jpg) top repeat-x;
            float: left;
            overflow: hidden;
            height: 127px;
            width: 100%;
        }
        #logo_left
        {
            background: url(../img/logo_left.jpg) top repeat-x;
            float: left;
            overflow: hidden;
            height: 127px;
            width: 480px;
        }
        #logo_right
        {
            background: url(../img/logo_right.jpg) top repeat-x;
            float: right;
            overflow: hidden;
            height: 127px;
            width: 524px;
        }
        .b22
        {
            background: url(../img/addressList.gif) no-repeat;
            border: 0;
            height: 20px;
            width: 82px;
            cursor: hand;
        }
    </style>
    <script type="text/javascript">
        function onUserSettingUrl() {
            var sFeature = "dialogWidth:640px; dialogHeight:480px;center:yes;help:no;resizable:no;scroll:no;status:no;menubar:no;";

            var returnValue = window.showModalDialog("../UserPanel/UserSettingsManager.aspx", null, sFeature);

            if (returnValue == "reload") {
                refreshContent("refresh");
            }
        }

        function onDelegate() {
          //  var sFeature = "dialogWidth:660px; dialogHeight:480px;center:yes;help:no;resizable:no;scroll:no;status:no;menubar:no;";

            window.showModalDialog("../../OACommonPages/DelegationAuthorized/OriginalDelegationEntry.htm", null, null);
        }

        function refreshContent(command) {
            var commandInput = false;
            try {
                commandInput = window.parent.document.frames["content"].window.document.getElementById("__commandInput");
            }
            catch (e) {
            }

            if (commandInput)
                commandInput.value = command;
        }

        function onUserMenuClick() {
            var o = $get("nav_left");
            var menu = $find('dMenu');
            event.returnValue = false;

            menu.get_popupChildControl().set_positionElement(o);
            menu.get_popupChildControl().set_positioningMode($HGRootNS.PositioningMode.BottomLeft);

            menu.get_popupChildControl().show();
        }

        function goHomePage() {
            //var url = "../TaskList/UnCompletedTaskList.aspx";
            var url = "http://eip";
            window.open(url, "_blank");
        }

        function goSearchList() {
            var url = "../Search/SearchList.aspx?";

            var content = document.getElementById("txtQuery").value;

            if (content.replace(/^\s+|\s+$/g, "") == "") {
                document.getElementById("txtQuery").value = "";
                alert("请输入搜索条件");
                return false;
            }

            if (content) {
                url = url + "content=" + escape(content).replace(/\+/g, "%2B");
            }

            window.open(url, "content");
        }

        function goSearch() {
            if (event.keyCode == 13) {
                goSearchList();
                event.keyCode = 0;
            }
        }
        function goAddressList() {
            window.open("/MCSWebApp/OACommonPages/UserInfoExtend/UserInfoExtendMain.aspx", "_blank");
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div id="cbody">
        <div id="header">
            <div id="logo">
                <div id="logo_left">
                </div>
                <div id="logo_right">
                </div>
            </div>
            <div id="nav" style="width: 100%; height: 30px;">
                <MCS:DeluxeMenu ID="dMenu" runat="server" HasControlSeparator="true" Orientation="Vertical"
                    imgheight="32" StaticDisplayLevels="0">
                    <Items>
                        <MCS:MenuItem Text="个人设置..." NavigateUrl="javascript:onUserSettingUrl()" ImageUrl="../images/jiaose.gif">
                        </MCS:MenuItem>
                        <MCS:MenuItem Text="授权委派..." NavigateUrl="javascript:onDelegate()">
                        </MCS:MenuItem>
                    </Items>
                </MCS:DeluxeMenu>
                <div id="nav_left" style="text-align: left">
                    <%--<a onclick="onUserMenuClick();" style="vertical-align: middle; color: White">  --%>
                    <asp:Label runat="server" ID="lblUserName" />
                    <img runat="server" id="imgDelegate" alt="当前在委托状态中" style="vertical-align: middle;
                        height: 16px; display: none" src="../img/wt.gif" />
                    <span id="spanDelegateFlag" runat="server"></span>
                    <asp:Label runat="server" ID="lblDateTime" />
                    <%--</a>--%>
                    <input class="b22" type="button" onclick="goAddressList()" />
                   <%-- <a href="/MCSWebApp/OACommonPages/UserInfoExtend/UserInfoExtendMain.aspx" target="_blank">
                        通讯录</a>--%>
                </div>
                <div id="nav_right">
                    <PORTAL:FullTextSearchApplicationDropDownList ID="FullTextSearchApplicationDropDownList"
                        runat="server" Visible="false">
                    </PORTAL:FullTextSearchApplicationDropDownList>
                    <input id="txtQuery" title="多个关键字使用空格分隔" type="text" maxlength="128" style="vertical-align: middle;
                        height: 20px" onkeypress="goSearch();" />
                    <input class="b1" type="button" onclick="goSearchList()" value="" />
                    <input class="b2" type="button" value="" onclick="window.open('/MCSWebApp/MCS.OA.Stat/Query/FormQueryList.aspx', 'content')" />
                    <input class="b3" type="button" onclick="goHomePage()" /><CCPC:SignInLogoControl
                        runat="server" ID="SignInLogo" ReturnUrl="~/Default.aspx" AutoRedirect="True"
                        Target="_top" SignOutImage="~/images/logout.gif" CssClass="b4" Visible="True">
                    </CCPC:SignInLogoControl>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
