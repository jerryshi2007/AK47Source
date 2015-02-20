<%@ Page Language="C#" AutoEventWireup="true" Codebehind="CookieCacheDependencyTest.aspx.cs"
    Inherits="MCS.Web.Passport.TestPages.CookieCacheDependencyTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="serverForm" runat="server">
        <div>
            <div>
                <asp:Button runat="server" ID="setCacheDataBtn" Text="Set Cache Data" OnClick="setCacheDataBtn_Click" />
                <asp:Button runat="server" ID="showCacheDataBtn" Text="Show Cache Data" OnClick="showCacheDataBtn_Click" />
                <asp:Button runat="server" ID="clearCookieBtn" Text="Clear Cookie" OnClick="clearCookieBtn_Click" />
            </div>
            <div>
                <span>Cache Data: </span>
                <asp:Label runat="server" ID="cacheData" />
            </div>
        </div>
    </form>
</body>
</html>
