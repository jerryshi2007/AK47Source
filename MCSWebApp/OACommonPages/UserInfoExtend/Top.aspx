<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top.aspx.cs" Inherits="MCS.OA.CommonPages.UserInfoExtend.Top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        #logo
        {
            background: url(../images/bg_logo.jpg) top repeat-x;
            float: left;
            overflow:hidden;
            height: 127px;
            width: 100%;
        }
        #logo_left
        {
            background: url(../images/logo_left.jpg) top repeat-x;
            float: left;
            overflow: hidden;
            height: 127px;
            width: 480px;
        }
        #logo_right
        {
            background: url(../images/logo_right.jpg) top repeat-x;
            float: right;
            overflow: hidden;
            height: 127px;
            width: 524px;
        }
        #nav
        {
            background: url(../../MCSOAPortal/img/bg_menu.jpg);
            color: #FFF;
            height: 40px;
            font-weight: 700;
            line-height: 30px;
            overflow: hidden;
        }
        #nav_left
        {
            cursor: hand;
            background: url(../../MCSOAPortal/img/head.gif) top left no-repeat;
            display: inline;
            float: left;
            margin-left: 20px;
            padding-left: 30px;
            width: 400px;
        }
        #cbody
        {
            margin: 0 auto;
            overflow: hidden;
            width: 100%;
        }
        #header
        {
            /*width: 1004px;*/
            width: 100%;
            text-align: center;
        }
    </style>
</head>
<body>
    <div id="cbody">
        <div id="header">
            <div id="logo">
                <div id="logo_left" >
                </div>
                <div id="logo_right">
                </div>
            </div>
            <div id="nav" style="width: 100%; height: 40px;">
                <div id="nav_left" style="text-align: left">
                    <a onclick="onUserMenuClick();" style="vertical-align: middle; color: White">
                        <asp:Label runat="server" ID="lblUserName" />
                        <asp:Label runat="server" ID="lblDateTime" />
                    </a>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
