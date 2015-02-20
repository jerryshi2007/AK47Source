<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyEditor.aspx.cs"
    Inherits="AUCenter.Dialogs.PropertyEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>扩展属性定义</title>
    <link href="../Styles/dlg.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/pccom.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/propertyeditor.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <asp:HiddenField runat="server" ID="hfAUSchemaID" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            扩展属性定义-<span id="lblTitle" runat="server"> </span><span class="pc-timepointmark">
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <div>
            <label>
                描述<input type="text" id="txtDescription" runat="server" maxlength="255" />
            </label>
        </div>
        <div class="property-container">
            <ul class="property-list" id="pptList">
            </ul>
            <div id="panAdd" runat="server">
                <span>添加一个属性</span><input type="text" class="pc-input" id="txtAddNew" /><input type="button"
                    value="添加" class="pc-button" id="btnAddNew" /><span id="lblAddPrompt" class="pc-prompt"></span>
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" class="pcdlg-button" runat="server" id="btnSave" value="保存" /><input
                type="button" class="pcdlg-button" value="关闭" onclick="window.close();" />
        </div>
        <div class="pc-hide">
            <asp:HiddenField runat="server" ID="hfPostData" />
            <soa:SubmitButton runat="server" ID="btnSubmit" OnClick="DoPost" />
        </div>
    </div>
    </form>
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="PropertyEditor.js" type="text/javascript"></script>
</body>
</html>
