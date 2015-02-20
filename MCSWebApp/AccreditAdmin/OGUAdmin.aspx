<%@ Page CodeBehind="OGUAdmin.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="MCS.Applications.AccreditAdmin.OGUAdmin" %>
<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns:hgui>
<head>
    <title>机构人员管理系统</title>
    <link rel="Shortcut Icon" href="./images/icon/key.ico">
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312">
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="ProgId" content="VisualStudio.HTML">
    <meta name="Originator" content="Microsoft Visual Studio .NET 7.1">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="./css/Input.css" type="text/css" rel="stylesheet">

    <script type="text/javascript" language="javascript" src="./oguScript/validate.js"></script>

    <script type="text/javascript" language="javascript" src="./oguScript/xmlHttp.js"></script>

    <script type="text/javascript" language="javascript" src="./oguScript/uiScript.js"></script>

    <script type="text/javascript" language="javascript" src="./oguScript/dbGrid.js"></script>

    <script type="text/javascript" language="javascript" src="./selfScript/individuality.js"></script>

    <script type="text/javascript" language="javascript" src="./selfScript/accreditAdmin.js"></script>

    <script type="text/javascript" language="javascript" src="./selfScript/organizeTree.js"></script>

    <script type="text/javascript" language="javascript" src="./selfScript/persist.js"></script>

    <script type="text/javascript" language="javascript" src="./selfScript/splitter.js"></script>

    <script type="text/javascript" language="javascript" src="./Script/ApplicationRoot.js"></script>

    <script type="text/javascript" language="javascript" src="OGUAdmin.js"></script>

</head>
<body onload="onDocumentLoad();" onunload="onDocumentUnload();" onresize="onWindowResize();">
    <img src="./images/domain.gif" style="display: none" width="16" height="16">
    <img src="./images/user.gif" style="display: none" width="16" height="16">
    <img src="./images/Organization.gif" style="display: none" width="16" height="16">
    <img src="./images/group.gif" style="display: none" width="16" height="16">
    <hgui:smenu id="menuTree" style="visibility: hidden; behavior: url(./htc/hMenu.htc);
        position: absolute" onmenuclick="menuTreeClick()" onbeforepopup="menuBeforePopup()">
			刷新,,,,./images/refresh.gif,refresh;
			查找...,,,,./images/find.gif,search;
			-,0,,,,;
			数据导出...,,,,./images/export.gif,export;
			数据导入...,,,,./images/import.gif,import;
			-,0,,,,;
			属性...,,,,./images/property.gif,property
		</hgui:smenu>
    <!--
			显示组织机构及人员列表...,,,,,ouChart;
			同步人员的部门属性,,,,,syncDepartment;
			-->
    <xml id="ADSearchConfig" src="./xml/ADSearchConfig.xml"></xml>
    <input type="hidden" id="currentUserName" name="currentUserName" title="用于记录系统中当前登录用户的身份"
        runat="server">
    <input type="hidden" id="paramValue" title="用于向ｌｉｓｔ页面发送数据展示请求">
    <input type="hidden" id="syncData" onpropertychange="onSyncDataChange();" title="用于ｌｉｓｔ界面发生改变性操作时候接受数据并同时响应相关操作使用">
    <table style="width: 100%; height: 100%" cellspacing="0" id="BodyTable">
        <tr style="height: 64px">
            <td colspan="3" style="border-bottom: black 1px solid">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <font face="SimSun" style="font-weight: bold; font-size: 18pt">
                                <%--<img class="shadowAlpha" align="absMiddle" src="./images/32/customs.gif" width="48"
                                    height="48">--%>
                                机构人员管理系统</font>
                        </td>
                        <td style="width: 60px">
                            <CCPC:SignInLogoControl runat="server" ID="SignInLogo" AutoRedirect="true"></CCPC:SignInLogoControl>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width: 180px">
                <hgui:htree id="tv" style="behavior: url(./htc/hTree.htc); overflow: auto; width: 100%;
                    height: 100%" onnodeselected="tvNodeSelected();" onnodeexpand="tvNodeExpand();"
                    onnoderightclick="tvNodeRightClick();">
				</hgui:htree>
            </td>
            <td id="splitterContainer">
            </td>
            <td id="innerDocTD">
                <iframe id='frmContainer' style="width: 100%; height: 100%" frameborder='0' scrolling='auto'>
                </iframe>
            </td>
        </tr>
    </table>
</body>
</html>
