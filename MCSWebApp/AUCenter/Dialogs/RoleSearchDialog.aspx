<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleSearchDialog.aspx.cs"
    Inherits="AUCenter.Dialogs.RoleSearchDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>角色搜索</title>
    <link href="../Styles/pccom.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/dlg.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/aumain.css" rel="stylesheet" type="text/css" />
</head>
<body class="au-full pcdlg">
    <form id="form1" runat="server">
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            角色搜索
        </h1>
    </div>
    <div class="pcdlg-content">
        <asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true">
            <Services>
            </Services>
        </asp:ScriptManager>
        <div>
            <div class="au-progress" id="progressbar">
            </div>
            <div style="height: 5px;">
                <div id="prompt" class="au-prompt">
                    正在载入数据...
                </div>
            </div>
            <ul id="navbar" class="pc-nav-path">
                <li class="pc-nav-path-content" id="pickApp"><a href="javascript:void(0)" id="A1">选择应用</a></li>
                <li class="pc-nav-path-content"><em>|</em></li>
                <li class="pc-nav-path-content"><em id="appLabel"></em></li>
                <li style="float: right; height: 24px; line-height: 25px;" class="pc-nav-path-content">
                    过滤关键词
                    <input type="text" id="filter" value="abc" style="margin-top: 0" placeholder="请输入过滤关键词"
                        maxlength="25" />
                </li>
            </ul>
            <ul id="list" class="au-selectable-list">
            </ul>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" value="确定" class="pcdlg-button" id="btnOk" />
            <input type="button" value="取消" class="pcdlg-button" onclick="window.close();" />
        </div>
    </div>
    </form>
    <script src="RoleSearchDialog.js" type="text/javascript"></script>
</body>
</html>
