<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editor.aspx.cs" Inherits="AUCenter.Dialogs.Editor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>属性编辑</title>
    <link href="../Styles/dlg.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/pccom.css" rel="stylesheet" type="text/css" />
</head>
<body onkeydown="handleKey(event)">
    <form id="form1" runat="server" target="innerFrame">
    <au:SceneControl runat="server" />
    <script type="text/javascript">
        function onSaveClick() {

            var reValue = $HGRootNS.PropertyEditorControlBase.ValidateProperties();

            if (!reValue.isValid)	//返回是否通过
                return false; //不通过则阻止提交

            var strips = $find("tabs"); ;
            var allProperties = [];

            for (var i = 0; i < strips.get_tabPageCount(); i++) {
                var propertyForm = $find(strips.get_tabPage(i).get_tag() + "_Form");
                var ppts = propertyForm.get_properties();
                for (var k = 0; k < ppts.length; k++) {
                    allProperties.push(ppts[k]);
                }
            }

            $get("properties").value = Sys.Serialization.JavaScriptSerializer.serialize(allProperties);

            var saveBtn = $get("btSave");

            window.returnValue = true;

            if (saveBtn) {
                saveBtn.click();
            }
        }
    </script>
    <div>
        <asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="true">
            <Services>
                <asp:ServiceReference Path="~/Services/CommonServices.asmx" />
            </Services>
        </asp:ScriptManager>
    </div>
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            属性编辑<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-container5">
            <span id="description" runat="server"></span>
        </div>
        <div>
            <mcs:RelaxedTabStrip runat="server" ID="tabs">
            </mcs:RelaxedTabStrip>
        </div>
        <div id="panelContainer" style="display: none" runat="server">
        </div>
        <div style="display: none" id="alterKeyPanel">
            可选ID
            <asp:TextBox runat="server" ID="alterKey" />当前ID： <span style="text-indent: 2em"
                id="lblCurrentID" runat="server"></span>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" id="okButton" runat="server" onclick="return ($pc.getEnabled(this) && onSaveClick())"
                accesskey="S" class="pcdlg-button btn-def" value="保存(S)" /><input type="button" accesskey="C"
                    class="pcdlg-button btn-cancel" onclick="window.returnValue = false;window.close()"
                    value="关闭(C)" />
        </div>
    </div>
    <div style="display: none">
        <input type="hidden" id="properties" runat="server" />
        <input type="hidden" id="currentSchemaType" runat="server" />
        <input type="hidden" id="currentParentID" runat="server" />
        <soa:SubmitButton runat="server" ID="btSave" OnClick="SaveClick" Text="保存(S)" CssClass="pcdlg-button btn-def"
            RelativeControlID="okButton" PopupCaption="正在保存..." />
    </div>
    <div style="display: none">
        <iframe name="innerFrame"></iframe>
    </div>
    <script src="../Scripts/EditorScripts.js" type="text/javascript"></script>
    <script type="text/javascript">
        function handleKey(e) {
            if (e.ctrlKey && e.altKey && e.keyCode == 188) {
                $get("alterKeyPanel").style.display = "block";
            }
        }
    
    </script>
    </form>
</body>
</html>
