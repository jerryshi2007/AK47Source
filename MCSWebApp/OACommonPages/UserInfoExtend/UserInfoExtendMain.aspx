<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserInfoExtendMain.aspx.cs"
    Inherits="MCS.OA.CommonPages.UserInfoExtend.UserInfoExtendMain" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>远洋地产通讯录</title>
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
</head>
<frameset id='frameset1' rows="170,*">
    <frame name="frameTop" src="top.aspx"  scrolling="no" noresize frameborder="0">
	<frameset id='frameset6' cols="220,*" >
		<frame name="frameLeft" src="Left.aspx"  scrolling="no" frameborder="0">
		<frame name="SearchContent" src="SearchContent.aspx" scrolling="yes" frameborder="0">
	</frameset>
</frameset>
</html>
