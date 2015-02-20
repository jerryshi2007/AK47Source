<%@ Page Language="C#" AutoEventWireup="true" Codebehind="PrincipalTest.aspx.cs"
    Inherits="MCS.Web.Passport.Anonymous.PrincipalTest" %>
<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls"
    Assembly="MCS.Library.Passport" %>
    
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Principal测试</title>
    <style type="text/css">
        .captionCell { text-align: right; font-weight: bold; };
        .table { border: 1px solid gray};
    </style>
</head>
<body>
    <form id="serverForm" runat="server">
        <div>
             <CCPC:SignInLogoControl runat="server" id="SignInLogo">
            </CCPC:SignInLogoControl>
        </div>
        <div runat="server" id="principalInfo">
        
        </div>
    </form>
</body>
</html>
